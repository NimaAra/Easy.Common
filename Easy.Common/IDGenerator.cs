namespace Easy.Common;

using System;
using System.Threading;

/// <summary>
/// Inspired by <see href="https://github.com/aspnet/KestrelHttpServer/blob/6fde01a825cffc09998d3f8a49464f7fbe40f9c4/src/Kestrel.Core/Internal/Infrastructure/CorrelationIdGenerator.cs"/>,
/// this class generates an efficient 20-bytes ID which is the concatenation of a <c>base36</c> encoded
/// machine name and <c>base32</c> encoded <see cref="long"/> using the alphabet <c>0-9</c> and <c>A-V</c>.
/// </summary>
public sealed class IDGenerator
{
    private const string Encode_32_Chars = "0123456789ABCDEFGHIJKLMNOPQRSTUV";
    private static readonly char[] _prefix = new char[6];
    private static long _lastId = DateTime.UtcNow.Ticks;

    private static readonly ThreadLocal<char[]> _charBufferThreadLocal =
        new(() =>
        {
            char[] buffer = new char[20];
            buffer[0] = _prefix[0];
            buffer[1] = _prefix[1];
            buffer[2] = _prefix[2];
            buffer[3] = _prefix[3];
            buffer[4] = _prefix[4];
            buffer[5] = _prefix[5];
            buffer[6] = '-';
            return buffer;
        });

    static IDGenerator() => PopulatePrefix();
    private IDGenerator() { }

    /// <summary>
    /// Returns a single instance of the <see cref="IDGenerator"/>.
    /// </summary>
    public static IDGenerator Instance { get; } = new IDGenerator();

    /// <summary>
    /// Returns an ID. e.g: <c>XOGLN1-0HLHI1F5INOFA</c>
    /// </summary>
    public string Next => GenerateImpl(Interlocked.Increment(ref _lastId));

    private static string GenerateImpl(long id)
    {
        char[] buffer = _charBufferThreadLocal.Value!;

        buffer[7] = Encode_32_Chars[(int)(id >> 60) & 31];
        buffer[8] = Encode_32_Chars[(int)(id >> 55) & 31];
        buffer[9] = Encode_32_Chars[(int)(id >> 50) & 31];
        buffer[10] = Encode_32_Chars[(int)(id >> 45) & 31];
        buffer[11] = Encode_32_Chars[(int)(id >> 40) & 31];
        buffer[12] = Encode_32_Chars[(int)(id >> 35) & 31];
        buffer[13] = Encode_32_Chars[(int)(id >> 30) & 31];
        buffer[14] = Encode_32_Chars[(int)(id >> 25) & 31];
        buffer[15] = Encode_32_Chars[(int)(id >> 20) & 31];
        buffer[16] = Encode_32_Chars[(int)(id >> 15) & 31];
        buffer[17] = Encode_32_Chars[(int)(id >> 10) & 31];
        buffer[18] = Encode_32_Chars[(int)(id >> 5) & 31];
        buffer[19] = Encode_32_Chars[(int)id & 31];

        return new string(buffer, 0, buffer.Length);
    }

    private static void PopulatePrefix()
    {
        int machineHash = Math.Abs(Environment.MachineName.GetHashCode());
        string machineEncoded = Base36.Encode(machineHash);

        int i = _prefix.Length - 1;
        int j = 0;
        while (i >= 0)
        {
            if (j < machineEncoded.Length)
            {
                _prefix[i] = machineEncoded[j];
                j++;
            }
            else
            {
                _prefix[i] = '0';
            }
            i--;
        }
    }
}