namespace TonLibDotNet.Utils
{
    public  class KeyUtils
    {
        public static readonly KeyUtils Instance = new();

        /// <summary>
        /// Extracts 32 bytes of public key from TonLib Ed25519 public key string (48 chars).
        /// </summary>
        /// <param name="value">String representation of Ed25519 public key (48 chars).</param>
        /// <returns>32 bytes of public key</returns>
        /// <remarks>
        /// Based on <see href="https://github.com/ton-blockchain/ton/blob/master/crypto/block/block.cpp#L44-L66"/>.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">When <paramref name="value"/> is not a valid public key string.</exception>
        public byte[] ParseEd25519PublicKey(string value)
        {
            if (value == null || value.Length != 48)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Serialized Ed25519 public key must be exactly 48 characters long");
            }

            // Decode from both base64 and base64url
            var bytes = Convert.FromBase64String(value.Replace('_', '/').Replace('-', '+'));

            if (bytes[0] != 0x3E)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Not a public key");
            }

            if (bytes[1] != 0xE6)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Not an ed25519 public key");
            }

            var crc1 = Crc16.Ccitt.ComputeChecksum(bytes.AsSpan(0, 34));
            var crc2 = (bytes[34] << 8) + bytes[35];
            if (crc1 != crc2)
            {
                throw new ArgumentOutOfRangeException(nameof(value), $"Public key has incorrect crc16 hash: {crc1:X} vs {crc2:X}");
            }

            var result = new byte[32];

            Array.Copy(bytes, 2, result, 0, 32);

            return result;
        }
    }
}
