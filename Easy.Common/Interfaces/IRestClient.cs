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
        /// Gets the base address of Uniform Resource Identifier (URI) of the Internet resource used when sending requests.
        /// </summary>
        Uri BaseAddress { get; }

        /// <summary>
        /// Sends the given <paramref name="request"/>.
        /// </summary>
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request);

        /// <summary>
        /// Sends the given <paramref name="request"/> with the given <paramref name="cToken"/>.
        /// </summary>
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cToken);

        /// <summary>
        /// Sends the given <paramref name="request"/> with the given <paramref name="option"/>.
        /// </summary>
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption option);

        /// <summary>
        /// Sends the given <paramref name="request"/> with the given <paramref name="option"/> and <paramref name="cToken"/>.
        /// </summary>
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption option, CancellationToken cToken);

        /// <summary>
        /// Sends a <c>PUT</c> request with the given <paramref name="content"/> to the specified <paramref name="uri"/>.
        /// </summary>
        Task<HttpResponseMessage> PutAsync(string uri, HttpContent content);

        /// <summary>
        /// Sends a <c>PUT</c> request with the given <paramref name="content"/> to the specified <paramref name="uri"/> with the given <paramref name="timeout"/>.
        /// </summary>
        Task<HttpResponseMessage> PutAsync(string uri, HttpContent content, TimeSpan timeout);

        /// <summary>
        /// Sends a <c>PUT</c> request with the given <paramref name="content"/> to the specified <paramref name="uri"/>.
        /// </summary>
        Task<HttpResponseMessage> PutAsync(Uri uri, HttpContent content);
        
        /// <summary>
        /// Sends a <c>PUT</c> request with the given <paramref name="content"/> to the specified <paramref name="uri"/> with the given <paramref name="timeout"/>.
        /// </summary>
        Task<HttpResponseMessage> PutAsync(Uri uri, HttpContent content, TimeSpan timeout);

        /// <summary>
        /// Sends a <c>PUT</c> request with the given <paramref name="content"/> to the specified <paramref name="uri"/> 
        /// with the given <paramref name="cToken"/>.
        /// </summary>
        Task<HttpResponseMessage> PutAsync(Uri uri, HttpContent content, CancellationToken cToken);

        /// <summary>
        /// Sends a <c>PUT</c> request with the given <paramref name="content"/> to the specified <paramref name="uri"/> 
        /// with the given <paramref name="cToken"/>.
        /// </summary>       
        Task<HttpResponseMessage> PutAsync(string uri, HttpContent content, CancellationToken cToken);

        /// <summary>
        /// Sends a <c>POST</c> request with the given <paramref name="content"/> to the specified <paramref name="uri"/>.
        /// </summary>
        Task<HttpResponseMessage> PostAsync(string uri, HttpContent content);

        /// <summary>
        /// Sends a <c>POST</c> request with the given <paramref name="content"/> to the specified <paramref name="uri"/> with the given <paramref name="timeout"/>.
        /// </summary>
        Task<HttpResponseMessage> PostAsync(string uri, HttpContent content, TimeSpan timeout);

        /// <summary>
        /// Sends a <c>POST</c> request with the given <paramref name="content"/> to the specified <paramref name="uri"/>.
        /// </summary>
        Task<HttpResponseMessage> PostAsync(Uri uri, HttpContent content);

        /// <summary>
        /// Sends a <c>POST</c> request with the given <paramref name="content"/> to the specified <paramref name="uri"/> with the given <paramref name="timeout"/>.
        /// </summary>
        Task<HttpResponseMessage> PostAsync(Uri uri, HttpContent content, TimeSpan timeout);

        /// <summary>
        /// Sends a <c>POST</c> request with the given <paramref name="content"/> to the specified <paramref name="uri"/> 
        /// with the given <paramref name="cToken"/>.
        /// </summary>
        Task<HttpResponseMessage> PostAsync(Uri uri, HttpContent content, CancellationToken cToken);

        /// <summary>
        /// Sends a <c>POST</c> request with the given <paramref name="content"/> to the specified <paramref name="uri"/> 
        /// with the given <paramref name="cToken"/>.
        /// </summary>     
        Task<HttpResponseMessage> PostAsync(string uri, HttpContent content, CancellationToken cToken);

        /// <summary>
        /// Sends a <c>GET</c> request to the specified <paramref name="uri"/>.
        /// </summary>
        Task<string> GetStringAsync(string uri);
        
        /// <summary>
        /// Sends a <c>GET</c> request to the specified <paramref name="uri"/> with the given <paramref name="timeout"/>.
        /// </summary>
        Task<string> GetStringAsync(string uri, TimeSpan timeout);

        /// <summary>
        /// Sends a <c>GET</c> request to the specified <paramref name="uri"/>.
        /// </summary>
        Task<string> GetStringAsync(Uri uri);

        /// <summary>
        /// Sends a <c>GET</c> request to the specified <paramref name="uri"/> with the given <paramref name="timeout"/>.
        /// </summary>
        Task<string> GetStringAsync(Uri uri, TimeSpan timeout);

        /// <summary>
        /// Sends a <c>GET</c> request to the specified <paramref name="uri"/>.
        /// </summary>
        Task<Stream> GetStreamAsync(string uri);

        /// <summary>
        /// Sends a <c>GET</c> request to the specified <paramref name="uri"/> with the given <paramref name="timeout"/>.
        /// </summary>
        Task<Stream> GetStreamAsync(string uri, TimeSpan timeout);

        /// <summary>
        /// Sends a <c>GET</c> request to the specified <paramref name="uri"/>.
        /// </summary>
        Task<Stream> GetStreamAsync(Uri uri);

        /// <summary>
        /// Sends a <c>GET</c> request to the specified <paramref name="uri"/> with the given <paramref name="timeout"/>.
        /// </summary>
        Task<Stream> GetStreamAsync(Uri uri, TimeSpan timeout);

        /// <summary>
        /// Sends a <c>GET</c> request to the specified <paramref name="uri"/>.
        /// </summary>
        Task<byte[]> GetByteArrayAsync(string uri);

        /// <summary>
        /// Sends a <c>GET</c> request to the specified <paramref name="uri"/> with the given <paramref name="timeout"/>.
        /// </summary>
        Task<byte[]> GetByteArrayAsync(string uri, TimeSpan timeout);

        
        /// <summary>
        /// Sends a <c>GET</c> request to the specified <paramref name="uri"/>.
        /// </summary>
        Task<byte[]> GetByteArrayAsync(Uri uri);

        /// <summary>
        /// Sends a <c>GET</c> request to the specified <paramref name="uri"/> with the given <paramref name="timeout"/>.
        /// </summary>
        Task<byte[]> GetByteArrayAsync(Uri uri, TimeSpan timeout);

        /// <summary>
        /// Sends a <c>GET</c> request to the specified <paramref name="uri"/>.
        /// </summary>
        Task<HttpResponseMessage> GetAsync(string uri);

        /// <summary>
        /// Sends a <c>GET</c> request to the specified <paramref name="uri"/> with the given <paramref name="timeout"/>.
        /// </summary>
        Task<HttpResponseMessage> GetAsync(string uri, TimeSpan timeout);

        /// <summary>
        /// Sends a <c>GET</c> request to the specified <paramref name="uri"/>.
        /// </summary>
        Task<HttpResponseMessage> GetAsync(Uri uri);

        /// <summary>
        /// Sends a <c>GET</c> request to the specified <paramref name="uri"/> with the given <paramref name="timeout"/>.
        /// </summary>
        Task<HttpResponseMessage> GetAsync(Uri uri, TimeSpan timeout);

        /// <summary>
        /// Sends a <c>GET</c> request to the specified <paramref name="uri"/> with the given <paramref name="cToken"/>.
        /// </summary>
        Task<HttpResponseMessage> GetAsync(string uri, CancellationToken cToken);

        /// <summary>
        /// Sends a <c>GET</c> request to the specified <paramref name="uri"/> with the given <paramref name="cToken"/>.
        /// </summary>
        Task<HttpResponseMessage> GetAsync(Uri uri, CancellationToken cToken);

        /// <summary>
        /// Sends a <c>GET</c> request to the specified <paramref name="uri"/> with the given <paramref name="option"/>.
        /// </summary>
        Task<HttpResponseMessage> GetAsync(string uri, HttpCompletionOption option);

        /// <summary>
        /// Sends a <c>GET</c> request to the specified <paramref name="uri"/> with the given <paramref name="option"/>.
        /// </summary>
        Task<HttpResponseMessage> GetAsync(Uri uri, HttpCompletionOption option);

        /// <summary>
        /// Sends a <c>GET</c> request to the specified <paramref name="uri"/> with the given <paramref name="option"/> and <paramref name="cToken"/>.
        /// </summary>
        Task<HttpResponseMessage> GetAsync(string uri, HttpCompletionOption option, CancellationToken cToken);

        /// <summary>
        /// Sends a <c>GET</c> request to the specified <paramref name="uri"/> with the given <paramref name="option"/> and <paramref name="cToken"/>.
        /// </summary>
        Task<HttpResponseMessage> GetAsync(Uri uri, HttpCompletionOption option, CancellationToken cToken);

        /// <summary>
        /// Sends a <c>DELETE</c> request to the specified <paramref name="uri"/>.
        /// </summary>
        Task<HttpResponseMessage> DeleteAsync(string uri);

        /// <summary>
        /// Sends a <c>DELETE</c> request to the specified <paramref name="uri"/> with the given <paramref name="timeout"/>.
        /// </summary>
        Task<HttpResponseMessage> DeleteAsync(string uri, TimeSpan timeout);

        /// <summary>
        /// Sends a <c>DELETE</c> request to the specified <paramref name="uri"/>.
        /// </summary>
        Task<HttpResponseMessage> DeleteAsync(Uri uri);

        /// <summary>
        /// Sends a <c>DELETE</c> request to the specified <paramref name="uri"/> with the given <paramref name="timeout"/>.
        /// </summary>
        Task<HttpResponseMessage> DeleteAsync(Uri uri, TimeSpan timeout);

        /// <summary>
        /// Sends a <c>DELETE</c> request to the specified <paramref name="uri"/> with the given <paramref name="cToken"/>.
        /// </summary>
        Task<HttpResponseMessage> DeleteAsync(string uri, CancellationToken cToken);

        /// <summary>
        /// Sends a <c>DELETE</c> request to the specified <paramref name="uri"/> with the given <paramref name="cToken"/>.
        /// </summary>
        Task<HttpResponseMessage> DeleteAsync(Uri uri, CancellationToken cToken);

        /// <summary>
        /// Cancels all pending requests on this instance.
        /// </summary>
        void CancelPendingRequests();
    }
}