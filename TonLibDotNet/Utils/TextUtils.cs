using System.Diagnostics.CodeAnalysis;

namespace TonLibDotNet.Utils
{
    public class TextUtils
    {
        public static readonly TextUtils Instance = new ();

        /// <summary>
        /// Encodes string for transaction comment (which usually stored in Base64).
        /// </summary>
        /// <param name="source">Source string (to encode).</param>
        /// <returns>Encoded string.</returns>
        /// <remarks>Returns null or empty string when 'source' is null or empty string, respectively.</remarks>
        [return: NotNullIfNotNull("source")]
        public string? EncodeAsBase64(string? source)
        {
            if (string.IsNullOrEmpty(source))
            {
                return source;
            }

            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(source));
        }

        /// <summary>
        /// Tries to decode string from Base64 (transaction comments usually stored in Base64).
        /// </summary>
        /// <param name="source">Source string (to decode).</param>
        /// <param name="result">Decoded string.</param>
        /// <remarks>Returns <b>false</b> when 'source' is null, empty string or invalid/incomplete Base64 string.</remarks>
        public bool TryDecodeBase64(string? source, [NotNullWhen(true)] out string? result)
        {
            if (string.IsNullOrEmpty(source))
            {
                result = null;
                return false;
            }

            Span<byte> bytes = stackalloc byte[source.Length];
            if (!Convert.TryFromBase64String(source, bytes, out var count))
            {
                result = null;
                return false;
            }

            result = System.Text.Encoding.UTF8.GetString(bytes[..count]);
            return true;
        }
    }
}
