namespace Easy.Common;

using Easy.Common.Extensions;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// A simple implementation of a telnet client.
/// </summary>
public sealed class TelnetClient
{
    private readonly TcpClient _client;

    private TimeSpan _loginTimeout = 100.Milliseconds();

    /// <summary>
    /// Creates an instance of the <see cref="TelnetClient"/>.
    /// </summary>
    public TelnetClient(string host, int port) => _client = new TcpClient(host, port);

    /// <summary>
    /// Creates an instance of the <see cref="TelnetClient"/>.
    /// </summary>
    public TelnetClient(IPEndPoint endpoint) => _client = new TcpClient(endpoint);

    /// <summary>
    /// Creates an instance of the <see cref="TelnetClient"/>.
    /// </summary>
    public TelnetClient(AddressFamily family) => _client = new TcpClient(family);

    /// <summary>
    /// Gets the flag indicating whether the client is connected.
    /// </summary>
    public bool IsConnected => _client.Connected;

    /// <summary>
    /// Performs a login.
    /// </summary>
    public Task<string> Login(string username, string password) => 
        Login(username, password, 100.Milliseconds());

    /// <summary>
    /// Performs a login.
    /// </summary>
    public async Task<string> Login(string username, string password, TimeSpan timeout)
    {
        TimeSpan oldTimeout = _loginTimeout;
        _loginTimeout = timeout;
        string? s = await Read().ConfigureAwait(false);
        if (s is null || !s.TrimEnd().EndsWith(":"))
        {
            throw new Exception("Failed to connect: no login prompt");
        }

        await WriteLine(username).ConfigureAwait(false);

        s += await Read().ConfigureAwait(false);
        if (!s.TrimEnd().EndsWith(":"))
        {
            throw new Exception("Failed to connect: no password prompt");
        }

        await WriteLine(password).ConfigureAwait(false);

        s += await Read().ConfigureAwait(false);
        _loginTimeout = oldTimeout;
        return s;
    }

    /// <summary>
    /// Writes the given <paramref name="command"/> followed by <c>\n</c>.
    /// </summary>
    public Task WriteLine(string command) => Write(command + "\n");

    /// <summary>
    /// Writes the given <paramref name="command"/>.
    /// </summary>
    public async Task Write(string command)
    {
        if (!_client.Connected) { return; }

        byte[] buffer = System.Text.Encoding.ASCII.GetBytes(command.Replace("\0xFF", "\0xFF\0xFF"));
        await _client.GetStream().WriteAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
    }

    /// <summary>
    /// Reads from the client.
    /// </summary>
    public async Task<string?> Read()
    {
        if (!_client.Connected) { return null; }

        StringBuilder sb = StringBuilderCache.Acquire();
        do
        {
            Parse(sb);
            await Task.Delay(_loginTimeout).ConfigureAwait(false);
        }
        while (_client.Available > 0);

        return StringBuilderCache.GetStringAndRelease(sb);
    }

    private void Parse(StringBuilder sb)
    {
        while (_client.Available > 0)
        {
            int input = _client.GetStream().ReadByte();
            switch (input)
            {
                case -1:
                    break;
                case (int)Verbs.IAC:
                    // interpret as command
                    int inputVerb = _client.GetStream().ReadByte();
                        
                    if (inputVerb == -1) { break; }
                        
                    switch (inputVerb)
                    {
                        case (int)Verbs.IAC:
                            //literal IAC = 255 escaped, so append char 255 to string
                            sb.Append(inputVerb);
                            break;
                        case (int)Verbs.DO:
                        case (int)Verbs.DONT:
                        case (int)Verbs.WILL:
                        case (int)Verbs.WONT:
                            // reply to all commands with "WONT", unless it is SGA (suppres go ahead)
                            int inputOption = _client.GetStream().ReadByte();
                                
                            if (inputOption == -1) { break; }
                                
                            _client.GetStream().WriteByte((byte)Verbs.IAC);
                            if (inputOption == (int) Options.SGA)
                            {
                                _client.GetStream().WriteByte(inputVerb == (int)Verbs.DO ? 
                                    (byte)Verbs.WILL : (byte)Verbs.DO);
                            } 
                            else
                            {
                                _client.GetStream().WriteByte(inputVerb == (int)Verbs.DO ? 
                                    (byte)Verbs.WONT : (byte)Verbs.DONT);
                            }

                            _client.GetStream().WriteByte((byte)inputOption);
                            break;
                    }
                    break;
                default:
                    sb.Append((char)input);
                    break;
            }
        }
    }

    private enum Verbs
    {
        WILL = 251,
        WONT = 252,
        DO = 253,
        DONT = 254,
        IAC = 255
    }

    private enum Options
    {
        SGA = 3
    }
}