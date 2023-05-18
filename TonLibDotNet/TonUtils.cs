using System.Diagnostics.CodeAnalysis;
using TonLibDotNet.Utils;

namespace TonLibDotNet
{
    public static class TonUtils
    {
        /// <summary>
        /// Useful function to work with coin amounts.
        /// </summary>
        public static CoinUtils Coins { get; } = CoinUtils.Instance;

        /// <summary>
        /// Useful function to work with addresses.
        /// </summary>
        public static AddressUtils Address { get; } = AddressUtils.Instance;

        /// <summary>
        /// Useful function to work with text strings.
        /// </summary>
        public static TextUtils Text { get; } = TextUtils.Instance;

        /// <summary>
        /// Useful function to work with ADNL addresses.
        /// </summary>
        public static AdnlUtils Adnl { get; } = AdnlUtils.Instance;
    }
}
