using TonLibDotNet.Recipes;

namespace TonLibDotNet
{
    public static class TonRecipes
    {
        /// <summary>
        /// Functions to work with DNS contracts (*.ton domains).
        /// </summary>
        /// <remarks>
        /// Based on <see href="https://github.com/ton-blockchain/TEPs/blob/master/text/0081-dns-standard.md">TEP 81: TON DNS Standard</see>
        ///   and <see href="https://github.com/ton-blockchain/dns-contract">TON DNS Contracts</see>.
        /// </remarks>
        public static RootDnsRecipes RootDns { get; } = RootDnsRecipes.Instance;

        /// <summary>
        /// Functions to work with t.me contracts (*.t.me domains).
        /// </summary>
        /// <remarks>
        /// Based on <see href="https://github.com/ton-blockchain/TEPs/blob/master/text/0081-dns-standard.md">TEP 81: TON DNS Standard</see>
        ///   and <see href="https://github.com/TelegramMessenger/telemint">Telemint (Telegram username) Contracts</see>.
        /// </remarks>
        public static TelemintRecipes Telemint { get; } = TelemintRecipes.Instance;
    }
}
