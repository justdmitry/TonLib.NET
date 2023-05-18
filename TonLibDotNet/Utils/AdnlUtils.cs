namespace TonLibDotNet.Utils
{
    public class AdnlUtils
    {
        public static readonly AdnlUtils Instance = new ();

        /// <summary>
        /// Encodes 256bit ADNL address into 55-characters string.
        /// </summary>
        /// <param name="adnl">ADNL address bytes.</param>
        /// <returns>String ADNL representation (55 characters).</returns>
        /// <exception cref="ArgumentOutOfRangeException">Source byte array length is not equal to 32.</exception>
        /// <seealso href="https://github.com/ton-blockchain/ton/blob/24dc184a2ea67f9c47042b4104bbb4d82289fac1/crypto/common/util.cpp#L195">Reference implementation</seealso>
        public string Encode(ReadOnlySpan<byte> adnl)
        {
            if (adnl.Length != 32)
            {
                throw new ArgumentOutOfRangeException(nameof(adnl), "Must be 32 bytes");
            }

            Span<byte> buf = stackalloc byte[35];
            buf[0] = 0x2d;
            adnl.CopyTo(buf[1..]);
            var crc = Crc16.Ccitt.ComputeChecksum(buf[..33]);
            buf[33] = (byte)((crc >> 8) & 0xFF);
            buf[34] = (byte)(crc & 0xFF);

            return Base32.Encode(buf).ToLowerInvariant()[1..];
        }

        /// <summary>
        /// Decodes ADNL address string into 32 bytes (256 bits).
        /// </summary>
        /// <param name="adnl">ADNL address as string (55 characters).</param>
        /// <returns>ADNL address as 32 bytes.</returns>
        /// <exception cref="ArgumentNullException">Input string is null or empty.</exception>
        /// <exception cref="ArgumentException">Input string length is not 55.</exception>
        public Span<byte> Decode(string adnl)
        {
            if (string.IsNullOrEmpty(adnl))
            {
                throw new ArgumentNullException(nameof(adnl));
            }

            if (adnl.Length != 55)
            {
                throw new ArgumentException("Must be 55 characters", nameof(adnl));
            }

            var bytes = Base32.Decode("F" + adnl);
            return bytes.AsSpan(1, 32);
        }
    }
}
