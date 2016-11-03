namespace Easy.Common.Interfaces
{
    using System;
    using System.Collections.Generic;
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
        /// Clears all of the endpoints which this instance has sent a request to.
        /// </summary>
        void ClearEndpoints();

        /// <summary>
        /// Cancels all pending requests on this instance.
        /// </summary>
        void CancelPendingRequests();
    }
}