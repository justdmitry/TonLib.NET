namespace TonLibDotNet.Recipes
{
    /// <remarks>
    /// Based on <see href="https://github.com/ton-blockchain/TEPs/blob/master/text/0081-dns-standard.md">TEP 81: TON DNS Standard</see>
    ///   and <see href="https://github.com/ton-blockchain/dns-contract">TON DNS Contracts</see>.
    /// </remarks>
    public partial class RootDnsRecipes : Tep81Recipes
    {
        /// <summary>
        /// .ton Root Collection - parent for all .TON DNS NFTs.
        /// </summary>
        public const string TonRootCollection = "EQC3dNlesgVD8YbAazcauIrXBPfiVhMMr5YYk2in0Mtsz0Bz";

        /// <summary>
        /// From <see href="https://github.com/ton-blockchain/dns-contract/blob/main/func/dns-utils.fc">dns-utils.fc</see>
        /// </summary>
        private const int OPChangeDnsRecord = 0x4eb1f0f9;

        public static readonly RootDnsRecipes Instance = new();
    }
}
