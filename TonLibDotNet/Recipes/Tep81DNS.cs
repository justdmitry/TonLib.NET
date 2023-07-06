using TonLibDotNet.Cells;

namespace TonLibDotNet.Recipes
{
    /// <remarks>
    /// Based on <see href="https://github.com/ton-blockchain/TEPs/blob/master/text/0081-dns-standard.md">TEP 81: TON DNS Standard</see>.
    /// </remarks>
    public class Tep81DNS
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

        protected DnsEntries LoadDns(Slice slice)
        {
            var result = new DnsEntries();

            var entries = slice.TryLoadAndParseDict(256, s => s.LoadBitsToBytes(256), s => s);
            if (entries != null)
            {
                foreach (var entry in entries)
                {
                    var type = entry.Value.LoadUShort();
                    if (type == DnsEntryTypeWallet && CategoryBytesWallet.SequenceEqual(entry.Key))
                    {
                        result.Wallet = entry.Value.LoadAddressIntStd();
                    }
                    else if (type == DnsEntryTypeAdnl && CategoryBytesSite.SequenceEqual(entry.Key))
                    {
                        result.SiteToAdnl = TonUtils.Adnl.Encode(entry.Value.LoadBitsToBytes(256));
                    }
                    else if (type == DnsEntryTypeStorageBagId && CategoryBytesSite.SequenceEqual(entry.Key))
                    {
                        result.SiteToStorage = Convert.ToHexString(entry.Value.LoadBitsToBytes(256));
                    }
                    else if (type == DnsEntryTypeStorageBagId && CategoryBytesStorage.SequenceEqual(entry.Key))
                    {
                        result.Storage = Convert.ToHexString(entry.Value.LoadBitsToBytes(256));
                    }
                    else if (type == DnsEntryTypeNextDnsResolver && CategoryBytesNextDnsResolver.SequenceEqual(entry.Key))
                    {
                        result.DnsNextResolver = entry.Value.LoadAddressIntStd();
                    }
                }
            }

            return result;
        }

        public class DnsEntries
        {
            /// <summary>
            /// Domain 'wallet' entry value (address as EQ... string).
            /// </summary>
            public string? Wallet { get; set; }

            /// <summary>
            /// Domain 'site' entry value when set to ADNL address (as 55 characters string).
            /// </summary>
            public string? SiteToAdnl { get; set; }

            /// <summary>
            /// Domain 'site' entry value when set to Storage BagID value (in HEX).
            /// </summary>
            public string? SiteToStorage { get; set; }

            /// <summary>
            /// Domain 'storage' set to BagID value (in HEX).
            /// </summary>
            public string? Storage { get; set; }

            /// <summary>
            /// Domain 'dns_next_resolver' entry value (address as EQ... string).
            /// </summary>
            public string? DnsNextResolver { get; set; }
        }
    }
}
