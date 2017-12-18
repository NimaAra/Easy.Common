namespace Easy.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Easy.Common.Extensions;
    using Easy.Common.Interfaces;
    using System.IO;

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

            // Default is 2 minutes: https://msdn.microsoft.com/en-us/library/system.net.servicepointmanager.dnsrefreshtimeout(v=vs.110).aspx
            ServicePointManager.DnsRefreshTimeout = (int)1.Minutes().TotalMilliseconds;
            
            // Increases the concurrent outbound connections
            ServicePointManager.DefaultConnectionLimit = 1024;
        }

        /// <summary>
        /// Gets the headers which should be sent with each request.
        /// </summary>
        public IDictionary<string, string> DefaultRequestHeaders => _client.DefaultRequestHeaders.ToDictionary(x => x.Key, x => x.Value.First());

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
        /// Sends an HTTP request as an asynchronous operation.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage message) => SendAsync(message, HttpCompletionOption.ResponseContentRead, CancellationToken.None);

        /// <summary>
        /// Sends an HTTP request as an asynchronous operation.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage message, CancellationToken cToken) => SendAsync(message, HttpCompletionOption.ResponseContentRead, cToken);

        /// <summary>
        /// Sends an HTTP request as an asynchronous operation.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage message, HttpCompletionOption option) => SendAsync(message, option, CancellationToken.None);

        /// <summary>
        /// Sends an HTTP request as an asynchronous operation.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage message, HttpCompletionOption option, CancellationToken cToken)
        {
            Ensure.NotNull(message, nameof(message));
            AddConnectionLeaseTimeout(message.RequestUri);
            return _client.SendAsync(message, option, cToken);
        }

        /// <summary>
        /// Sends a PUT request to the specified URI as an asynchronous operation
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="UriFormatException"/>
        public Task<HttpResponseMessage> PutAsync(string uri, HttpContent content)
        {
            var message = new HttpRequestMessage(HttpMethod.Put, uri);
            message.Content = content;
            return SendAsync(message);
        }

        /// <summary>
        /// Sends a PUT request to the specified URI as an asynchronous operation
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public Task<HttpResponseMessage> PutAsync(Uri uri, HttpContent content)
        {
            var message = new HttpRequestMessage(HttpMethod.Put, uri);
            message.Content = content;
            return SendAsync(message);
        }

        /// <summary>
        /// Sends a PUT request to the specified URI with a cancellation token as an asynchronous operation
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public Task<HttpResponseMessage> PutAsync(Uri uri, HttpContent content, CancellationToken cToken)
        {
            var message = new HttpRequestMessage(HttpMethod.Put, uri);
            message.Content = content;
            return SendAsync(message, cToken);
        }

        /// <summary>
        /// Sends a PUT request to the specified URI with a cancellation token as an asynchronous operation
        /// </summary>       
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="UriFormatException"/>
        public Task<HttpResponseMessage> PutAsync(string uri, HttpContent content, CancellationToken cToken)
        {
            var message = new HttpRequestMessage(HttpMethod.Put, uri);
            message.Content = content;
            return SendAsync(message, cToken);
        }

        /// <summary>
        /// Send a POST request to the specified Uri as an asynchronous operation
        /// </summary>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="UriFormatException"/>
        public Task<HttpResponseMessage> PostAsync(string uri, HttpContent content)
        {
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = content;
            return SendAsync(message);
        }

        /// <summary>
        /// Send a POST request to the specified Uri as an asynchronous operation
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public Task<HttpResponseMessage> PostAsync(Uri uri, HttpContent content)
        {
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = content;
            return SendAsync(message);
        }

        /// <summary>
        /// Send a POST request to the specified Uri with a cancellation token as an asynchronous operation
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public Task<HttpResponseMessage> PostAsync(Uri uri, HttpContent content, CancellationToken cToken)
        {
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = content;
            return SendAsync(message, cToken);
        }

        /// <summary>
        /// Send a POST request to the specified Uri with a cancellation token as an asynchronous operation
        /// </summary>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="UriFormatException"/>
        public Task<HttpResponseMessage> PostAsync(string uri, HttpContent content, CancellationToken cToken)
        {
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = content;
            return SendAsync(message, cToken);
        }

        /// <summary>
        /// Send a GET request to the specified Uri and returns the response body as a string in an asynchronous operation
        /// </summary>
        /// <exception cref="ArgumentException"/>
        ///<exception cref="UriFormatException"/>
        public Task<string> GetStringAsync(string uri)
        {
            AddConnectionLeaseTimeout(uri);
            return _client.GetStringAsync(uri);
        }

        /// <summary>
        /// Send a GET request to the specified Uri and returns the response body as a string in an asynchronous operation
        /// </summary>
        ///<exception cref="ArgumentNullException"/>
        public Task<string> GetStringAsync(Uri uri)
        {
            Ensure.NotNull(uri, "uri");
            AddConnectionLeaseTimeout(uri);
            return _client.GetStringAsync(uri);
        }

        /// <summary>
        /// Send a GET request to the specified Uri and returns the response body as a stream in an asynchronous operation
        /// </summary>
        /// <exception cref="ArgumentException"/>
        ///<exception cref="UriFormatException"/>
        public Task<Stream> GetStreamAsync(string uri)
        {
            AddConnectionLeaseTimeout(uri);
            return _client.GetStreamAsync(uri);
        }

        /// <summary>
        /// Send a GET request to the specified Uri and returns the response body as a stream in an asynchronous operation
        /// </summary>
        ///<exception cref="ArgumentNullException"/>
        public Task<Stream> GetStreamAsync(Uri uri)
        {
            Ensure.NotNull(uri, "uri");
            AddConnectionLeaseTimeout(uri);
            return _client.GetStreamAsync(uri);
        }

        /// <summary>
        /// Send a GET request to the specified Uri and returns the response body as a byte array in an asynchronous operation
        /// </summary>
        ///<exception cref="UriFormatException"/>
        /// <exception cref="ArgumentException"/>
        public Task<byte[]> GetByteArrayAsync(string uri)
        {
            AddConnectionLeaseTimeout(uri);
            return _client.GetByteArrayAsync(uri);
        }

        /// <summary>
        /// Send a GET request to the specified Uri and returns the response body as a byte array in an asynchronous operation
        /// </summary>
        ///<exception cref="ArgumentNullException"/>
        public Task<byte[]> GetByteArrayAsync(Uri uri)
        {
            Ensure.NotNull(uri, "uri");
            AddConnectionLeaseTimeout(uri);
            return _client.GetByteArrayAsync(uri);
        }

        /// <summary>
        /// Sends a GET request to the specifed Uri as an asynchronous operation
        /// </summary>
        /// <exception cref="UriFormatException"/>
        /// <exception cref="ArgumentException"/>
        public Task<HttpResponseMessage> GetAsync(string uri)
        {
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return SendAsync(message);
        }

        /// <summary>
        /// Sends a GET request to the specifed Uri as an asynchronous operation
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public Task<HttpResponseMessage> GetAsync(Uri uri)
        {
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return SendAsync(message);
        }

        /// <summary>
        /// Sends a GET request to the specifed Uri with a cancellation token as an asynchronous opertaion
        /// </summary>
        /// <exception cref="UriFormatException"/>
        /// <exception cref="ArgumentException"/>
        public Task<HttpResponseMessage> GetAsync(string uri, CancellationToken cToken)
        {
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return SendAsync(message, cToken);
        }

        /// <summary>
        /// Sends a GET request to the specifed Uri with a cancellation token as an asynchronous opertaion
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public Task<HttpResponseMessage> GetAsync(Uri uri, CancellationToken cToken)
        {
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return SendAsync(message, cToken);
        }

        /// <summary>
        /// Send a GET request to the specified Uri with an HTTP completion option as an asynchronous operation
        /// </summary>
        /// <exception cref="UriFormatException"/>
        /// <exception cref="ArgumentException"/>
        public Task<HttpResponseMessage> GetAsync(string uri, HttpCompletionOption completionOption)
        {
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return SendAsync(message, completionOption);
        }

        /// <summary>
        /// Send a GET request to the specified Uri with an HTTP completion option as an asynchronous operation
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public Task<HttpResponseMessage> GetAsync(Uri uri, HttpCompletionOption completionOption)
        {
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return SendAsync(message, completionOption);
        }

        /// <summary>
        /// Send a GET request to the specified Uri with an HTTP completion option and a cancellation token as an asynchronous operation
        /// </summary>
        /// <exception cref="UriFormatException"/>
        /// <exception cref="ArgumentException"/>
        public Task<HttpResponseMessage> GetAsync(string uri, HttpCompletionOption completionOption, CancellationToken cToken)
        {
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return SendAsync(message, completionOption, cToken);
        }

        /// <summary>
        /// Send a GET request to the specified Uri with an HTTP completion option and a cancellation token as an asynchronous operation
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public Task<HttpResponseMessage> GetAsync(Uri uri, HttpCompletionOption completionOption, CancellationToken cToken)
        {
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return SendAsync(message, completionOption, cToken);
        }

        /// <summary>
        /// Send a DELETE request to the specified Uri as an asynchronous operation
        /// </summary>
        /// <exception cref="UriFormatException"/>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        public Task<HttpResponseMessage> DeleteAsync(string uri)
        {
            var message = new HttpRequestMessage(HttpMethod.Delete, uri);
            return SendAsync(message);
        }

        /// <summary>
        /// Send a DELETE request to the specified Uri as an asynchronous operation
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="InvalidOperationException"/>
        public Task<HttpResponseMessage> DeleteAsync(Uri uri)
        {
            var message = new HttpRequestMessage(HttpMethod.Delete, uri);
            return SendAsync(message);
        }

        /// <summary>
        /// Send a DELETE request to the specified Uri with a cancellation token as an asynchronous operation
        /// </summary>
        /// <exception cref="UriFormatException"/>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        public Task<HttpResponseMessage> DeleteAsync(string uri, CancellationToken cToken)
        {
            var message = new HttpRequestMessage(HttpMethod.Delete, uri);
            return SendAsync(message, cToken);
        }

        /// <summary>
        /// Send a DELETE request to the specified Uri with a cancellation token as an asynchronous operation
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="InvalidOperationException"/>
        public Task<HttpResponseMessage> DeleteAsync(Uri uri, CancellationToken cToken)
        {
            var message = new HttpRequestMessage(HttpMethod.Delete, uri);
            return SendAsync(message, cToken);
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

        private void AddDefaultHeaders(IDictionary<string, string> headers)
        {
            if (headers == null) { return; }

            foreach (var item in headers)
            {
                _client.DefaultRequestHeaders.Add(item.Key, item.Value);
            }
        }

        private void AddRequestTimeout(TimeSpan? timeout)
        {
            if (!timeout.HasValue) { return; }
            _client.Timeout = timeout.Value;
        }

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

        private void AddConnectionLeaseTimeout(string endpointUri)
        {
            Ensure.NotNullOrEmptyOrWhiteSpace(endpointUri, nameof(endpointUri) + " must not be null, empty or whitespace");
            AddConnectionLeaseTimeout(new Uri(endpointUri));
        }
    }
}