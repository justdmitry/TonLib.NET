using System.Diagnostics.CodeAnalysis;

namespace TonLibDotNet.Recipes
{
    /// <inheritdoc />
    public class TelegramUsernamesRecipes : TelemintRecipes
    {
        public static readonly TelegramUsernamesRecipes Instance = new();

        public override string CollectionAddress => "EQCA14o1-VWhS2efqoh_9M1b_A9DtKTuoqfmkn83AbJzwnPi";

        /// <inheritdoc />
        /// <remarks>
        /// <para>Only second-level .t.me domains are allowed, with or without '.t.me' suffix <br/>
        /// <i>(e.g. 'alice.t.me.', 'alice.t.me' and 'alice' are allowed, but 'alice.ton' or 'this.is.alice.t.me' are not)</i>.</para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">Provided <paramref name="name"/> is null or empty or whitespace.</exception>
        public override bool TryNormalizeName(string name, [NotNullWhen(true)] out string normalizedName)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            normalizedName = string.Empty;

            var parts = name.Trim().ToLowerInvariant().Split('.', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 2 || parts.Length > 3)
            {
                return false;
            }

            if (parts.Length == 3
                && !string.Equals(parts[1], "t", StringComparison.InvariantCulture)
                && !string.Equals(parts[2], "me", StringComparison.InvariantCulture))
            {
                return false;
            }

            normalizedName = parts[0];
            return true;
        }
    }
}
