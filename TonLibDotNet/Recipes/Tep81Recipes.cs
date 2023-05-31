using TonLibDotNet.Cells;

namespace TonLibDotNet.Recipes
{
    /// <remarks>
    /// Based on <see href="https://github.com/ton-blockchain/TEPs/blob/master/text/0081-dns-standard.md">TEP 81: TON DNS Standard</see>.
    /// </remarks>
    public class Tep81Recipes
    {
        /// <remarks>
        /// TL-B: <code>dns_smc_address#9fd3 smc_addr:MsgAddressInt flags:(## 8) { flags &lt;= 1 } cap_list:flags . 0?SmcCapList = DNSRecord</code>
        /// </remarks>
        protected const ushort DnsEntryTypeWallet = 0x9fd3;

        /// <remarks>
        /// TL-B: <code>dns_next_resolver#ba93 resolver:MsgAddressInt = DNSRecord</code>
        /// </remarks>
        protected const ushort DnsEntryTypeNextDnsResolver = 0xba93;

        /// <remarks>
        /// TL-B: <code>dns_adnl_address#ad01 adnl_addr:bits256 flags:(## 8) { flags &lt;= 1 } proto_list:flags . 0?ProtoList = DNSRecord</code>
        /// </remarks>
        protected const ushort DnsEntryTypeAdnl = 0xad01;

        /// <remarks>
        /// TL-B: <code>dns_storage_address#7473 bag_id:bits256 = DNSRecord</code>
        /// </remarks>
        protected const ushort DnsEntryTypeStorageBagId = 0x7473;

        protected const string CategoryNameSite = "site";
        protected const string CategoryNameWallet = "wallet";
        protected const string CategoryNameStorage = "storage";
        protected const string CategoryNameNextDnsResolver = "dns_next_resolver";

        protected static readonly byte[] CategoryBytesSite = EncodeCategory(CategoryNameSite);
        protected static readonly byte[] CategoryBytesWallet = EncodeCategory(CategoryNameWallet);
        protected static readonly byte[] CategoryBytesStorage = EncodeCategory(CategoryNameStorage);
        protected static readonly byte[] CategoryBytesNextDnsResolver = EncodeCategory(CategoryNameNextDnsResolver);

        protected static byte[] EncodeCategory(string categoryName)
        {
            return System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.ASCII.GetBytes(categoryName));
        }

        protected CellBuilder StoreWallet(CellBuilder cellBuilder, string walletAddress)
        {
            return cellBuilder
                .StoreUShort(DnsEntryTypeWallet)
                .StoreAddressIntStd(walletAddress)
                .StoreByte(0); // flags
        }

        protected CellBuilder StoreNextResolver(CellBuilder cellBuilder, string nextResolverAddress)
        {
            return cellBuilder
                .StoreUShort(DnsEntryTypeNextDnsResolver)
                .StoreAddressIntStd(nextResolverAddress);
        }

        protected CellBuilder StoreAdnl(CellBuilder cellBuilder, ReadOnlySpan<byte> adnl)
        {
            if (adnl.Length != 32)
            {
                throw new ArgumentOutOfRangeException(nameof(adnl), "ADNL address must be 32 bytes long");
            }

            return cellBuilder
                .StoreUShort(DnsEntryTypeAdnl)
                .StoreBytes(adnl)
                .StoreByte(0);
        }

        protected CellBuilder StoreBagId(CellBuilder cellBuilder, ReadOnlySpan<byte> bagId)
        {
            if (bagId.Length != 32)
            {
                throw new ArgumentOutOfRangeException(nameof(bagId), "BagId must be 32 bytes long");
            }

            return cellBuilder
                .StoreUShort(DnsEntryTypeStorageBagId)
                .StoreBytes(bagId);
        }
    }
}
