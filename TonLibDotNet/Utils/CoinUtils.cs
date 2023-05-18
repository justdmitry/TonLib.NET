namespace TonLibDotNet.Utils
{
    public class CoinUtils
    {
        public static readonly CoinUtils Instance = new ();

        /// <summary>
        /// Converts nano-TON amount to decimal TON (moves decimal point to 9 positions to the left).
        /// </summary>
        /// <remarks>It's safe to store nano-TON as 'long' while total TON supply is unchanged (5*10^18), details are in <see href="https://t.me/tondev/122940"/>.</remarks>
        public decimal FromNano(long nano)
        {
            // Last division is to get rid of trailing zeroes, see https://stackoverflow.com/questions/4525854/remove-trailing-zeros
            return nano * 0.000_000_001M / 1.000000000000000000000000000000000m;
        }

        /// <summary>
        /// Converts decimal TON amount to nano-TONs (moves decimal point to 9 positions to the right).
        /// </summary>
        /// <remarks>It's safe to store nano-TON as 'long' while total TON supply is unchanged (5*10^18), details are in <see href="https://t.me/tondev/122940"/>.</remarks>
        public long ToNano(decimal ton)
        {
            return Convert.ToInt64(ton * 1_000_000_000M);
        }
    }
}
