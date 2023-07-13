using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using TonLibDotNet.Types.Smc;
using TonLibDotNet.Types.Tvm;
using TonLibDotNet.Utils;

namespace TonLibDotNet.Recipes
{
    /// <remarks>
    /// Based on <see href="https://github.com/ton-blockchain/TEPs/blob/master/text/0081-dns-standard.md">TEP 81: TON DNS Standard</see>
    ///   and <see href="https://github.com/TelegramMessenger/telemint">Telemint (Telegram usernames and numbers) Contracts</see>.
    /// </remarks>
    public abstract class TelemintRecipes : Tep81DNS
    {
        /// <summary>
        /// Collection address (parent for all item NFTs).
        /// </summary>
        public abstract string CollectionAddress { get; }

        /// <summary>
        /// Tries to normalize name to form used by Collection.
        /// </summary>
        /// <param name="name">Name to normalize.</param>
        /// <param name="normalizedName">Normalized name, if possible.</param>
        /// <returns>Returns <b>true</b> when name had been successfully normalized, and <b>false</b> otherwise.</returns>
        public abstract bool TryNormalizeName(string name, [NotNullWhen(true)] out string normalizedName);

        /// <summary>
        /// Computes 'index' value for NFT with specified Name.
        /// </summary>
        /// <param name="name">Name to get 'index' for.</param>
        /// <returns><b>byte[8]</b> with required value (<i>string_hash(domain)</i>, according to <see href="https://github.com/TelegramMessenger/telemint/blob/main/func/nft-collection.fc#L93">contract source</see>).</returns>
        /// <remarks>
        /// <para>Check overriden <see cref="TryNormalizeName">TryNormalizeName</see> docs for 'valid' vs 'invalid' names description.</para>
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">Requested <paramref name="name"/> is not valid or too long.</exception>
        public byte[] GetNftIndex(string name)
        {
            if (!TryNormalizeName(name, out var normalizedName))
            {
                throw new ArgumentOutOfRangeException(nameof(name), "Invalid name.");
            }

            // UTF-8 encoded string up to 126 bytes, https://github.com/ton-blockchain/TEPs/blob/master/text/0081-dns-standard.md#domain-names
            var bytes = System.Text.Encoding.UTF8.GetBytes(normalizedName);
            if (bytes.Length > 126)
            {
                throw new ArgumentOutOfRangeException(nameof(name), "Value is too long. No more than 126 chars are allowed.");
            }

            // index is simple slice_hash(domain), see https://github.com/TelegramMessenger/telemint/blob/main/func/nft-collection.fc#L93
            return System.Security.Cryptography.SHA256.HashData(bytes);
        }

        /// <summary>
        /// Resolves Name into Item NFT address (for both existing/minted and not-yet-minted names) by calling 'get_nft_address_by_index' method.
        /// </summary>
        /// <param name="tonClient"><see cref="ITonClient"/> instance.</param>
        /// <param name="name">Name to resolve.</param>
        /// <returns>Address of Item NFT for requested Name.</returns>
        /// <remarks>
        /// <para>Check overriden <see cref="TryNormalizeName">TryNormalizeName</see> docs for 'valid' vs 'invalid' names description.</para>
        /// </remarks>
        public async Task<string> GetNftAddress(ITonClient tonClient, string name)
        {
            var index = GetNftIndex(name);

            await tonClient.InitIfNeeded().ConfigureAwait(false);

            var smc = await tonClient.SmcLoad(new Types.AccountAddress(CollectionAddress)).ConfigureAwait(false);

            // slice get_nft_address_by_index(int index)
            var stack = new List<StackEntry>()
            {
                new StackEntryNumber(new NumberDecimal(new BigInteger(index, true, true).ToString(CultureInfo.InvariantCulture))),
            };
            var result = await tonClient.SmcRunGetMethod(smc.Id, new MethodIdName("get_nft_address_by_index"), stack).ConfigureAwait(false);

            await tonClient.SmcForget(smc.Id).ConfigureAwait(false);

            TonLibNonZeroExitCodeException.ThrowIfNonZero(result.ExitCode);

            return result.Stack[0].ToBoc().RootCells[0].BeginRead().LoadAddressIntStd();
        }

        /// <summary>
        /// Returns full domain name (e.g. 'alice.t.me' or '888...') for specified NFT by calling 'get_full_domain' on it.
        /// </summary>
        /// <param name="tonClient"><see cref="ITonClient"/> instance.</param>
        /// <param name="nftAddress">Address of existing Item NFT.</param>
        /// <returns>String like 'alice.t.me' or '888...', returned by 'get_full_domain' method (with \0 -> '.' replaced and parts reversed).</returns>
        /// <remarks>
        /// <para>Telemint NFT contract must be deployed and active (to execute get-method).</para>
        /// </remarks>
        /// <exception cref="TonLibNonZeroExitCodeException" />
        /// <seealso href="https://github.com/TelegramMessenger/telemint/blob/main/func/nft-item.fc#L294">Source of 'get_full_domain' method.</seealso>
        public async Task<string> GetFullDomain(ITonClient tonClient, string nftAddress)
        {
            await tonClient.InitIfNeeded().ConfigureAwait(false);

            var smc = await tonClient.SmcLoad(new Types.AccountAddress(nftAddress)).ConfigureAwait(false);

            // slice get_full_domain()
            var result = await tonClient.SmcRunGetMethod(smc.Id, new MethodIdName("get_full_domain")).ConfigureAwait(false);

            await tonClient.SmcForget(smc.Id).ConfigureAwait(false);

            TonLibNonZeroExitCodeException.ThrowIfNonZero(result.ExitCode);

            var invertedDomain = System.Text.Encoding.UTF8.GetString(result.Stack[0].ToBoc().RootCells[0].Content);
            return string.Join('.', invertedDomain.Split((char)0, StringSplitOptions.RemoveEmptyEntries).Reverse());
        }

        /// <summary>
        /// Returns Name (e.g. 'alice' from 'alice.t.me' or '888...' for numbers) for specified NFT by calling 'get_telemint_token_name' on it.
        /// </summary>
        /// <param name="tonClient"><see cref="ITonClient"/> instance.</param>
        /// <param name="nftAddress">Address of existing Item NFT.</param>
        /// <returns>String like 'alice' (for 'alice.t.me' NFT), returned by 'get_telemint_token_name' method.</returns>
        /// <remarks>
        /// <para>Telemint NFT contract must be deployed and active (to execute get-method).</para>
        /// </remarks>
        /// <exception cref="TonLibNonZeroExitCodeException" />
        /// <seealso href="https://github.com/TelegramMessenger/telemint/blob/main/func/nft-item.fc#L302">Source of 'get_telemint_token_name' method.</seealso>
        public async Task<string> GetTokenName(ITonClient tonClient, string nftAddress)
        {
            await tonClient.InitIfNeeded().ConfigureAwait(false);

            var smc = await tonClient.SmcLoad(new Types.AccountAddress(nftAddress)).ConfigureAwait(false);

            // slice get_telemint_token_name()
            var result = await tonClient.SmcRunGetMethod(smc.Id, new MethodIdName("get_telemint_token_name")).ConfigureAwait(false);

            await tonClient.SmcForget(smc.Id).ConfigureAwait(false);

            TonLibNonZeroExitCodeException.ThrowIfNonZero(result.ExitCode);

            return System.Text.Encoding.UTF8.GetString(result.Stack[0].ToBoc().RootCells[0].Content);
        }

        /// <summary>
        /// Combines <see cref="GetNftAddress(ITonClient, string)">GetNftAddress</see> and <see cref="GetAllInfo(ITonClient, string)">GetAllInfo</see> calls.
        /// </summary>
        /// <param name="tonClient"><see cref="ITonClient"/> instance.</param>
        /// <param name="name">Name to resolve and get info for.</param>
        /// <returns><see cref="TelemintInfo"/> with data about smartcontract.</returns>
        /// <remarks>
        /// <para>⚠ Will work only for <see href="https://github.com/TelegramMessenger/telemint/blob/main/func/nft-item.fc">Telemint Item smartcontracts</see>. May fail if future version will change stored data layout.</para>
        /// <para>Telemint NFT contract must be deployed and active (to execute get-method).</para>
        /// <para>Check overriden <see cref="TryNormalizeName">TryNormalizeName</see> docs for 'valid' vs 'invalid' names description.</para>
        /// </remarks>
        public async Task<TelemintInfo> GetAllInfoByName(ITonClient tonClient, string name)
        {
            var address = await GetNftAddress(tonClient, name).ConfigureAwait(false);

            return await GetAllInfo(tonClient, address).ConfigureAwait(false);
        }

        /// <summary>
        /// Parses all contract data from Telemint Item NFT (*.t.me username or +888 number).
        /// </summary>
        /// <param name="tonClient"><see cref="ITonClient"/> instance.</param>
        /// <param name="nftAddress">Address of Telemint Item (*.t.me username or +888 number) NFT.</param>
        /// <returns><see cref="TelemintInfo"/> with data about smartcontract.</returns>
        /// <remarks>
        /// <para>Telemint NFT contract must be deployed and active (to execute get-method).</para>
        /// <para>⚠ Will work only for <see href="https://github.com/TelegramMessenger/telemint/blob/main/func/nft-item.fc">Telemint Item smartcontracts</see>. May fail if future version will change stored data layout.</para>
        /// </remarks>
        public async Task<TelemintInfo> GetAllInfo(ITonClient tonClient, string nftAddress)
        {
            var di = new TelemintInfo
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

            di.IsDeployed = true;

            // from https://github.com/TelegramMessenger/telemint/blob/main/func/common.fc#L183 unpack_item_data
            //  and https://github.com/TelegramMessenger/telemint/blob/main/func/common.fc#L209   pack_item_data
            var slice = data.ToBoc().RootCells[0].BeginRead();
            var config = slice.LoadRef().BeginRead();
            var state = slice.TryLoadDict()?.BeginRead();
            slice.EndRead();

            // from https://github.com/TelegramMessenger/telemint/blob/main/func/common.fc#L166 unpack_item_config
            //  and https://github.com/TelegramMessenger/telemint/blob/main/func/common.fc#L176   pack_item_config
            di.Index = config.LoadBitsToBytes(256);
            di.CollectionAddress = config.LoadAddressIntStd();
            config.EndRead();

            if (state == null)
            {
                return di;
            }

            // from https://github.com/TelegramMessenger/telemint/blob/main/func/common.fc#L235 unpack_item_state
            //  and https://github.com/TelegramMessenger/telemint/blob/main/func/common.fc#L247   pack_item_state
            di.OwnerAddress = state.TryLoadAddressIntStd();
            var content = state.LoadRef().BeginRead();
            var auction = state.TryLoadDict()?.BeginRead();
            var royaltyParams = state.LoadRef().BeginRead();
            state.EndRead();

            // from https://github.com/TelegramMessenger/telemint/blob/main/func/common.fc#L224 unpack_item_content
            //  and https://github.com/TelegramMessenger/telemint/blob/main/func/common.fc#L216   pack_item_content
            _ = content.LoadRef(); // nftContent
            di.Entries = LoadDns(content);
            var tokenInfo = content.LoadRef().BeginRead();
            content.EndRead();

            // from https://github.com/TelegramMessenger/telemint/blob/main/func/common.fc#L103 unpack_token_info
            //  and https://github.com/TelegramMessenger/telemint/blob/main/func/common.fc#L113   pack_token_info
            var nameLength = tokenInfo.LoadByte();
            di.Name = System.Text.Encoding.UTF8.GetString(tokenInfo.LoadBytes(nameLength));
            var domainLength = tokenInfo.LoadByte();
            di.DomainName = string.Join('.', System.Text.Encoding.UTF8.GetString(tokenInfo.LoadBytes(domainLength)).Split((char)0, StringSplitOptions.RemoveEmptyEntries).Reverse());
            tokenInfo.EndRead();

            // from https://github.com/TelegramMessenger/telemint/blob/main/func/common.fc#L390 unpack_auction
            //  and https://github.com/TelegramMessenger/telemint/blob/main/func/common.fc#L400   pack_auction
            if (auction != null)
            {
                var auctionState = auction.LoadRef().BeginRead();
                var auctionConfig = auction.LoadRef().BeginRead();
                auction.EndRead();

                di.AuctionInfo = new AuctionInfo();

                // from https://github.com/TelegramMessenger/telemint/blob/main/func/common.fc#L357 unpack_auction_state
                //  and https://github.com/TelegramMessenger/telemint/blob/main/func/common.fc#L367   pack_auction_state
                var lastBid = auctionState.TryLoadDict()?.BeginRead();
                di.AuctionInfo.MinBid = TonUtils.Coins.FromNano(auctionState.LoadCoins());
                di.AuctionInfo.EndTime = DateTimeOffset.FromUnixTimeSeconds(auctionState.LoadUInt(32));
                auctionState.EndRead();

                // from https://github.com/TelegramMessenger/telemint/blob/main/func/common.fc#L375 unpack_auction_config
                di.AuctionInfo.BeneficiaryAddress = auctionConfig.LoadAddressIntStd();
                di.AuctionInfo.InitialMinBid = TonUtils.Coins.FromNano(auctionConfig.LoadCoins());
                di.AuctionInfo.MaxBid = TonUtils.Coins.FromNano(auctionConfig.LoadCoins());
                di.AuctionInfo.MinBidStep = auctionConfig.LoadByte();
                di.AuctionInfo.MinExtendTime = TimeSpan.FromSeconds(auctionConfig.LoadUInt());
                di.AuctionInfo.Duration = TimeSpan.FromSeconds(auctionConfig.LoadUInt());
                auctionConfig.EndRead();

                // from https://github.com/TelegramMessenger/telemint/blob/main/func/common.fc#L338 unpack_last_bid
                //  and https://github.com/TelegramMessenger/telemint/blob/main/func/common.fc#L348   pack_last_bid
                if (lastBid != null)
                {
                    di.AuctionInfo.LastBid = new();
                    di.AuctionInfo.LastBid.Address = lastBid.LoadAddressIntStd();
                    di.AuctionInfo.LastBid.Amount = TonUtils.Coins.FromNano(lastBid.LoadCoins());
                    di.AuctionInfo.LastBid.Time = DateTimeOffset.FromUnixTimeSeconds(lastBid.LoadUInt());
                    lastBid.EndRead();
                }
            }

            // from https://github.com/TelegramMessenger/telemint/blob/main/func/common.fc#L198 unpack_nft_royalty_params
            //  and https://github.com/TelegramMessenger/telemint/blob/main/func/common.fc#L190   pack_nft_royalty_params
            di.RoyaltyParams.Numerator = royaltyParams.LoadUShort();
            di.RoyaltyParams.Denominator = royaltyParams.LoadUShort();
            di.RoyaltyParams.Destination = royaltyParams.LoadAddressIntStd();
            royaltyParams.EndRead();

            return di;
        }

        /// <summary>
        /// Information from Telemint (*.t.me username or +888 number) smartcontract.
        /// </summary>
        public class TelemintInfo
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
            /// Address of collection (top-level .t.me or +888 numbers contract).
            /// </summary>
            public string CollectionAddress { get; set; } = string.Empty;

            /// <summary>
            /// Address of owner of this domain, or null if domain is not yet owned (e.g. not minted or auction is in progress).
            /// </summary>
            public string? OwnerAddress { get; set; }

            /// <summary>
            /// Name of this item (e.g. 'alice' for 'alice.t.me' or '888...' for number).
            /// </summary>
            public string? Name { get; set; }

            /// <summary>
            /// Parent domain of this item (e.g. 't.me' for 'alice.t.me').
            /// </summary>
            public string? DomainName { get; set; }

            /// <summary>
            /// Auction info (if any).
            /// </summary>
            public AuctionInfo? AuctionInfo { get; set; }

            /// <summary>
            /// Royalty information.
            /// </summary>
            public RoyaltyParams RoyaltyParams { get; set; } = new();

            /// <summary>
            /// DNS Entry records in this domain.
            /// </summary>
            public DnsEntries Entries { get; set; } = new();
        }

        /// <summary>
        /// Auction info (for *.t.me or '+888...' smartcontract).
        /// </summary>
        public class AuctionInfo
        {
            /// <summary>
            /// Minumum bid.
            /// </summary>
            public decimal MinBid { get; set; }

            /// <summary>
            /// Time when auction ends.
            /// </summary>
            public DateTimeOffset EndTime { get; set; }

            /// <summary>
            /// Beneficiary address.
            /// </summary>
            public string BeneficiaryAddress { get; set; } = string.Empty;

            /// <summary>
            /// Initial (min) bid.
            /// </summary>
            public decimal InitialMinBid { get; set; }

            /// <summary>
            /// Max bid (to buy immediately).
            /// </summary>
            public decimal MaxBid { get; set; }

            /// <summary>
            /// Minimum bid step (in percents).
            /// </summary>
            public byte MinBidStep { get; set; }

            /// <summary>
            /// Time to extend auction (if bid received near auction end).
            /// </summary>
            public TimeSpan MinExtendTime { get; set; }

            /// <summary>
            /// Total auction duration.
            /// </summary>
            public TimeSpan Duration { get; set; }

            /// <summary>
            /// Last (current winner) bid (if any).
            /// </summary>
            public LastBid? LastBid { get; set; }
        }

        /// <summary>
        /// Information about last (current winner) bid.
        /// </summary>
        public class LastBid
        {
            /// <summary>
            /// Address of bidder.
            /// </summary>
            public string Address { get; set; } = string.Empty;

            /// <summary>
            /// Bid amount.
            /// </summary>
            public decimal Amount { get; set; }

            /// <summary>
            /// Bid time.
            /// </summary>
            public DateTimeOffset Time { get; set; }
        }

        public class RoyaltyParams
        {
            public ushort Numerator { get; set; }

            public ushort Denominator { get; set; }

            public string Destination { get; set; } = string.Empty;
        }
    }
}
