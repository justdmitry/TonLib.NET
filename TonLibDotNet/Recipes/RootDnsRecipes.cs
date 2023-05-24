namespace TonLibDotNet.Recipes
{
    /// <remarks>
    /// Based on <see href="https://github.com/ton-blockchain/TEPs/blob/master/text/0081-dns-standard.md">TEP 81: TON DNS Standard</see>
    ///   and <see href="https://github.com/ton-blockchain/dns-contract">TON DNS Smart Contracts</see>.
    /// </remarks>
    public partial class RootDnsRecipes
    {
        /// <remarks>
        /// TL-B: <code>dns_smc_address#9fd3 smc_addr:MsgAddressInt flags:(## 8) { flags &lt;= 1 } cap_list:flags . 0?SmcCapList = DNSRecord</code>
        /// </remarks>
        private const ushort DnsEntryTypeWallet = 0x9fd3;

        /// <remarks>
        /// TL-B: <code>dns_next_resolver#ba93 resolver:MsgAddressInt = DNSRecord</code>
        /// </remarks>
        private const ushort DnsEntryTypeNextDnsResolver = 0xba93;

        /// <remarks>
        /// TL-B: <code>dns_adnl_address#ad01 adnl_addr:bits256 flags:(## 8) { flags &lt;= 1 } proto_list:flags . 0?ProtoList = DNSRecord</code>
        /// </remarks>
        private const ushort DnsEntryTypeAdnl = 0xad01;

        /// <remarks>
        /// TL-B: <code>dns_storage_address#7473 bag_id:bits256 = DNSRecord</code>
        /// </remarks>
        private const ushort DnsEntryTypeStorageBagId = 0x7473;

        private const string CategoryNameSite = "site";
        private const string CategoryNameWallet = "wallet";
        private const string CategoryNameStorage = "storage";
        private const string CategoryNameNextDnsResolver = "dns_next_resolver";

        private static readonly byte[] CategoryBytesSite = EncodeCategory(CategoryNameSite);
        private static readonly byte[] CategoryBytesWallet = EncodeCategory(CategoryNameWallet);
        private static readonly byte[] CategoryBytesStorage = EncodeCategory(CategoryNameStorage);
        private static readonly byte[] CategoryBytesNextDnsResolver = EncodeCategory(CategoryNameNextDnsResolver);

        /// <summary>
        /// From <see href="https://github.com/ton-blockchain/dns-contract/blob/main/func/dns-utils.fc">dns-utils.fc</see>
        /// </summary>
        private const int OPChangeDnsRecord = 0x4eb1f0f9;

        public static readonly RootDnsRecipes Instance = new();

        protected static byte[] EncodeCategory(string categoryName)
        {
            return System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.ASCII.GetBytes(categoryName));
        }
    }
}
