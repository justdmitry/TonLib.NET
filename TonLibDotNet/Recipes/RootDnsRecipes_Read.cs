using System.Globalization;
using TonLibDotNet.Types.Dns;
using TonLibDotNet.Types.Smc;
using TonLibDotNet.Utils;

namespace TonLibDotNet.Recipes
{
    public partial class RootDnsRecipes
    {
        /// <summary>
        /// Resolves DNS name into DNS Item NFT address (for both existing/minted and not-minted-yet domains).
        /// </summary>
        /// <param name="tonClient"><see cref="ITonClient"/> instance.</param>
        /// <param name="domainName">Domain name to resolve. Second-level only, e.g. 'alice.ton'.</param>
        /// <returns>Address of DNS Item NFT for requested domain.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Requested domainName is not second-level one.</exception>
        public async Task<string> GetNftAddress(ITonClient tonClient, string domainName)
        {
            // Count dots in name, but not count last one.
            var depth = domainName.Count(x => x == '.') - (domainName.EndsWith('.') ? 1 : 0);
            if (depth != 1)
            {
                throw new ArgumentOutOfRangeException(nameof(domainName), "Only second-level domains (e.g. alice.ton) are supported");
            }

            await tonClient.InitIfNeeded();

            var resolved = await tonClient.DnsResolve(domainName, ttl: 1); // only resolve in root contract

            return ((EntryDataNextResolver)resolved.Entries[0].Value).Resolver.Value;
        }

        /// <summary>
        /// Executes 'get_editor' method on DNS Item NFT contract, returns owner of this domain (if any).
        /// </summary>
        /// <param name="tonClient"><see cref="ITonClient"/> instance.</param>
        /// <param name="nftAddress">Address of DNS Item NFT.</param>
        /// <returns>Editor (owner) address, or null (if this domain has no owner, for example when auction is in progress).</returns>
        /// <remarks>DNS Item contract must be deployed and active (to execute get-method).</remarks>
        /// <exception cref="TonLibNonZeroExitCodeException" />
        /// <seealso href="https://github.com/ton-blockchain/dns-contract/blob/main/func/nft-item.fc#L275">Source of 'get_editor' method.</seealso>
        public async Task<string?> GetEditor(ITonClient tonClient, string nftAddress)
        {
            await tonClient.InitIfNeeded().ConfigureAwait(false);

            var smc = await tonClient.SmcLoad(new Types.AccountAddress(nftAddress)).ConfigureAwait(false);

            // slice get_editor()
            var result = await tonClient.SmcRunGetMethod(smc.Id, new MethodIdName("get_editor")).ConfigureAwait(false);

            await tonClient.SmcForget(smc.Id).ConfigureAwait(false);

            if (result.ExitCode != 0)
            {
                throw new TonLibNonZeroExitCodeException(result.ExitCode);
            }

            return result.Stack[0].ToTvmCell().ToBoc().RootCells[0].BeginRead().TryLoadAddressIntStd();
        }

        /// <summary>
        /// Executes 'get_domain' method on DNS Item NFT contract, returns domain name.
        /// </summary>
        /// <param name="tonClient"><see cref="ITonClient"/> instance.</param>
        /// <param name="nftAddress">Address of DNS Item NFT.</param>
        /// <returns>Domain name stored in this contract, e.g. returns "alice" for "alice.ton" domain.</returns>
        /// <remarks>DNS Item contract must be deployed and active (to execute get-method).</remarks>
        /// <exception cref="TonLibNonZeroExitCodeException" />
        /// <seealso href="https://github.com/ton-blockchain/dns-contract/blob/main/func/nft-item.fc#L280">Source of 'get_domain' method.</seealso>
        public async Task<string> GetDomainName(ITonClient tonClient, string nftAddress)
        {
            await tonClient.InitIfNeeded().ConfigureAwait(false);

            var smc = await tonClient.SmcLoad(new Types.AccountAddress(nftAddress)).ConfigureAwait(false);

            // slice get_domain()
            var result = await tonClient.SmcRunGetMethod(smc.Id, new MethodIdName("get_domain")).ConfigureAwait(false);

            await tonClient.SmcForget(smc.Id).ConfigureAwait(false);

            if (result.ExitCode != 0)
            {
                throw new TonLibNonZeroExitCodeException(result.ExitCode);
            }

            return System.Text.Encoding.UTF8.GetString(result.Stack[0].ToTvmCell().ToBoc().RootCells[0].Content);
        }

        /// <summary>
        /// Executes 'get_auction_info' method on DNS Item NFT contract, returns auction data (max bid, bidder, end time).
        /// </summary>
        /// <param name="tonClient"><see cref="ITonClient"/> instance.</param>
        /// <param name="nftAddress">Address of DNS Item NFT.</param>
        /// <returns>Auction data (max bid, bidder, end time), or <b>null</b> if auction not yet started or already finished.</returns>
        /// <remarks>DNS Item contract must be deployed and active (to execute get-method).</remarks>
        /// <exception cref="TonLibNonZeroExitCodeException" />
        /// <seealso href="https://github.com/ton-blockchain/dns-contract/blob/main/func/nft-item.fc#L285">Source of 'get_auction_info' method.</seealso>
        public async Task<AuctionInfo?> GetAuctionInfo(ITonClient tonClient, string nftAddress)
        {
            await tonClient.InitIfNeeded();

            var smc = await tonClient.SmcLoad(new Types.AccountAddress(nftAddress)).ConfigureAwait(false);

            // (slice, int, int) get_auction_info()
            //   MsgAddressInt max_bid_address
            //   Coins max_bid_amount
            //   int auction_end_time
            var result = await tonClient.SmcRunGetMethod(smc.Id, new MethodIdName("get_auction_info")).ConfigureAwait(false);

            await tonClient.SmcForget(smc.Id).ConfigureAwait(false);

            if (result.ExitCode != 0)
            {
                throw new TonLibNonZeroExitCodeException(result.ExitCode);
            }

            var bidNano = long.Parse(result.Stack[1].ToTvmNumberDecimal(), CultureInfo.InvariantCulture);
            if (bidNano == 0)
            {
                return null;
            }

            var adr = result.Stack[0].ToTvmCell().ToBoc().RootCells[0].BeginRead().LoadAddressIntStd();
            var bid = TonUtils.Coins.FromNano(bidNano);
            var end = DateTimeOffset.FromUnixTimeSeconds(long.Parse(result.Stack[2].ToTvmNumberDecimal(), CultureInfo.InvariantCulture));

            return new AuctionInfo() { MaxBidAddress = adr, MaxBidAmount = bid, AuctionEndTime = end };
        }

        /// <summary>
        /// Executes 'get_last_fill_up_time' method on DNS Item NFT contract, returns last domain fill-up date-time.
        /// </summary>
        /// <param name="tonClient"><see cref="ITonClient"/> instance.</param>
        /// <param name="nftAddress">Address of DNS Item NFT.</param>
        /// <returns>Last fill-up time, as stored in contract. Domain will be release one year after this date.</returns>
        /// <remarks>DNS Item contract must be deployed and active (to execute get-method).</remarks>
        /// <exception cref="TonLibNonZeroExitCodeException" />
        /// <seealso href="https://github.com/ton-blockchain/dns-contract/blob/main/func/nft-item.fc#L290">Source of 'get_last_fill_up_time' method.</seealso>
        public async Task<DateTimeOffset> GetLastFillUpTime(ITonClient tonClient, string nftAddress)
        {
            await tonClient.InitIfNeeded().ConfigureAwait(false);

            var smc = await tonClient.SmcLoad(new Types.AccountAddress(nftAddress)).ConfigureAwait(false);

            // slice get_domain()
            var result = await tonClient.SmcRunGetMethod(smc.Id, new MethodIdName("get_last_fill_up_time")).ConfigureAwait(false);

            await tonClient.SmcForget(smc.Id).ConfigureAwait(false);

            if (result.ExitCode != 0)
            {
                throw new TonLibNonZeroExitCodeException(result.ExitCode);
            }

            return DateTimeOffset.FromUnixTimeSeconds(long.Parse(result.Stack[0].ToTvmNumberDecimal(), CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Calls 'dns.resolve' tonlib method against DNS Item NFT contract, returns records.
        /// </summary>
        /// <param name="tonClient"><see cref="ITonClient"/> instance.</param>
        /// <param name="nftAddress">Address of DNS Item NFT.</param>
        /// <returns>All records that stored in DNS Item NFT data.</returns>
        /// <remarks>DNS Item contract must be deployed and active.</remarks>
        /// <exception cref="TonLibNonZeroExitCodeException" />
        public async Task<DnsEntries> GetEntries(ITonClient tonClient, string nftAddress)
        {
            await tonClient.InitIfNeeded().ConfigureAwait(false);

            var result = await tonClient.DnsResolve(".", new Types.AccountAddress(nftAddress)).ConfigureAwait(false);

            var de = new DnsEntries();

            foreach(var item in result.Entries)
            {
                var category = Convert.FromBase64String(item.Category);

                if (item.Value is EntryDataSmcAddress wa && CategoryBytesWallet.SequenceEqual(category))
                {
                    de.Wallet = wa.SmcAddress.Value;
                }
                else if (item.Value is EntryDataAdnlAddress saa && CategoryBytesSite.SequenceEqual(category))
                {
                    de.SiteToAdnl = saa.AdnlAddress.Value;
                }
                else if (item.Value is EntryDataStorageAddress ssa && CategoryBytesSite.SequenceEqual(category))
                {
                    de.SiteToStorage = Convert.ToHexString(Convert.FromBase64String(ssa.BagId));
                }
                else if (item.Value is EntryDataStorageAddress sa && CategoryBytesStorage.SequenceEqual(category))
                {
                    de.Storage = Convert.ToHexString(Convert.FromBase64String(sa.BagId));
                }
                else if (item.Value is EntryDataNextResolver nra && CategoryBytesNextDnsResolver.SequenceEqual(category))
                {
                    de.DnsNextResolver = nra.Resolver.Value;
                }
            }

            return de;
        }

        /// <summary>
        /// Resolves *.ton domain to DNS Item NFT and parses data of this contract.
        /// </summary>
        /// <param name="tonClient"><see cref="ITonClient"/> instance.</param>
        /// <param name="domainName">Domain name to get data from. Second-level only, e.g. 'alice.ton'.</param>
        /// <returns><see cref="DomainInfo"/> with data about domain.</returns>
        /// <remarks>⚠ Method may fail if future versions of <see href="https://github.com/ton-blockchain/dns-contract/blob/main/func/nft-item.fc">DNS Item smartcontract</see> will change stored data layout.</remarks>
        /// <exception cref="ArgumentOutOfRangeException">Requested domainName is not second-level one.</exception>
        public async Task<DomainInfo> GetAllInfo(ITonClient tonClient, string domainName)
        {
            // Count dots in name, but not count last one.
            var depth = domainName.Count(x => x == '.') - (domainName.EndsWith('.') ? 1 : 0);
            if (depth != 1)
            {
                throw new ArgumentOutOfRangeException(nameof(domainName), "Only second-level domains (e.g. alice.ton) are supported");
            }

            await tonClient.InitIfNeeded().ConfigureAwait(false);

            var resolved = await tonClient.DnsResolve(domainName, ttl: 1); // only resolve in root contract

            var address = ((EntryDataNextResolver)resolved.Entries[0].Value).Resolver.Value;

            var di = new DomainInfo
            {
                Name = domainName.ToLowerInvariant(),
                Address = address,
                IsDeployed = false,
            };

            var smc = await tonClient.SmcLoad(address).ConfigureAwait(false);
            var data = await tonClient.SmcGetData(smc.Id).ConfigureAwait(false);

            if (string.IsNullOrEmpty(data.Bytes))
            {
                return di;
            }

            // structure is (https://github.com/ton-blockchain/dns-contract/blob/main/func/nft-item.fc#L30):
            // ;; uint256 index
            // ;; MsgAddressInt collection_address
            // ;; MsgAddressInt owner_address
            // ;; cell content
            // ;; cell domain -e.g contains "alice"(without ending \0) for "alice.ton" domain
            // ;; cell auction - auction info
            // ;; int last_fill_up_time
            var slice = data.ToBoc().RootCells[0].BeginRead();

            di.IsDeployed = true;

            di.Index = slice.LoadBitsToBytes(256);
            di.CollectionAddress = slice.LoadAddressIntStd();
            di.EditorAddress = slice.TryLoadAddressIntStd();

            var content = slice.LoadRef();
            var contentSlice = content.BeginRead();
            contentSlice.SkipBits(8);
            var entries = contentSlice.TryLoadAndParseDict(256, s => s.LoadBitsToBytes(256), s => s);
            if (entries != null)
            {
                foreach (var entry in entries)
                {
                    var type = entry.Value.LoadUShort();
                    if (type == DnsEntryTypeWallet && CategoryBytesWallet.SequenceEqual(entry.Key))
                    {
                        di.Entries.Wallet = entry.Value.LoadAddressIntStd();
                    }
                    else if (type == DnsEntryTypeAdnl && CategoryBytesSite.SequenceEqual(entry.Key))
                    {
                        di.Entries.SiteToAdnl = TonUtils.Adnl.Encode(entry.Value.LoadBitsToBytes(256));
                    }
                    else if (type == DnsEntryTypeStorageBagId && CategoryBytesSite.SequenceEqual(entry.Key))
                    {
                        di.Entries.SiteToStorage = Convert.ToHexString(entry.Value.LoadBitsToBytes(256));
                    }
                    else if (type == DnsEntryTypeStorageBagId && CategoryBytesStorage.SequenceEqual(entry.Key))
                    {
                        di.Entries.Storage = Convert.ToHexString(entry.Value.LoadBitsToBytes(256));
                    }
                    else if (type == DnsEntryTypeNextDnsResolver && CategoryBytesNextDnsResolver.SequenceEqual(entry.Key))
                    {
                        di.Entries.DnsNextResolver = entry.Value.LoadAddressIntStd();
                    }
                }
            }

            slice.LoadRef(); // skip domain

            var auc = slice.TryLoadDict();
            if (auc != null)
            {
                // Structure is (from https://github.com/ton-blockchain/dns-contract/blob/main/func/nft-item.fc#L12):
                // ;; MsgAddressInt max_bid_address
                // ;; Coins max_bid_amount
                // ;; int auction_end_time
                var aucSlice = auc.BeginRead();

                di.AuctionInfo = new AuctionInfo
                {
                    MaxBidAddress = aucSlice.LoadAddressIntStd(),
                    MaxBidAmount = TonUtils.Coins.FromNano(aucSlice.LoadCoins()),
                    AuctionEndTime = DateTimeOffset.FromUnixTimeSeconds(aucSlice.LoadLong())
                };

                aucSlice.EndRead();
            }

            di.LastFillUpTime = DateTimeOffset.FromUnixTimeSeconds(slice.LoadLong());

            slice.EndRead();

            return di;
        }

        public class AuctionInfo
        {
            /// <summary>
            /// Address if wallet who made the last (maximum) bid.
            /// </summary>
            public string MaxBidAddress { get; set; } = string.Empty;

            /// <summary>
            /// Last (maximum) bid.
            /// </summary>
            public decimal MaxBidAmount { get; set; }

            /// <summary>
            /// Time when auction ends.
            /// </summary>
            public DateTimeOffset AuctionEndTime { get; set; }
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

        public class DomainInfo
        {
            /// <summary>
            /// Domain name (e.g. 'alice.ton').
            /// </summary>
            public string Name { get; set; } = string.Empty;

            /// <summary>
            /// Address of DNS Item contract.
            /// </summary>
            public string Address { get; set; } = string.Empty;

            /// <summary>
            /// True if contract is already deployed.
            /// </summary>
            public bool IsDeployed { get; set; }

            /// <summary>
            /// Value of 'index' field for this contract.
            /// </summary>
            public byte[] Index { get; set; } = Array.Empty<byte>();

            /// <summary>
            /// Address of collection (top-level .ton domain contract).
            /// </summary>
            public string CollectionAddress { get; set; } = string.Empty;

            /// <summary>
            /// Address of editor (owner) of this domain, or null if domain is not yet owned (e.g. auction is in progress).
            /// </summary>
            public string? EditorAddress { get; set; } = string.Empty;

            /// <summary>
            /// Information about auction (if any).
            /// </summary>
            public AuctionInfo? AuctionInfo { get; set; }

            /// <summary>
            /// Last fill-up time of this domain.
            /// </summary>
            public DateTimeOffset LastFillUpTime { get; set; }

            /// <summary>
            /// DNS Entry records in this domain.
            /// </summary>
            public DnsEntries Entries { get; set; } = new();
        }
    }
}
