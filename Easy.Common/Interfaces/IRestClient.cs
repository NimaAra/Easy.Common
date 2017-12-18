namespace Easy.Common.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Specifies the contract to be implemented by a rest client.
    /// </summary>
    public interface IRestClient : IDisposable
    {
        /// <summary>
        /// Gets the headers which should be sent with each request.
        /// </summary>
        IDictionary<string, string> DefaultRequestHeaders { get; }

        /// <summary>
        /// Gets the time to wait before the request times out.
        /// </summary>
        TimeSpan Timeout { get; }

        /// <summary>
        /// Gets the maximum number of bytes to buffer when reading the response content.
        /// </summary>
        uint MaxResponseContentBufferSize { get; }

        /// <summary>
        /// Gets all of the endpoints which this instance has sent a request to.
        /// </summary>
        Uri[] Endpoints { get; }

        /// <summary>
        /// Sends an HTTP request as an asynchronous operation.
        /// </summary>
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage message);

        /// <summary>
        /// Sends an HTTP request as an asynchronous operation.
        /// </summary>
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage message, CancellationToken cToken);

        /// <summary>
        /// Sends an HTTP request as an asynchronous operation.
        /// </summary>
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage message, HttpCompletionOption option);

        /// <summary>
        /// Sends an HTTP request as an asynchronous operation.
        /// </summary>
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage message, HttpCompletionOption option, CancellationToken cToken);

        /// <summary>
        /// Sends a PUT request to the specified URI as an asynchronous operation
        /// </summary>
        Task<HttpResponseMessage> PutAsync(string uri, HttpContent content);

        /// <summary>
        /// Sends a PUT request to the specified URI as an asynchronous operation
        /// </summary>
        Task<HttpResponseMessage> PutAsync(Uri uri, HttpContent content);

        /// <summary>
        /// Sends a PUT request to the specified URI with a cancellation token as an asynchronous operation
        /// </summary>
        Task<HttpResponseMessage> PutAsync(Uri uri, HttpContent content, CancellationToken cancellationToken);

        /// <summary>
        /// Sends a PUT request to the specified URI with a cancellation token as an asynchronous operation
        /// </summary>       
        Task<HttpResponseMessage> PutAsync(string uri, HttpContent content, CancellationToken cancellationToken);

        /// <summary>
        /// Send a POST request to the specified Uri as an asynchronous operation
        /// </summary>
        Task<HttpResponseMessage> PostAsync(string uri, HttpContent content);

        /// <summary>
        /// Send a POST request to the specified Uri as an asynchronous operation
        /// </summary>
        Task<HttpResponseMessage> PostAsync(Uri uri, HttpContent content);

        /// <summary>
        /// Send a POST request to the specified Uri with a cancellation token as an asynchronous operation
        /// </summary>
        Task<HttpResponseMessage> PostAsync(Uri uri, HttpContent content, CancellationToken cancellationToken);

        /// <summary>
        /// Send a POST request to the specified Uri with a cancellation token as an asynchronous operation
        /// </summary>
        Task<HttpResponseMessage> PostAsync(string uri, HttpContent content, CancellationToken cancellationToken);

        /// <summary>
        /// Send a GET request to the specified Uri and returns the response body as a string in an asynchronous operation
        /// </summary>
        Task<string> GetStringAsync(string uri);

        /// <summary>
        /// Send a GET request to the specified Uri and returns the response body as a string in an asynchronous operation
        /// </summary>
        Task<string> GetStringAsync(Uri uri);

        /// <summary>
        /// Send a GET request to the specified Uri and returns the response body as a stream in an asynchronous operation
        /// </summary>
        Task<Stream> GetStreamAsync(string uri);

        /// <summary>
        /// Send a GET request to the specified Uri and returns the response body as a stream in an asynchronous operation
        /// </summary>
        Task<Stream> GetStreamAsync(Uri uri);

        /// <summary>
        /// Send a GET request to the specified Uri and returns the response body as a byte array in an asynchronous operation
        /// </summary>
        Task<byte[]> GetByteArrayAsync(string uri);

        /// <summary>
        /// Send a GET request to the specified Uri and returns the response body as a byte array in an asynchronous operation
        /// </summary>
        Task<byte[]> GetByteArrayAsync(Uri uri);

        /// <summary>
        /// Sends a GET request to the specifed Uri as an asynchronous operation
        /// </summary>
        Task<HttpResponseMessage> GetAsync(string uri);

        /// <summary>
        /// Sends a GET request to the specifed Uri as an asynchronous operation
        /// </summary>
        Task<HttpResponseMessage> GetAsync(Uri uri);

        /// <summary>
        /// Sends a GET request to the specifed Uri with a cancellation token as an asynchronous opertaion
        /// </summary>
        Task<HttpResponseMessage> GetAsync(string uri, CancellationToken cancellationToken);

        /// <summary>
        /// Sends a GET request to the specifed Uri with a cancellation token as an asynchronous opertaion
        /// </summary>
        Task<HttpResponseMessage> GetAsync(Uri uri, CancellationToken cancellationToken);

        /// <summary>
        /// Send a GET request to the specified Uri with an HTTP completion option as an asynchronous operation
        /// </summary>
        Task<HttpResponseMessage> GetAsync(string uri, HttpCompletionOption completionOption);

        /// <summary>
        /// Send a GET request to the specified Uri with an HTTP completion option as an asynchronous operation
        /// </summary>
        Task<HttpResponseMessage> GetAsync(Uri uri, HttpCompletionOption completionOption);

        /// <summary>
        /// Send a GET request to the specified Uri with an HTTP completion option and a cancellation token as an asynchronous operation
        /// </summary>
        Task<HttpResponseMessage> GetAsync(string uri, HttpCompletionOption completionOption, CancellationToken cancellationToken);

        /// <summary>
        /// Send a GET request to the specified Uri with an HTTP completion option and a cancellation token as an asynchronous operation
        /// </summary>
        Task<HttpResponseMessage> GetAsync(Uri uri, HttpCompletionOption completionOption, CancellationToken cancellationToken);

        /// <summary>
        /// Send a DELETE request to the specified Uri as an asynchronous operation
        /// </summary>
        Task<HttpResponseMessage> DeleteAsync(string uri);

        /// <summary>
        /// Send a DELETE request to the specified Uri as an asynchronous operation
        /// </summary>
        Task<HttpResponseMessage> DeleteAsync(Uri uri);

        /// <summary>
        /// Send a DELETE request to the specified Uri with a cancellation token as an asynchronous operation
        /// </summary>
        Task<HttpResponseMessage> DeleteAsync(string uri, CancellationToken cancellationToken);

        /// <summary>
        /// Send a DELETE request to the specified Uri with a cancellation token as an asynchronous operation
        /// </summary>
        Task<HttpResponseMessage> DeleteAsync(Uri uri, CancellationToken cancellationToken);


        /// <summary>
        /// Clears all of the endpoints which this instance has sent a request to.
        /// </summary>
        void ClearEndpoints();

        /// <summary>
        /// Cancels all pending requests on this instance.
        /// </summary>
        void CancelPendingRequests();
    }
}