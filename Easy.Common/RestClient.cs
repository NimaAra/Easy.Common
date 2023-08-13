namespace Easy.Common;

using Easy.Common.Extensions;
using Easy.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

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
        
    static RestClient() => ConfigureServicePointManager();

    /// <summary>
    /// Creates an instance of the <see cref="RestClient"/>.
    /// </summary>
    public RestClient(
        IDictionary<string, IEnumerable<string>>? defaultRequestHeaders = default,
        HttpMessageHandler? handler = default,
        Uri? baseAddress = default,
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

    private static HttpMessageHandler GetHandler() =>
        new SocketsHttpHandler
        {
            // https://github.com/dotnet/corefx/issues/26895
            // https://github.com/dotnet/corefx/issues/26331
            // https://github.com/dotnet/corefx/pull/26839
            PooledConnectionLifetime = ConnectionLifeTime,
            PooledConnectionIdleTimeout = ConnectionLifeTime,
            MaxConnectionsPerServer = MAX_CONNECTION_PER_SERVER
        };

    /// <inheritdoc/>
    public IReadOnlyDictionary<string, string[]> DefaultRequestHeaders => 
        _client.DefaultRequestHeaders.ToDictionary(x => x.Key, x => x.Value.ToArray());

    /// <inheritdoc/>
    public TimeSpan Timeout => _client.Timeout;

    /// <inheritdoc/>
    public uint MaxResponseContentBufferSize => (uint)_client.MaxResponseContentBufferSize;

    /// <inheritdoc/>
    public Uri? BaseAddress => _client.BaseAddress;

    /// <inheritdoc/>
    public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request) => 
        SendAsync(request, HttpCompletionOption.ResponseContentRead, CancellationToken.None);

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException"/>
    public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cToken) => 
        SendAsync(request, HttpCompletionOption.ResponseContentRead, cToken);

    /// <inheritdoc/>
    public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption option) => 
        SendAsync(request, option, CancellationToken.None);

    /// <inheritdoc/>
    public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption option, CancellationToken cToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        return _client.SendAsync(request, option, cToken);
    }

    /// <inheritdoc/>
    public Task<HttpResponseMessage> GetAsync(string uri) => SendAsync(new HttpRequestMessage(HttpMethod.Get, uri));

    /// <inheritdoc/>
    public Task<HttpResponseMessage> GetAsync(string uri, TimeSpan timeout) => GetAsync(new Uri(uri, UriKind.RelativeOrAbsolute), timeout);

    /// <inheritdoc/>
    public Task<HttpResponseMessage> GetAsync(Uri uri) => SendAsync(new HttpRequestMessage(HttpMethod.Get, uri));

    /// <inheritdoc/>
    public async Task<HttpResponseMessage> GetAsync(Uri uri, TimeSpan timeout)
    {
        using CancellationTokenSource cts = new(timeout);
        return await SendAsync(new HttpRequestMessage(HttpMethod.Get, uri), cts.Token).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public Task<HttpResponseMessage> GetAsync(string uri, CancellationToken cToken) => SendAsync(new HttpRequestMessage(HttpMethod.Get, uri), cToken);

    /// <inheritdoc/>
    public Task<HttpResponseMessage> GetAsync(Uri uri, CancellationToken cToken) => SendAsync(new HttpRequestMessage(HttpMethod.Get, uri), cToken);

    /// <inheritdoc/>
    public Task<HttpResponseMessage> GetAsync(string uri, HttpCompletionOption option) => SendAsync(new HttpRequestMessage(HttpMethod.Get, uri), option);

    /// <inheritdoc/>
    public Task<HttpResponseMessage> GetAsync(Uri uri, HttpCompletionOption option) => SendAsync(new HttpRequestMessage(HttpMethod.Get, uri), option);

    /// <inheritdoc/>
    public Task<HttpResponseMessage> GetAsync(string uri, HttpCompletionOption option, CancellationToken cToken) => 
        SendAsync(new HttpRequestMessage(HttpMethod.Get, uri), option, cToken);

    /// <inheritdoc/>
    public Task<HttpResponseMessage> GetAsync(Uri uri, HttpCompletionOption option, CancellationToken cToken) => 
        SendAsync(new HttpRequestMessage(HttpMethod.Get, uri), option, cToken);

    /// <inheritdoc/>
    public Task<HttpResponseMessage> PutAsync(string uri, HttpContent content) => 
        SendAsync(new HttpRequestMessage(HttpMethod.Put, uri) { Content = content });

    /// <inheritdoc/>
    public Task<HttpResponseMessage> PutAsync(string uri, HttpContent content, TimeSpan timeout) => 
        PutAsync(new Uri(uri, UriKind.RelativeOrAbsolute), content, timeout);

    /// <inheritdoc/>
    public Task<HttpResponseMessage> PutAsync(Uri uri, HttpContent content) => 
        SendAsync(new HttpRequestMessage(HttpMethod.Put, uri) { Content = content });

    /// <inheritdoc/>
    public async Task<HttpResponseMessage> PutAsync(Uri uri, HttpContent content, TimeSpan timeout)
    {
        using CancellationTokenSource cts = new(timeout);
        return await SendAsync(new HttpRequestMessage(HttpMethod.Put, uri) { Content = content }, cts.Token).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public Task<HttpResponseMessage> PutAsync(Uri uri, HttpContent content, CancellationToken cToken) => 
        SendAsync(new HttpRequestMessage(HttpMethod.Put, uri) { Content = content }, cToken);

    /// <inheritdoc/>
    public Task<HttpResponseMessage> PutAsync(string uri, HttpContent content, CancellationToken cToken) => 
        SendAsync(new HttpRequestMessage(HttpMethod.Put, uri) { Content = content }, cToken);

    /// <inheritdoc/>
    public Task<HttpResponseMessage> PostAsync(string uri, HttpContent content) => 
        SendAsync(new HttpRequestMessage(HttpMethod.Post, uri) { Content = content });

    /// <inheritdoc/>
    public Task<HttpResponseMessage> PostAsync(string uri, HttpContent content, TimeSpan timeout) => 
        PostAsync(new Uri(uri, UriKind.RelativeOrAbsolute), content, timeout);

    /// <inheritdoc/>
    public Task<HttpResponseMessage> PostAsync(Uri uri, HttpContent content) => 
        SendAsync(new HttpRequestMessage(HttpMethod.Post, uri) { Content = content });

    /// <inheritdoc/>
    public async Task<HttpResponseMessage> PostAsync(Uri uri, HttpContent content, TimeSpan timeout)
    {
        using CancellationTokenSource cts = new(timeout);
        return await SendAsync(new HttpRequestMessage(HttpMethod.Post, uri) { Content = content }, cts.Token).ConfigureAwait(false);
    } 
        
    /// <inheritdoc/>
    public Task<HttpResponseMessage> PostAsync(Uri uri, HttpContent content, CancellationToken cToken) => 
        SendAsync(new HttpRequestMessage(HttpMethod.Post, uri) { Content = content }, cToken);

    /// <inheritdoc/>
    public Task<HttpResponseMessage> PostAsync(string uri, HttpContent content, CancellationToken cToken) => 
        SendAsync(new HttpRequestMessage(HttpMethod.Post, uri) { Content = content }, cToken);

    /// <inheritdoc/>
    public Task<HttpResponseMessage> DeleteAsync(string uri) => SendAsync(new HttpRequestMessage(HttpMethod.Delete, uri));

    /// <inheritdoc/>
    public Task<HttpResponseMessage> DeleteAsync(string uri, TimeSpan timeout) => DeleteAsync(new Uri(uri, UriKind.RelativeOrAbsolute), timeout);

    /// <inheritdoc/>
    public Task<HttpResponseMessage> DeleteAsync(Uri uri) => SendAsync(new HttpRequestMessage(HttpMethod.Delete, uri));

    /// <inheritdoc/>
    public async Task<HttpResponseMessage> DeleteAsync(Uri uri, TimeSpan timeout)
    {
        using CancellationTokenSource cts = new(timeout);
        return await SendAsync(new HttpRequestMessage(HttpMethod.Delete, uri), cts.Token).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public Task<HttpResponseMessage> DeleteAsync(string uri, CancellationToken cToken) => 
        SendAsync(new HttpRequestMessage(HttpMethod.Delete, uri), cToken);

    /// <inheritdoc/>
    public Task<HttpResponseMessage> DeleteAsync(Uri uri, CancellationToken cToken) => SendAsync(new HttpRequestMessage(HttpMethod.Delete, uri), cToken);

    /// <inheritdoc/>
    public Task<string> GetStringAsync(string uri)
    {
        Ensure.NotNullOrEmptyOrWhiteSpace(uri);
        return GetStringAsync(new Uri(uri, UriKind.RelativeOrAbsolute));
    }

    /// <inheritdoc/>
    public Task<string> GetStringAsync(string uri, TimeSpan timeout)
    {
        Ensure.NotNullOrEmptyOrWhiteSpace(uri);
        return GetStringAsync(new Uri(uri, UriKind.RelativeOrAbsolute), timeout);
    }

    /// <inheritdoc/>
    public Task<string> GetStringAsync(Uri uri)
    {
        ArgumentNullException.ThrowIfNull(uri);
        return _client.GetStringAsync(uri);
    }

    /// <summary>
    /// Sends a <c>GET</c> request to the specified <paramref name="uri"/> with the given <paramref name="timeout"/>.
    /// </summary>
    ///<exception cref="ArgumentNullException"/>
    public async Task<string> GetStringAsync(Uri uri, TimeSpan timeout)
    {
        ArgumentNullException.ThrowIfNull(uri);
        HttpResponseMessage resp = await GetAsync(uri, timeout).ConfigureAwait(false);
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public Task<Stream> GetStreamAsync(string uri)
    {
        Ensure.NotNullOrEmptyOrWhiteSpace(uri);
        return GetStreamAsync(new Uri(uri, UriKind.RelativeOrAbsolute));
    }

    /// <inheritdoc/>
    public Task<Stream> GetStreamAsync(string uri, TimeSpan timeout)
    {
        Ensure.NotNullOrEmptyOrWhiteSpace(uri);
        return GetStreamAsync(new Uri(uri, UriKind.RelativeOrAbsolute), timeout);
    }

    /// <inheritdoc/>
    public Task<Stream> GetStreamAsync(Uri uri)
    {
        ArgumentNullException.ThrowIfNull(uri);
        return _client.GetStreamAsync(uri);
    }

    /// <inheritdoc/>
    public async Task<Stream> GetStreamAsync(Uri uri, TimeSpan timeout)
    {
        ArgumentNullException.ThrowIfNull(uri);
        HttpResponseMessage resp = await GetAsync(uri, timeout).ConfigureAwait(false);
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public Task<byte[]> GetByteArrayAsync(string uri)
    {
        Ensure.NotNullOrEmptyOrWhiteSpace(uri);
        return GetByteArrayAsync(new Uri(uri, UriKind.RelativeOrAbsolute));
    }

    /// <inheritdoc/>
    public Task<byte[]> GetByteArrayAsync(string uri, TimeSpan timeout)
    {
        Ensure.NotNullOrEmptyOrWhiteSpace(uri);
        return GetByteArrayAsync(new Uri(uri, UriKind.RelativeOrAbsolute), timeout);
    }

    /// <inheritdoc/>
    public Task<byte[]> GetByteArrayAsync(Uri uri)
    {
        ArgumentNullException.ThrowIfNull(uri);
        return _client.GetByteArrayAsync(uri);
    }

    /// <inheritdoc/>
    public async Task<byte[]> GetByteArrayAsync(Uri uri, TimeSpan timeout)
    {
        ArgumentNullException.ThrowIfNull(uri);
        HttpResponseMessage resp = await GetAsync(uri, timeout).ConfigureAwait(false);
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public void CancelPendingRequests() => _client.CancelPendingRequests();

    /// <inheritdoc/>
    public void Dispose() => _client.Dispose();
        
    private static void ConfigureServicePointManager()
    {
        // Default is 2 minutes, see https://msdn.microsoft.com/en-us/library/system.net.servicepointmanager.dnsrefreshtimeout(v=vs.110).aspx
        ServicePointManager.DnsRefreshTimeout = (int)ConnectionLifeTime.TotalMilliseconds;

        // Increases the concurrent outbound connections
        ServicePointManager.DefaultConnectionLimit = MAX_CONNECTION_PER_SERVER;
    }

    private void AddBaseAddress(Uri? uri)
    {
        if (uri is null) { return; }

        _client.BaseAddress = uri;
    }

    private void AddDefaultHeaders(IEnumerable<KeyValuePair<string, IEnumerable<string>>>? headers)
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
}