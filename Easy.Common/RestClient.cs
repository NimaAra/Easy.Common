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
        private readonly HttpClient _client;
        private readonly HashSet<Uri> _endpoints;
        private readonly TimeSpan _connectionCloseTimeoutPeriod;

        static RestClient() => ConfigureServicePointManager();

        /// <summary>
        /// Creates an instance of the <see cref="RestClient"/>.
        /// </summary>
        public RestClient(
            IDictionary<string, string> defaultRequestHeaders = null,
            HttpMessageHandler handler = null,
            bool disposeHandler = true,
            TimeSpan? timeout = null,
            ulong? maxResponseContentBufferSize = null)
        {
            _client = handler == null ? new HttpClient() : new HttpClient(handler, disposeHandler);

            AddDefaultHeaders(defaultRequestHeaders);
            AddRequestTimeout(timeout);
            AddMaxResponseBufferSize(maxResponseContentBufferSize);
            
            _endpoints = new HashSet<Uri>();
            _connectionCloseTimeoutPeriod = 1.Minutes();
        }

        /// <summary>
        /// Gets the headers which should be sent with each request.
        /// </summary>
        public IDictionary<string, string> DefaultRequestHeaders 
            => _client.DefaultRequestHeaders.ToDictionary(x => x.Key, x => x.Value.First());

        /// <summary>
        /// Gets the time to wait before the request times out.
        /// </summary>
        public TimeSpan Timeout => _client.Timeout;

        /// <summary>
        /// Gets the maximum number of bytes to buffer when reading the response content.
        /// </summary>
        public uint MaxResponseContentBufferSize => (uint)_client.MaxResponseContentBufferSize;

        /// <summary>
        /// Gets all of the endpoints which this instance has sent a request to.
        /// </summary>
        public Uri[] Endpoints
        {
            get { lock (_endpoints) { return _endpoints.ToArray(); } }
        }

        /// <summary>
        /// Sends the given <paramref name="request"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
            => SendAsync(request, HttpCompletionOption.ResponseContentRead, CancellationToken.None);

        /// <summary>
        /// Sends the given <paramref name="request"/> with the given <paramref name="cToken"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cToken)
            => SendAsync(request, HttpCompletionOption.ResponseContentRead, cToken);

        /// <summary>
        /// Sends the given <paramref name="request"/> with the given <paramref name="option"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption option)
            => SendAsync(request, option, CancellationToken.None);

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
        public Task<HttpResponseMessage> GetAsync(string uri) 
            => SendAsync(new HttpRequestMessage(HttpMethod.Get, uri));

        /// <summary>
        /// Sends a <c>GET</c> request to the specified <paramref name="uri"/> with the given <paramref name="timeout"/>.
        /// </summary>
        /// <exception cref="UriFormatException"/>
        /// <exception cref="ArgumentException"/>
        public Task<HttpResponseMessage> GetAsync(string uri, TimeSpan timeout)
            => GetAsync(new Uri(uri, UriKind.RelativeOrAbsolute), timeout);

        /// <summary>
        /// Sends a <c>GET</c> request to the specified <paramref name="uri"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public Task<HttpResponseMessage> GetAsync(Uri uri) 
            => SendAsync(new HttpRequestMessage(HttpMethod.Get, uri));

        /// <summary>
        /// Sends a <c>GET</c> request to the specified <paramref name="uri"/> with the given <paramref name="timeout"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public Task<HttpResponseMessage> GetAsync(Uri uri, TimeSpan timeout) 
            => SendAsync(new HttpRequestMessage(HttpMethod.Get, uri), new CancellationTokenSource(timeout).Token);

        /// <summary>
        /// Sends a <c>GET</c> request to the specified <paramref name="uri"/> with the given <paramref name="cToken"/>.
        /// </summary>
        /// <exception cref="UriFormatException"/>
        /// <exception cref="ArgumentException"/>
        public Task<HttpResponseMessage> GetAsync(string uri, CancellationToken cToken)
            => SendAsync(new HttpRequestMessage(HttpMethod.Get, uri), cToken);

        /// <summary>
        /// Sends a <c>GET</c> request to the specified <paramref name="uri"/> with the given <paramref name="cToken"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public Task<HttpResponseMessage> GetAsync(Uri uri, CancellationToken cToken)
            => SendAsync(new HttpRequestMessage(HttpMethod.Get, uri), cToken);

        /// <summary>
        /// Sends a <c>GET</c> request to the specified <paramref name="uri"/> with the given <paramref name="option"/>.
        /// </summary>
        /// <exception cref="UriFormatException"/>
        /// <exception cref="ArgumentException"/>
        public Task<HttpResponseMessage> GetAsync(string uri, HttpCompletionOption option)
            => SendAsync(new HttpRequestMessage(HttpMethod.Get, uri), option);

        /// <summary>
        /// Sends a <c>GET</c> request to the specified <paramref name="uri"/> with the given <paramref name="option"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public Task<HttpResponseMessage> GetAsync(Uri uri, HttpCompletionOption option)
            => SendAsync(new HttpRequestMessage(HttpMethod.Get, uri), option);

        /// <summary>
        /// Sends a <c>GET</c> request to the specified <paramref name="uri"/> with the given <paramref name="option"/> and <paramref name="cToken"/>.
        /// </summary>
        /// <exception cref="UriFormatException"/>
        /// <exception cref="ArgumentException"/>
        public Task<HttpResponseMessage> GetAsync(string uri, HttpCompletionOption option, CancellationToken cToken)
            => SendAsync(new HttpRequestMessage(HttpMethod.Get, uri), option, cToken);

        /// <summary>
        /// Sends a <c>GET</c> request to the specified <paramref name="uri"/> with the given <paramref name="option"/> and <paramref name="cToken"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public Task<HttpResponseMessage> GetAsync(Uri uri, HttpCompletionOption option, CancellationToken cToken)
            => SendAsync(new HttpRequestMessage(HttpMethod.Get, uri), option, cToken);

        /// <summary>
        /// Sends a <c>PUT</c> request with the given <paramref name="content"/> to the specified <paramref name="uri"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="UriFormatException"/>
        public Task<HttpResponseMessage> PutAsync(string uri, HttpContent content)
            => SendAsync(new HttpRequestMessage(HttpMethod.Put, uri) { Content = content });

        /// <summary>
        /// Sends a <c>PUT</c> request with the given <paramref name="content"/> to the specified <paramref name="uri"/> with the given <paramref name="timeout"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="UriFormatException"/>
        public Task<HttpResponseMessage> PutAsync(string uri, HttpContent content, TimeSpan timeout)
            => PutAsync(new Uri(uri, UriKind.RelativeOrAbsolute), content, timeout);

        /// <summary>
        /// Sends a <c>PUT</c> request with the given <paramref name="content"/> to the specified <paramref name="uri"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public Task<HttpResponseMessage> PutAsync(Uri uri, HttpContent content)
            => SendAsync(new HttpRequestMessage(HttpMethod.Put, uri) { Content = content });

        /// <summary>
        /// Sends a <c>PUT</c> request with the given <paramref name="content"/> to the specified <paramref name="uri"/> with the given <paramref name="timeout"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public Task<HttpResponseMessage> PutAsync(Uri uri, HttpContent content, TimeSpan timeout)
            => SendAsync(new HttpRequestMessage(HttpMethod.Put, uri) { Content = content }, new CancellationTokenSource(timeout).Token);

        /// <summary>
        /// Sends a <c>PUT</c> request with the given <paramref name="content"/> to the specified <paramref name="uri"/> 
        /// with the given <paramref name="cToken"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public Task<HttpResponseMessage> PutAsync(Uri uri, HttpContent content, CancellationToken cToken)
            => SendAsync(new HttpRequestMessage(HttpMethod.Put, uri) { Content = content }, cToken);

        /// <summary>
        /// Sends a <c>PUT</c> request with the given <paramref name="content"/> to the specified <paramref name="uri"/> 
        /// with the given <paramref name="cToken"/>.
        /// </summary>     
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="UriFormatException"/>
        public Task<HttpResponseMessage> PutAsync(string uri, HttpContent content, CancellationToken cToken)
            => SendAsync(new HttpRequestMessage(HttpMethod.Put, uri) { Content = content }, cToken);

        /// <summary>
        /// Sends a <c>POST</c> request with the given <paramref name="content"/> to the specified <paramref name="uri"/>.
        /// </summary>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="UriFormatException"/>
        public Task<HttpResponseMessage> PostAsync(string uri, HttpContent content)
            => SendAsync(new HttpRequestMessage(HttpMethod.Post, uri) { Content = content });

        /// <summary>
        /// Sends a <c>POST</c> request with the given <paramref name="content"/> to the specified <paramref name="uri"/> with the given <paramref name="timeout"/>.
        /// </summary>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="UriFormatException"/>
        public Task<HttpResponseMessage> PostAsync(string uri, HttpContent content, TimeSpan timeout)
            => PostAsync(new Uri(uri, UriKind.RelativeOrAbsolute), content, timeout);

        /// <summary>
        /// Sends a <c>POST</c> request with the given <paramref name="content"/> to the specified <paramref name="uri"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public Task<HttpResponseMessage> PostAsync(Uri uri, HttpContent content)
            => SendAsync(new HttpRequestMessage(HttpMethod.Post, uri) { Content = content });

        /// <summary>
        /// Sends a <c>POST</c> request with the given <paramref name="content"/> to the specified <paramref name="uri"/> with the given <paramref name="timeout"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public Task<HttpResponseMessage> PostAsync(Uri uri, HttpContent content, TimeSpan timeout)
            => SendAsync(new HttpRequestMessage(HttpMethod.Post, uri) { Content = content }, new CancellationTokenSource(timeout).Token);

        /// <summary>
        /// Sends a <c>POST</c> request with the given <paramref name="content"/> to the specified <paramref name="uri"/> 
        /// with the given <paramref name="cToken"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public Task<HttpResponseMessage> PostAsync(Uri uri, HttpContent content, CancellationToken cToken)
            => SendAsync(new HttpRequestMessage(HttpMethod.Post, uri) { Content = content }, cToken);

        /// <summary>
        /// Sends a <c>POST</c> request with the given <paramref name="content"/> to the specified <paramref name="uri"/> 
        /// with the given <paramref name="cToken"/>.
        /// </summary>     
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="UriFormatException"/>
        public Task<HttpResponseMessage> PostAsync(string uri, HttpContent content, CancellationToken cToken)
            => SendAsync(new HttpRequestMessage(HttpMethod.Post, uri) { Content = content }, cToken);

        /// <summary>
        /// Sends a <c>DELETE</c> request to the specified <paramref name="uri"/>.
        /// </summary>
        /// <exception cref="UriFormatException"/>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        public Task<HttpResponseMessage> DeleteAsync(string uri)
            => SendAsync(new HttpRequestMessage(HttpMethod.Delete, uri));

        /// <summary>
        /// Sends a <c>DELETE</c> request to the specified <paramref name="uri"/> with the given <paramref name="timeout"/>.
        /// </summary>
        /// <exception cref="UriFormatException"/>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        public Task<HttpResponseMessage> DeleteAsync(string uri, TimeSpan timeout)
            => DeleteAsync(new Uri(uri, UriKind.RelativeOrAbsolute), timeout);

        /// <summary>
        /// Sends a <c>DELETE</c> request to the specified <paramref name="uri"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="InvalidOperationException"/>
        public Task<HttpResponseMessage> DeleteAsync(Uri uri)
            => SendAsync(new HttpRequestMessage(HttpMethod.Delete, uri));

        /// <summary>
        /// Sends a <c>DELETE</c> request to the specified <paramref name="uri"/> with the given <paramref name="timeout"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="InvalidOperationException"/>
        public Task<HttpResponseMessage> DeleteAsync(Uri uri, TimeSpan timeout) 
            => SendAsync(new HttpRequestMessage(HttpMethod.Delete, uri), new CancellationTokenSource(timeout).Token);

        /// <summary>
        /// Sends a <c>DELETE</c> request to the specified <paramref name="uri"/> with the given <paramref name="cToken"/>.
        /// </summary>
        /// <exception cref="UriFormatException"/>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        public Task<HttpResponseMessage> DeleteAsync(string uri, CancellationToken cToken)
            => SendAsync(new HttpRequestMessage(HttpMethod.Delete, uri), cToken);

        /// <summary>
        /// Sends a <c>DELETE</c> request to the specified <paramref name="uri"/> with the given <paramref name="cToken"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="InvalidOperationException"/>
        public Task<HttpResponseMessage> DeleteAsync(Uri uri, CancellationToken cToken)
            => SendAsync(new HttpRequestMessage(HttpMethod.Delete, uri), cToken);

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
        /// Clears all of the endpoints which this instance has sent a request to.
        /// </summary>
        public void ClearEndpoints()
        {
            lock (_endpoints) { _endpoints.Clear(); }
        }

        /// <summary>
        /// Cancels all pending requests on this instance.
        /// </summary>
        public void CancelPendingRequests() => _client.CancelPendingRequests();

        /// <summary>
        /// Releases the unmanaged resources and disposes of the managed resources used by the <see cref="HttpClient"/>.
        /// </summary>
        public void Dispose()
        {
            _client.Dispose();
            lock (_endpoints) { _endpoints.Clear(); }
        }
        
        private static void ConfigureServicePointManager()
        {
            // Default is 2 minutes, see https://msdn.microsoft.com/en-us/library/system.net.servicepointmanager.dnsrefreshtimeout(v=vs.110).aspx
            ServicePointManager.DnsRefreshTimeout = (int)1.Minutes().TotalMilliseconds;

            // Increases the concurrent outbound connections
            ServicePointManager.DefaultConnectionLimit = 1024;
        }

        private void AddDefaultHeaders(IDictionary<string, string> headers)
        {
            if (headers == null) { return; }

            foreach (var item in headers)
            {
                _client.DefaultRequestHeaders.Add(item.Key, item.Value);
            }
        }

        private void AddRequestTimeout(TimeSpan? timeout) 
            => _client.Timeout = timeout ?? System.Threading.Timeout.InfiniteTimeSpan;

        private void AddMaxResponseBufferSize(ulong? size)
        {
            if (!size.HasValue) { return; }
            _client.MaxResponseContentBufferSize = (long)size.Value;
        }

        private void AddConnectionLeaseTimeout(Uri endpoint)
        {
            lock (_endpoints)
            {
                if (_endpoints.Contains(endpoint)) { return; }

                ServicePointManager.FindServicePoint(endpoint)
                    .ConnectionLeaseTimeout = (int)_connectionCloseTimeoutPeriod.TotalMilliseconds;
                _endpoints.Add(endpoint);
            }
        }
    }
}
