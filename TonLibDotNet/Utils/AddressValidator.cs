using System.Diagnostics.CodeAnalysis;

namespace TonLibDotNet.Utils
{
    /// <seealso href="https://docs.ton.org/learn/overviews/addresses#user-friendly-address">User-Friendly Address Structure</seealso>
    public static class AddressValidator
    {
        public const byte FlagBounceable = 0x11;
        public const byte FlagNonBounceable = 0x51;
        public const byte FlagTestnetOnly = 0x80;

        public const byte BasechainId = 0x00;
        public const byte MasterchainId = 0xFF;

        public static string MakeAddress(byte workchainId, byte[] accountId, bool bounceable = true, bool testnetOnly = false, bool urlSafe = true)
        {
            ArgumentNullException.ThrowIfNull(accountId);

            if (accountId.Length != 32)
            {
                throw new ArgumentException("Must be 32 bytes", nameof(accountId));
            }

            var bytes = new byte[36];

            bytes[0] = bounceable ? FlagBounceable : FlagNonBounceable;

            if (testnetOnly)
            {
                bytes[0] |= FlagTestnetOnly;
            }

            bytes[1] = workchainId;

            accountId.CopyTo(bytes, 2);

            var span = bytes.AsSpan();
            var crc = Crc16.Ccitt.ComputeChecksum(span[..^2]);
            System.Buffers.Binary.BinaryPrimitives.WriteUInt16BigEndian(span[^2..], crc);

            var address = Convert.ToBase64String(bytes);

            if (!urlSafe)
            {
                return address;
            }

            // use '_' and '-' instead of '/' and '+'
            var chars = address.ToCharArray();
            for (var i = 0; i < chars.Length; i++)
            {
                if (chars[i] == '/')
                {
                    chars[i] = '_';
                }
                else if (chars[i] == '+')
                {
                    chars[i] = '-';
                }
            }

            return new string(chars);
        }

        public static bool TryParseAddress(
            string address,
            out byte workchainId,
            [NotNullWhen(true)] out byte[]? accountId,
            out bool bounceable,
            out bool testnetOnly,
            out bool urlSafe)
        {
            if (string.IsNullOrEmpty(address))
            {
                throw new ArgumentNullException(nameof(address));
            }

            workchainId = default;
            accountId = default;
            bounceable = default;
            testnetOnly = default;
            urlSafe = default;

            if (address.Length != 48)
            {
                return false;
            }

            var chars = address.ToCharArray();
            var safeFound = false;
            var unsafeFound = false;
            for (var i = 0; i < chars.Length; i++)
            {
                switch (chars[i])
                {
                    case '/':
                    case '+':
                        unsafeFound = true;
                        break;

                    case '_':
                        safeFound = true;
                        chars[i] = '/';
                        break;

                    case '-':
                        safeFound = true;
                        chars[i] = '+';
                        break;
                }
            }

            if (safeFound && unsafeFound)
            {
                return false;
            }

            urlSafe = !unsafeFound;

            var bytes = new byte[36];

            if (!Convert.TryFromBase64Chars(chars, bytes, out _))
            {
                return false;
            }

            testnetOnly = (bytes[0] & FlagTestnetOnly) == FlagTestnetOnly;

            bounceable = (bytes[0] & FlagNonBounceable) == FlagBounceable;

            workchainId = bytes[1];
            accountId = bytes[2..^2].ToArray();

            var checksum = Crc16.Ccitt.ComputeChecksum(bytes);
            if (checksum != 0)
            {
                return false;
            }

            return true;
        }
    }
}
