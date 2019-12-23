namespace Easy.Common
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Easy.Common.Extensions;
    using Easy.Common.Interfaces;

    /// <summary>
    /// An abstraction over <see cref="HttpClient"/> to address the following issues:
    /// <para><see href="http://aspnetmonsters.com/2016/08/2016-08-27-httpclientwrong/"/></para>
    /// <para><see href="http://byterot.blogspot.co.uk/2016/07/singleton-httpclient-dns.html"/></para>
    /// <para><see href="http://naeem.khedarun.co.uk/blog/2016/11/30/httpclient-dns-settings-for-azure-cloud-services-and-traffic-manager-1480285049222/"/></para>
    /// </summary>
    public sealed class RestClient : IRestClient
    {
        private const int MAX_CONNECTION_PER_SERVER = 20;
        private static readonly TimeSpan ConnectionLifeTime = 1.Minutes();
        
        private readonly HttpClient _client;
        
#if !NETCOREAPP
        private readonly HashSet<EndpointCacheKey> _endpoints = new HashSet<EndpointCacheKey>();
#endif

        static RestClient() => ConfigureServicePointManager();

        /// <summary>
        /// Creates an instance of the <see cref="RestClient"/>.
        /// </summary>
        public RestClient(
            IDictionary<string, IEnumerable<string>> defaultRequestHeaders = null,
            HttpMessageHandler handler = null,
            Uri baseAddress = null,
            bool disposeHandler = true,
            TimeSpan? timeout = null,
            ulong? maxResponseContentBufferSize = null)
        {
            _client = new HttpClient(handler ?? GetHandler(), disposeHandler);

            AddBaseAddress(baseAddress);
            AddDefaultHeaders(defaultRequestHeaders);
            AddRequestTimeout(timeout);
            AddMaxResponseBufferSize(maxResponseContentBufferSize);
        }

        private static HttpMessageHandler GetHandler()
        {
#if NETSTANDARD2_0 || NET471 || NET472
            return new HttpClientHandler
            {
                MaxConnectionsPerServer = 20
            };
#elif NETCOREAPP
            return new SocketsHttpHandler
            {
                // https://github.com/dotnet/corefx/issues/26895
                // https://github.com/dotnet/corefx/issues/26331
                // https://github.com/dotnet/corefx/pull/26839
                PooledConnectionLifetime = ConnectionLifeTime,
                PooledConnectionIdleTimeout = ConnectionLifeTime,
                MaxConnectionsPerServer = MAX_CONNECTION_PER_SERVER
            };
#else
            return new HttpClientHandler();
#endif
        }

        /// <summary>
        /// Gets the headers which should be sent with each request.
        /// </summary>
        public IReadOnlyDictionary<string, string[]> DefaultRequestHeaders => 
            _client.DefaultRequestHeaders.ToDictionary(x => x.Key, x => x.Value.ToArray());

        /// <summary>
        /// Gets the time to wait before the request times out.
        /// </summary>
        public TimeSpan Timeout => _client.Timeout;

        /// <summary>
        /// Gets the maximum number of bytes to buffer when reading the response content.
        /// </summary>
        public uint MaxResponseContentBufferSize => (uint)_client.MaxResponseContentBufferSize;

        /// <summary>
        /// Gets the base address of Uniform Resource Identifier (URI) of the Internet resource used when sending requests.
        /// </summary>
        public Uri BaseAddress => _client.BaseAddress;

        /// <summary>
        /// Sends the given <paramref name="request"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request) => 
            SendAsync(request, HttpCompletionOption.ResponseContentRead, CancellationToken.None);

        /// <summary>
        /// Sends the given <paramref name="request"/> with the given <paramref name="cToken"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cToken) => 
            SendAsync(request, HttpCompletionOption.ResponseContentRead, cToken);

        /// <summary>
        /// Sends the given <paramref name="request"/> with the given <paramref name="option"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption option) => 
            SendAsync(request, option, CancellationToken.None);

        /// <summary>
        /// Sends the given <paramref name="request"/> with the given <paramref name="option"/> and <paramref name="cToken"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption option, CancellationToken cToken)
        {
            Ensure.NotNull(request, nameof(request));
            AddConnectionLeaseTimeout(request.RequestUri);
            return _client.SendAsync(request, option, cToken);
        }

        /// <summary>
        /// Sends a <c>GET</c> request to the specified <paramref name="uri"/>.
        /// </summary>
        /// <exception cref="UriFormatException"/>
        /// <exception cref="ArgumentException"/>
        public Task<HttpResponseMessage> GetAsync(string uri) => 
            SendAsync(new HttpRequestMessage(HttpMethod.Get, uri));

        /// <summary>
        /// Sends a <c>GET</c> request to the specified <paramref name="uri"/> with the given <paramref name="timeout"/>.
        /// </summary>
        /// <exception cref="UriFormatException"/>
        /// <exception cref="ArgumentException"/>
        public Task<HttpResponseMessage> GetAsync(string uri, TimeSpan timeout) => 
            GetAsync(new Uri(uri, UriKind.RelativeOrAbsolute), timeout);

        /// <summary>
        /// Sends a <c>GET</c> request to the specified <paramref name="uri"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public Task<HttpResponseMessage> GetAsync(Uri uri) => 
            SendAsync(new HttpRequestMessage(HttpMethod.Get, uri));

        /// <summary>
        /// Sends a <c>GET</c> request to the specified <paramref name="uri"/> with the given <paramref name="timeout"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public async Task<HttpResponseMessage> GetAsync(Uri uri, TimeSpan timeout)
        {
            using var cts = new CancellationTokenSource(timeout);
            return await SendAsync(new HttpRequestMessage(HttpMethod.Get, uri), cts.Token)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Sends a <c>GET</c> request to the specified <paramref name="uri"/> with the given <paramref name="cToken"/>.
        /// </summary>
        /// <exception cref="UriFormatException"/>
        /// <exception cref="ArgumentException"/>
        public Task<HttpResponseMessage> GetAsync(string uri, CancellationToken cToken) => 
            SendAsync(new HttpRequestMessage(HttpMethod.Get, uri), cToken);

        /// <summary>
        /// Sends a <c>GET</c> request to the specified <paramref name="uri"/> with the given <paramref name="cToken"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public Task<HttpResponseMessage> GetAsync(Uri uri, CancellationToken cToken) => 
            SendAsync(new HttpRequestMessage(HttpMethod.Get, uri), cToken);

        /// <summary>
        /// Sends a <c>GET</c> request to the specified <paramref name="uri"/> with the given <paramref name="option"/>.
        /// </summary>
        /// <exception cref="UriFormatException"/>
        /// <exception cref="ArgumentException"/>
        public Task<HttpResponseMessage> GetAsync(string uri, HttpCompletionOption option) => 
            SendAsync(new HttpRequestMessage(HttpMethod.Get, uri), option);

        /// <summary>
        /// Sends a <c>GET</c> request to the specified <paramref name="uri"/> with the given <paramref name="option"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public Task<HttpResponseMessage> GetAsync(Uri uri, HttpCompletionOption option) => 
            SendAsync(new HttpRequestMessage(HttpMethod.Get, uri), option);

        /// <summary>
        /// Sends a <c>GET</c> request to the specified <paramref name="uri"/> with the given <paramref name="option"/> and <paramref name="cToken"/>.
        /// </summary>
        /// <exception cref="UriFormatException"/>
        /// <exception cref="ArgumentException"/>
        public Task<HttpResponseMessage> GetAsync(string uri, HttpCompletionOption option, CancellationToken cToken) => 
            SendAsync(new HttpRequestMessage(HttpMethod.Get, uri), option, cToken);

        /// <summary>
        /// Sends a <c>GET</c> request to the specified <paramref name="uri"/> with the given <paramref name="option"/> and <paramref name="cToken"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public Task<HttpResponseMessage> GetAsync(Uri uri, HttpCompletionOption option, CancellationToken cToken) => 
            SendAsync(new HttpRequestMessage(HttpMethod.Get, uri), option, cToken);

        /// <summary>
        /// Sends a <c>PUT</c> request with the given <paramref name="content"/> to the specified <paramref name="uri"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="UriFormatException"/>
        public Task<HttpResponseMessage> PutAsync(string uri, HttpContent content) => 
            SendAsync(new HttpRequestMessage(HttpMethod.Put, uri) { Content = content });

        /// <summary>
        /// Sends a <c>PUT</c> request with the given <paramref name="content"/> to the specified <paramref name="uri"/> with the given <paramref name="timeout"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="UriFormatException"/>
        public Task<HttpResponseMessage> PutAsync(string uri, HttpContent content, TimeSpan timeout) => 
            PutAsync(new Uri(uri, UriKind.RelativeOrAbsolute), content, timeout);

        /// <summary>
        /// Sends a <c>PUT</c> request with the given <paramref name="content"/> to the specified <paramref name="uri"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public Task<HttpResponseMessage> PutAsync(Uri uri, HttpContent content) => 
            SendAsync(new HttpRequestMessage(HttpMethod.Put, uri) { Content = content });

        /// <summary>
        /// Sends a <c>PUT</c> request with the given <paramref name="content"/> to the specified <paramref name="uri"/> with the given <paramref name="timeout"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public async Task<HttpResponseMessage> PutAsync(Uri uri, HttpContent content, TimeSpan timeout)
        {
            using var cts = new CancellationTokenSource(timeout);
            return await SendAsync(new HttpRequestMessage(HttpMethod.Put, uri) { Content = content }, cts.Token)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Sends a <c>PUT</c> request with the given <paramref name="content"/> to the specified <paramref name="uri"/> 
        /// with the given <paramref name="cToken"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public Task<HttpResponseMessage> PutAsync(Uri uri, HttpContent content, CancellationToken cToken) => 
            SendAsync(new HttpRequestMessage(HttpMethod.Put, uri) { Content = content }, cToken);

        /// <summary>
        /// Sends a <c>PUT</c> request with the given <paramref name="content"/> to the specified <paramref name="uri"/> 
        /// with the given <paramref name="cToken"/>.
        /// </summary>     
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="UriFormatException"/>
        public Task<HttpResponseMessage> PutAsync(string uri, HttpContent content, CancellationToken cToken) => 
            SendAsync(new HttpRequestMessage(HttpMethod.Put, uri) { Content = content }, cToken);

        /// <summary>
        /// Sends a <c>POST</c> request with the given <paramref name="content"/> to the specified <paramref name="uri"/>.
        /// </summary>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="UriFormatException"/>
        public Task<HttpResponseMessage> PostAsync(string uri, HttpContent content) => 
            SendAsync(new HttpRequestMessage(HttpMethod.Post, uri) { Content = content });

        /// <summary>
        /// Sends a <c>POST</c> request with the given <paramref name="content"/> to the specified <paramref name="uri"/> with the given <paramref name="timeout"/>.
        /// </summary>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="UriFormatException"/>
        public Task<HttpResponseMessage> PostAsync(string uri, HttpContent content, TimeSpan timeout) => 
            PostAsync(new Uri(uri, UriKind.RelativeOrAbsolute), content, timeout);

        /// <summary>
        /// Sends a <c>POST</c> request with the given <paramref name="content"/> to the specified <paramref name="uri"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public Task<HttpResponseMessage> PostAsync(Uri uri, HttpContent content) => 
            SendAsync(new HttpRequestMessage(HttpMethod.Post, uri) { Content = content });

        /// <summary>
        /// Sends a <c>POST</c> request with the given <paramref name="content"/> to the specified <paramref name="uri"/> with the given <paramref name="timeout"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public async Task<HttpResponseMessage> PostAsync(Uri uri, HttpContent content, TimeSpan timeout)
        {
            using var cts = new CancellationTokenSource(timeout);
            return await SendAsync(new HttpRequestMessage(HttpMethod.Post, uri) { Content = content }, cts.Token)
                .ConfigureAwait(false);
        } 
        
        /// <summary>
        /// Sends a <c>POST</c> request with the given <paramref name="content"/> to the specified <paramref name="uri"/> 
        /// with the given <paramref name="cToken"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public Task<HttpResponseMessage> PostAsync(Uri uri, HttpContent content, CancellationToken cToken) => 
            SendAsync(new HttpRequestMessage(HttpMethod.Post, uri) { Content = content }, cToken);

        /// <summary>
        /// Sends a <c>POST</c> request with the given <paramref name="content"/> to the specified <paramref name="uri"/> 
        /// with the given <paramref name="cToken"/>.
        /// </summary>     
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="UriFormatException"/>
        public Task<HttpResponseMessage> PostAsync(string uri, HttpContent content, CancellationToken cToken) => 
            SendAsync(new HttpRequestMessage(HttpMethod.Post, uri) { Content = content }, cToken);

        /// <summary>
        /// Sends a <c>DELETE</c> request to the specified <paramref name="uri"/>.
        /// </summary>
        /// <exception cref="UriFormatException"/>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        public Task<HttpResponseMessage> DeleteAsync(string uri) => 
            SendAsync(new HttpRequestMessage(HttpMethod.Delete, uri));

        /// <summary>
        /// Sends a <c>DELETE</c> request to the specified <paramref name="uri"/> with the given <paramref name="timeout"/>.
        /// </summary>
        /// <exception cref="UriFormatException"/>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        public Task<HttpResponseMessage> DeleteAsync(string uri, TimeSpan timeout) => 
            DeleteAsync(new Uri(uri, UriKind.RelativeOrAbsolute), timeout);

        /// <summary>
        /// Sends a <c>DELETE</c> request to the specified <paramref name="uri"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="InvalidOperationException"/>
        public Task<HttpResponseMessage> DeleteAsync(Uri uri) => 
            SendAsync(new HttpRequestMessage(HttpMethod.Delete, uri));

        /// <summary>
        /// Sends a <c>DELETE</c> request to the specified <paramref name="uri"/> with the given <paramref name="timeout"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="InvalidOperationException"/>
        public async Task<HttpResponseMessage> DeleteAsync(Uri uri, TimeSpan timeout)
        {
            using var cts = new CancellationTokenSource(timeout);
            return await SendAsync(new HttpRequestMessage(HttpMethod.Delete, uri), cts.Token)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Sends a <c>DELETE</c> request to the specified <paramref name="uri"/> with the given <paramref name="cToken"/>.
        /// </summary>
        /// <exception cref="UriFormatException"/>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        public Task<HttpResponseMessage> DeleteAsync(string uri, CancellationToken cToken) => 
            SendAsync(new HttpRequestMessage(HttpMethod.Delete, uri), cToken);

        /// <summary>
        /// Sends a <c>DELETE</c> request to the specified <paramref name="uri"/> with the given <paramref name="cToken"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="InvalidOperationException"/>
        public Task<HttpResponseMessage> DeleteAsync(Uri uri, CancellationToken cToken) => 
            SendAsync(new HttpRequestMessage(HttpMethod.Delete, uri), cToken);

        /// <summary>
        /// Sends a <c>GET</c> request to the specified <paramref name="uri"/>.
        /// </summary>
        /// <exception cref="ArgumentException"/>
        ///<exception cref="UriFormatException"/>
        public Task<string> GetStringAsync(string uri)
        {
            Ensure.NotNullOrEmptyOrWhiteSpace(uri);
            return GetStringAsync(new Uri(uri, UriKind.RelativeOrAbsolute));
        }

        /// <summary>
        /// Sends a <c>GET</c> request to the specified <paramref name="uri"/> with the given <paramref name="timeout"/>.
        /// </summary>
        /// <exception cref="ArgumentException"/>
        ///<exception cref="UriFormatException"/>
        public Task<string> GetStringAsync(string uri, TimeSpan timeout)
        {
            Ensure.NotNullOrEmptyOrWhiteSpace(uri);
            return GetStringAsync(new Uri(uri, UriKind.RelativeOrAbsolute), timeout);
        }

        /// <summary>
        /// Sends a <c>GET</c> request to the specified <paramref name="uri"/>.
        /// </summary>
        ///<exception cref="ArgumentNullException"/>
        public Task<string> GetStringAsync(Uri uri)
        {
            Ensure.NotNull(uri, nameof(uri));
            AddConnectionLeaseTimeout(uri);
            return _client.GetStringAsync(uri);
        }

        /// <summary>
        /// Sends a <c>GET</c> request to the specified <paramref name="uri"/> with the given <paramref name="timeout"/>.
        /// </summary>
        ///<exception cref="ArgumentNullException"/>
        public async Task<string> GetStringAsync(Uri uri, TimeSpan timeout)
        {
            Ensure.NotNull(uri, nameof(uri));
            var resp = await GetAsync(uri, timeout).ConfigureAwait(false);
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Sends a <c>GET</c> request to the specified <paramref name="uri"/>.
        /// </summary>
        /// <exception cref="ArgumentException"/>
        ///<exception cref="UriFormatException"/>
        public Task<Stream> GetStreamAsync(string uri)
        {
            Ensure.NotNullOrEmptyOrWhiteSpace(uri);
            return GetStreamAsync(new Uri(uri, UriKind.RelativeOrAbsolute));
        }

        /// <summary>
        /// Sends a <c>GET</c> request to the specified <paramref name="uri"/> with the given <paramref name="timeout"/>.
        /// </summary>
        /// <exception cref="ArgumentException"/>
        ///<exception cref="UriFormatException"/>
        public Task<Stream> GetStreamAsync(string uri, TimeSpan timeout)
        {
            Ensure.NotNullOrEmptyOrWhiteSpace(uri);
            return GetStreamAsync(new Uri(uri, UriKind.RelativeOrAbsolute), timeout);
        }

        /// <summary>
        /// Sends a <c>GET</c> request to the specified <paramref name="uri"/>.
        /// </summary>
        ///<exception cref="ArgumentNullException"/>
        public Task<Stream> GetStreamAsync(Uri uri)
        {
            Ensure.NotNull(uri, nameof(uri));
            AddConnectionLeaseTimeout(uri);
            return _client.GetStreamAsync(uri);
        }

        /// <summary>
        /// Sends a <c>GET</c> request to the specified <paramref name="uri"/> with the given <paramref name="timeout"/>.
        /// </summary>
        ///<exception cref="ArgumentNullException"/>
        public async Task<Stream> GetStreamAsync(Uri uri, TimeSpan timeout)
        {
            Ensure.NotNull(uri, nameof(uri));
            var resp = await GetAsync(uri, timeout).ConfigureAwait(false);
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadAsStreamAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Sends a <c>GET</c> request to the specified <paramref name="uri"/>.
        /// </summary>
        ///<exception cref="UriFormatException"/>
        /// <exception cref="ArgumentException"/>
        public Task<byte[]> GetByteArrayAsync(string uri)
        {
            Ensure.NotNullOrEmptyOrWhiteSpace(uri);
            return GetByteArrayAsync(new Uri(uri, UriKind.RelativeOrAbsolute));
        }

        /// <summary>
        /// Sends a <c>GET</c> request to the specified <paramref name="uri"/> with the given <paramref name="timeout"/>.
        /// </summary>
        ///<exception cref="UriFormatException"/>
        /// <exception cref="ArgumentException"/>
        public Task<byte[]> GetByteArrayAsync(string uri, TimeSpan timeout)
        {
            Ensure.NotNullOrEmptyOrWhiteSpace(uri);
            return GetByteArrayAsync(new Uri(uri, UriKind.RelativeOrAbsolute), timeout);
        }

        /// <summary>
        /// Sends a <c>GET</c> request to the specified <paramref name="uri"/>.
        /// </summary>
        ///<exception cref="ArgumentNullException"/>
        public Task<byte[]> GetByteArrayAsync(Uri uri)
        {
            Ensure.NotNull(uri, nameof(uri));
            AddConnectionLeaseTimeout(uri);
            return _client.GetByteArrayAsync(uri);
        }

        /// <summary>
        /// Sends a <c>GET</c> request to the specified <paramref name="uri"/> with the given <paramref name="timeout"/>.
        /// </summary>
        ///<exception cref="ArgumentNullException"/>
        public async Task<byte[]> GetByteArrayAsync(Uri uri, TimeSpan timeout)
        {
            Ensure.NotNull(uri, nameof(uri));
            var resp = await GetAsync(uri, timeout).ConfigureAwait(false);
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Cancels all pending requests on this instance.
        /// </summary>
        public void CancelPendingRequests() => _client.CancelPendingRequests();

        /// <summary>
        /// Releases the unmanaged resources and disposes of the managed resources used by the <see cref="HttpClient"/>.
        /// </summary>
        public void Dispose() => _client.Dispose();
        
        private static void ConfigureServicePointManager()
        {
            // Default is 2 minutes, see https://msdn.microsoft.com/en-us/library/system.net.servicepointmanager.dnsrefreshtimeout(v=vs.110).aspx
            ServicePointManager.DnsRefreshTimeout = (int)ConnectionLifeTime.TotalMilliseconds;

            // Increases the concurrent outbound connections
            ServicePointManager.DefaultConnectionLimit = MAX_CONNECTION_PER_SERVER;
        }

        private void AddBaseAddress(Uri uri)
        {
            if (uri is null) { return; }

            AddConnectionLeaseTimeout(uri);
            _client.BaseAddress = uri;
        }

        private void AddDefaultHeaders(IEnumerable<KeyValuePair<string, IEnumerable<string>>> headers)
        {
            if (headers is null) { return; }

            foreach (var item in headers)
            {
                _client.DefaultRequestHeaders.Add(item.Key, item.Value);
            }
        }

        private void AddRequestTimeout(TimeSpan? timeout) => 
            _client.Timeout = timeout ?? System.Threading.Timeout.InfiniteTimeSpan;

        private void AddMaxResponseBufferSize(ulong? size)
        {
            if (!size.HasValue) { return; }
            _client.MaxResponseContentBufferSize = (long)size.Value;
        }

        // ReSharper disable once MemberCanBeMadeStatic.Local
        // ReSharper disable once UnusedParameter.Local
        private void AddConnectionLeaseTimeout(Uri endpoint)
        {
#if !NETCOREAPP
            if (!endpoint.IsAbsoluteUri) { return; }
            
            var key = new EndpointCacheKey(endpoint);
            lock (_endpoints)
            {
                if (_endpoints.Contains(key)) { return; }

                ServicePointManager.FindServicePoint(endpoint)
                    .ConnectionLeaseTimeout = (int)ConnectionLifeTime.TotalMilliseconds;
                _endpoints.Add(key);
            }
#endif
        }

#if !NETCOREAPP
        private struct EndpointCacheKey : IEquatable<EndpointCacheKey>
        {
            private readonly Uri _uri;

            public EndpointCacheKey(Uri uri) => _uri = uri;

            public bool Equals(EndpointCacheKey other) => _uri == other._uri;

            public override bool Equals(object obj) => obj is EndpointCacheKey other && Equals(other);

            public override int GetHashCode() => 
                HashHelper.GetHashCode(_uri.Scheme, _uri.DnsSafeHost, _uri.Port);

            public static bool operator ==(EndpointCacheKey left, EndpointCacheKey right) => left.Equals(right);

            public static bool operator !=(EndpointCacheKey left, EndpointCacheKey right) => !left.Equals(right);
        }
#endif
    }
}
