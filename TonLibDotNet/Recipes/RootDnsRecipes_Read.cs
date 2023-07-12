using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Xml.Linq;
using TonLibDotNet.Types.Dns;
using TonLibDotNet.Types.Smc;
using TonLibDotNet.Types.Tvm;
using TonLibDotNet.Utils;

namespace TonLibDotNet.Recipes
{
    public partial class RootDnsRecipes
    {
        /// <summary>
        /// Tries to normalize domain name to form used by Collection.
        /// </summary>
        /// <param name="domainName">Domain name to normalize.</param>
        /// <param name="normalizedName">Normalized domain name, if possible.</param>
        /// <returns>Returns <b>true</b> when name had been successfully normalized, and <b>false</b> otherwise.</returns>
        /// <remarks>
        /// <para>Only second-level .ton domains are "valid", with or without '.ton' suffix <br/>
        /// <i>(e.g. 'alice.ton.', 'alice.ton' and 'alice' are allowed, but 'alice.t.me' or 'thisis.alice.ton' is not)</i>.</para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">Requested <paramref name="domainName"/> null or empty.</exception>
        public bool TryNormalizeName(string domainName, [NotNullWhen(true)] out string normalizedName)
        {
            if (string.IsNullOrEmpty(domainName))
            {
                throw new ArgumentNullException(nameof(domainName));
            }

            normalizedName = string.Empty;

            var parts = domainName.Trim().ToLowerInvariant().Split('.', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length > 2)
            {
                return false;
            }

            if (parts.Length == 2 && !string.Equals(parts[1], "ton", StringComparison.InvariantCulture))
            {
                return false;
            }

            normalizedName = parts[0];
            return true;
        }

        /// <summary>
        /// Computes 'index' value for NFT with specified name.
        /// </summary>
        /// <param name="domainName">Domain name to resolve, with or without '.ton' suffix.</param>
        /// <returns><b>byte[8]</b> with required value (<i>cell_hash(domain)</i>, according to <see href="https://github.com/ton-blockchain/dns-contract/blob/main/func/nft-collection.fc#L133">contract source</see>).</returns>
        /// <remarks>
        /// <para>Check overriden <see cref="TryNormalizeName">TryNormalizeName</see> docs for 'valid' vs 'invalid' names description.</para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">Requested <paramref name="domainName"/> null or empty.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Requested <paramref name="domainName"/> is too long.</exception>
        public byte[] GetNftIndex(string domainName)
        {
            if (!TryNormalizeName(domainName, out var normalizedName))
            {
                throw new ArgumentOutOfRangeException(nameof(domainName), "Invalid domain name.");
            }

            // UTF-8 encoded string up to 126 bytes, https://github.com/ton-blockchain/TEPs/blob/master/text/0081-dns-standard.md#domain-names
            var bytes = System.Text.Encoding.UTF8.GetBytes(normalizedName);
            if (bytes.Length > 126)
            {
                throw new ArgumentOutOfRangeException(nameof(domainName), "Value is too long. No more than 126 chars are allowed.");
            }

            Span<byte> hashSource = stackalloc byte[bytes.Length + 2];
            hashSource[0] = 0;
            hashSource[1] = (byte)(bytes.Length * 2);
            bytes.CopyTo(hashSource[2..]);

            return System.Security.Cryptography.SHA256.HashData(hashSource);
        }

        /// <summary>
        /// Resolves DNS name into DNS Item NFT address (for both existing/minted and not-minted-yet domains) by calling 'get_nft_address_by_index' method.
        /// </summary>
        /// <param name="tonClient"><see cref="ITonClient"/> instance.</param>
        /// <param name="domainName">Domain name to resolve, with or without '.ton' suffix.</param>
        /// <returns>Address of DNS Item NFT for requested domain.</returns>
        /// <remarks>
        /// <para>Only second-level .ton domains are allowed, with or without '.ton' suffix <br/>
        /// <i>(e.g. 'alice.ton.', 'alice.ton' and 'alice' are allowed, but 'alice.t.me' or 'thisis.alice.ton' is not)</i>.</para>
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">Requested <paramref name="domainName"/> is not second-level one, or not from '.ton' namespace.</exception>
        /// <exception cref="ArgumentNullException">Requested <paramref name="domainName"/> null or white-space only.</exception>
        public async Task<string> GetNftAddress(ITonClient tonClient, string domainName)
        {
            if (string.IsNullOrWhiteSpace(domainName))
            {
                throw new ArgumentNullException(nameof(domainName));
            }

            var parts = domainName.ToLowerInvariant().Split('.', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length > 2)
            {
                throw new ArgumentOutOfRangeException(nameof(domainName), "Only second-level domains (e.g. 'alice.ton') are supported.");
            }

            if (parts.Length == 2 && !string.Equals(parts[1], "ton", StringComparison.InvariantCulture))
            {
                throw new ArgumentOutOfRangeException(nameof(domainName), "Only '.ton' domains (e.g. 'alice.ton') are supported.");
            }

            // UTF-8 encoded string up to 126 bytes, https://github.com/ton-blockchain/TEPs/blob/master/text/0081-dns-standard.md#domain-names
            var bytes = System.Text.Encoding.UTF8.GetBytes(parts[0]);
            if (bytes.Length > 126)
            {
                throw new ArgumentOutOfRangeException(nameof(domainName), "Value is too long. No more than 126 chars are allowed.");
            }

            var hashSource = new byte[bytes.Length + 2];
            hashSource[0] = 0;
            hashSource[1] = (byte)(bytes.Length * 2);
            bytes.CopyTo(hashSource, 2);
            var index = System.Security.Cryptography.SHA256.HashData(hashSource);

            await tonClient.InitIfNeeded().ConfigureAwait(false);

            var smc = await tonClient.SmcLoad(new Types.AccountAddress(TonRootCollection)).ConfigureAwait(false);

            // slice get_nft_address_by_index(int index)
            var stack = new List<StackEntry>()
            {
                new StackEntryNumber(new NumberDecimal(new BigInteger(index, true, true).ToString(CultureInfo.InvariantCulture))),
            };
            var result = await tonClient.SmcRunGetMethod(smc.Id, new MethodIdName("get_nft_address_by_index"), stack).ConfigureAwait(false);

            await tonClient.SmcForget(smc.Id).ConfigureAwait(false);

            if (result.ExitCode != 0)
            {
                throw new TonLibNonZeroExitCodeException(result.ExitCode);
            }

            return result.Stack[0].ToTvmCell().ToBoc().RootCells[0].BeginRead().LoadAddressIntStd();
        }

        /// <summary>
        /// Executes 'get_editor' method on DNS Item NFT contract, returns owner of this domain (if any).
        /// </summary>
        /// <param name="tonClient"><see cref="ITonClient"/> instance.</param>
        /// <param name="nftAddress">Address of DNS Item NFT.</param>
        /// <returns>Editor (owner) address, or null (if this domain has no owner, for example when auction is in progress).</returns>
        /// <remarks>
        /// <para>DNS Item contract must be deployed and active (to execute get-method).</para>
        /// </remarks>
        /// <exception cref="TonLibNonZeroExitCodeException" />
        /// <seealso href="https://github.com/ton-blockchain/dns-contract/blob/main/func/nft-item.fc#L275">Source of 'get_editor' method.</seealso>
        public async Task<string?> GetEditor(ITonClient tonClient, string nftAddress)
        {
            await tonClient.InitIfNeeded().ConfigureAwait(false);

            var smc = await tonClient.SmcLoad(new Types.AccountAddress(nftAddress)).ConfigureAwait(false);

            // slice get_editor()
            var result = await tonClient.SmcRunGetMethod(smc.Id, new MethodIdName("get_editor")).ConfigureAwait(false);

            await tonClient.SmcForget(smc.Id).ConfigureAwait(false);

            TonLibNonZeroExitCodeException.ThrowIfNonZero(result.ExitCode);

            return result.Stack[0].ToTvmCell().ToBoc().RootCells[0].BeginRead().TryLoadAddressIntStd();
        }

        /// <summary>
        /// Executes 'get_domain' method on DNS Item NFT contract, returns domain name.
        /// </summary>
        /// <param name="tonClient"><see cref="ITonClient"/> instance.</param>
        /// <param name="nftAddress">Address of DNS Item NFT.</param>
        /// <returns>Domain name stored in this contract, e.g. returns "alice" for "alice.ton" domain.</returns>
        /// <remarks>
        /// <para>DNS Item contract must be deployed and active (to execute get-method).</para>
        /// </remarks>
        /// <exception cref="TonLibNonZeroExitCodeException" />
        /// <seealso href="https://github.com/ton-blockchain/dns-contract/blob/main/func/nft-item.fc#L280">Source of 'get_domain' method.</seealso>
        public async Task<string> GetDomainName(ITonClient tonClient, string nftAddress)
        {
            await tonClient.InitIfNeeded().ConfigureAwait(false);

            var smc = await tonClient.SmcLoad(new Types.AccountAddress(nftAddress)).ConfigureAwait(false);

            // slice get_domain()
            var result = await tonClient.SmcRunGetMethod(smc.Id, new MethodIdName("get_domain")).ConfigureAwait(false);

            await tonClient.SmcForget(smc.Id).ConfigureAwait(false);

            TonLibNonZeroExitCodeException.ThrowIfNonZero(result.ExitCode);

            return System.Text.Encoding.UTF8.GetString(result.Stack[0].ToTvmCell().ToBoc().RootCells[0].Content);
        }

        /// <summary>
        /// Executes 'get_auction_info' method on DNS Item NFT contract, returns auction data (max bid, bidder, end time).
        /// </summary>
        /// <param name="tonClient"><see cref="ITonClient"/> instance.</param>
        /// <param name="nftAddress">Address of DNS Item NFT.</param>
        /// <returns>Auction data (max bid, bidder, end time), or <b>null</b> if auction not yet started or already finished.</returns>
        /// <remarks>
        /// <para>DNS Item contract must be deployed and active (to execute get-method).</para>
        /// </remarks>
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

            TonLibNonZeroExitCodeException.ThrowIfNonZero(result.ExitCode);

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
        /// <remarks>
        /// <para>DNS Item contract must be deployed and active (to execute get-method).</para>
        /// </remarks>
        /// <exception cref="TonLibNonZeroExitCodeException" />
        /// <seealso href="https://github.com/ton-blockchain/dns-contract/blob/main/func/nft-item.fc#L290">Source of 'get_last_fill_up_time' method.</seealso>
        public async Task<DateTimeOffset> GetLastFillUpTime(ITonClient tonClient, string nftAddress)
        {
            await tonClient.InitIfNeeded().ConfigureAwait(false);

            var smc = await tonClient.SmcLoad(new Types.AccountAddress(nftAddress)).ConfigureAwait(false);

            // slice get_domain()
            var result = await tonClient.SmcRunGetMethod(smc.Id, new MethodIdName("get_last_fill_up_time")).ConfigureAwait(false);

            await tonClient.SmcForget(smc.Id).ConfigureAwait(false);

            TonLibNonZeroExitCodeException.ThrowIfNonZero(result.ExitCode);

            return DateTimeOffset.FromUnixTimeSeconds(long.Parse(result.Stack[0].ToTvmNumberDecimal(), CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Calls 'dns.resolve' tonlib method against DNS Item NFT contract, returns records.
        /// </summary>
        /// <param name="tonClient"><see cref="ITonClient"/> instance.</param>
        /// <param name="nftAddress">Address of DNS Item NFT.</param>
        /// <returns>All records that stored in DNS Item NFT data.</returns>
        /// <remarks>
        /// <para>DNS Item contract must be deployed and active (to execute get-method).</para>
        /// </remarks>
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
        /// Combines <see cref="GetNftAddress(ITonClient, string)">GetNftAddress</see> and <see cref="GetAllInfo(ITonClient, string)">GetAllInfo</see> calls.
        /// </summary>
        /// <param name="tonClient"><see cref="ITonClient"/> instance.</param>
        /// <param name="domainName">Domain name to resolve, with or without '.ton' suffix.</param>
        /// <returns><see cref="DomainInfo"/> with data about domain.</returns>
        /// <remarks>
        /// <para>DNS Item contract must be deployed and active (to execute get-method).</para>
        /// <para>⚠ Will work only for <see href="https://github.com/ton-blockchain/dns-contract/blob/main/func/nft-item.fc">DNS Item smartcontract</see>. May fail if future version will change stored data layout.</para>
        /// <para>Check overriden <see cref="TryNormalizeName">TryNormalizeName</see> docs for 'valid' vs 'invalid' names description.</para>
        /// </remarks>
        public async Task<DomainInfo> GetAllInfoByName(ITonClient tonClient, string domainName)
        {
            var address = await GetNftAddress(tonClient, domainName).ConfigureAwait(false);

            return await GetAllInfo(tonClient, address).ConfigureAwait(false);
        }

        /// <summary>
        /// Parses all contract data from *.ton DNS Item NFT.
        /// </summary>
        /// <param name="tonClient"><see cref="ITonClient"/> instance.</param>
        /// <param name="nftAddress">Address of DNS Item NFT.</param>
        /// <returns><see cref="DomainInfo"/> with data about domain.</returns>
        /// <remarks>
        /// <para>DNS Item contract must be deployed and active (to execute get-method).</para>
        /// <para>⚠ Will work only for <see href="https://github.com/ton-blockchain/dns-contract/blob/main/func/nft-item.fc">DNS Item smartcontract</see>. May fail if future version will change stored data layout.</para>
        /// </remarks>
        public async Task<DomainInfo> GetAllInfo(ITonClient tonClient, string nftAddress)
        {
            var di = new DomainInfo
            {
                Address = nftAddress,
                IsDeployed = false,
            };

            var smc = await tonClient.SmcLoad(nftAddress).ConfigureAwait(false);
            var data = await tonClient.SmcGetData(smc.Id).ConfigureAwait(false);
            await tonClient.SmcForget(smc.Id);

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
            di.OwnerAddress = slice.TryLoadAddressIntStd();

            var content = slice.LoadRef();
            var contentSlice = content.BeginRead();
            contentSlice.SkipBits(8);
            di.Entries = LoadDns(contentSlice);

            var domain = slice.LoadRef();
            di.Domain = System.Text.Encoding.UTF8.GetString(domain.Content);

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

        public class DomainInfo
        {
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
            /// Domain name (e.g. 'alice' for 'alice.ton').
            /// </summary>
            public string Domain { get; set; } = string.Empty;

            /// <summary>
            /// Address of editor (owner) of this domain, or null if domain is not yet owned (e.g. auction is in progress).
            /// </summary>
            public string? OwnerAddress { get; set; } = string.Empty;

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
