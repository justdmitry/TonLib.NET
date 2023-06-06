using System.Diagnostics.CodeAnalysis;

namespace TonLibDotNet.Recipes
{
    /// <inheritdoc />
    public class TelegramNumbersRecipes : TelemintRecipes
    {
        public static readonly TelegramNumbersRecipes Instance = new();

        public override string CollectionAddress => "EQAOQdwdw8kGftJCSFgOErM1mBjYPe4DBPq8-AhF6vr9si5N";

        /// <inheritdoc />
        /// <remarks>
        /// <para>Valid numers starts with '888' (or '+888') and must contain up to 16 digits, and optional delimiters (<b>+</b>, <b>(</b>, <b>)</b> and <b>-</b>).</para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">Provided <paramref name="name"/> is null or empty or whitespace.</exception>
        public override bool TryNormalizeName(string name, [NotNullWhen(true)] out string normalizedName)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            normalizedName = string.Empty;

            const int maxLen = 16; // currently numbers contain 7 or 11 digits, but will add a bit for future
            var len = 0;
            Span<char> chars = stackalloc char[maxLen];

            foreach (var ch in name)
            {
                switch (ch)
                {
                    case '0' or '1' or '2' or '3' or '4' or '5' or '6' or '7' or '8' or '9':
                        if (len >= maxLen)
                        {
                            return false;
                        }

                        chars[len++] = ch;
                        continue;
                    case ' ' or '+' or '-' or '(' or ')':
                        continue;
                    default:
                        return false;
                }
            }

            if (!chars.StartsWith("888"))
            {
                return false;
            }

            normalizedName = chars[..len].ToString();
            return true;
        }

        /// <summary>
        /// ⚠ Original 'get_full_domain' method had been excluded from contract code.
        /// </summary>
        /// <remarks>
        /// Will always throw <see cref="NotImplementedException"/>.
        /// </remarks>
        /// <exception cref="NotImplementedException">Always.</exception>
        [Obsolete("Original 'get_full_domain' method had been excluded from contract code.")]
        public new Task<string> GetFullDomain(ITonClient tonClient, string nftAddress)
        {
            throw new NotImplementedException("Original 'get_full_domain' method had been excluded from contract code.");
        }
    }
}
