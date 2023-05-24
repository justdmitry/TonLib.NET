﻿using TonLibDotNet.Cells;
using TonLibDotNet.Types;
using TonLibDotNet.Types.Msg;

namespace TonLibDotNet.Recipes
{
    public partial class RootDnsRecipes
    {
        public decimal DefaultAmount { get; set; } = 0.05M;

        public int DefaultSendMode { get; set; } = 1;

        /// <summary>
        /// Creates message that will write <paramref name="walletAddress"/> into 'wallet' entry of domain contract.
        /// </summary>
        /// <param name="domainNftAddress">DNS Item NFT address.</param>
        /// <param name="walletAddress">Wallet address to write into contract data.<br/> <b>Not</b> needs to be editor/owner of this domain (you may assign any wallet to domain).</param>
        /// <param name="amount">Amount of coins to send with this message (when null, <see cref="DefaultAmount">DefaultAmount</see> value will be used).</param>
        /// <param name="sendMode">SendMode for this message (when null, <see cref="DefaultSendMode">DefaultSendMode</see> value will be used).</param>
        /// <returns>Constructed and ready-to-be-sent Message (by editor/owner of this domain!).</returns>
        /// <remarks>DNS Item contract must already be deployed.</remarks>
        public Message CreateSetWalletMessage(string domainNftAddress, string walletAddress, decimal? amount = null, int? sendMode = null)
        {
            return CreateUpdateMessage(domainNftAddress, CategoryNameWallet, CreateDnsRecordForWallet(walletAddress), amount, sendMode);
        }

        /// <summary>
        /// Creates message that will erase existing value from 'wallet' entry of domain contract.
        /// </summary>
        /// <param name="domainNftAddress">DNS Item NFT address.</param>
        /// <param name="amount">Amount of coins to send with this message (when null, <see cref="DefaultAmount">DefaultAmount</see> value will be used).</param>
        /// <param name="sendMode">SendMode for this message (when null, <see cref="DefaultSendMode">DefaultSendMode</see> value will be used).</param>
        /// <returns>Constructed and ready-to-be-sent Message (by editor/owner of this domain!).</returns>
        /// <remarks>DNS Item contract must already be deployed.</remarks>
        public Message CreateEraseWalletMessage(string domainNftAddress, decimal? amount = null, int? sendMode = null)
        {
            return CreateUpdateMessage(domainNftAddress, CategoryNameWallet, null, amount, sendMode);
        }

        /// <summary>
        /// Creates message that will write <paramref name="adnlAddress"/> into 'site' entry of domain contract.
        /// </summary>
        /// <param name="domainNftAddress">DNS Item NFT address.</param>
        /// <param name="adnlAddress">ADNL address (256 bits) to write into contract data. You may use <see cref="TonUtils.Adnl"/> functions to convert string to bytes.</param>
        /// <param name="amount">Amount of coins to send with this message (when null, <see cref="DefaultAmount">DefaultAmount</see> value will be used).</param>
        /// <param name="sendMode">SendMode for this message (when null, <see cref="DefaultSendMode">DefaultSendMode</see> value will be used).</param>
        /// <returns>Constructed and ready-to-be-sent Message (by editor/owner of this domain!).</returns>
        /// <remarks>DNS Item contract must already be deployed.</remarks>
        public Message CreateUpdateSiteToAdnlMessage(string domainNftAddress, Span<byte> adnlAddress, decimal? amount = null, int? sendMode = null)
        {
            return CreateUpdateMessage(domainNftAddress, CategoryNameSite, CreateDnsRecordForAdnl(adnlAddress), amount, sendMode);
        }

        /// <summary>
        /// Creates message that will write <paramref name="bagId"/> into 'site' entry of domain contract.
        /// </summary>
        /// <param name="domainNftAddress">DNS Item NFT address.</param>
        /// <param name="bagId">Storage BagId (256 bits) to write into contract data.</param>
        /// <param name="amount">Amount of coins to send with this message (when null, <see cref="DefaultAmount">DefaultAmount</see> value will be used).</param>
        /// <param name="sendMode">SendMode for this message (when null, <see cref="DefaultSendMode">DefaultSendMode</see> value will be used).</param>
        /// <returns>Constructed and ready-to-be-sent Message (by editor/owner of this domain!).</returns>
        /// <remarks>DNS Item contract must already be deployed.</remarks>
        public Message CreateUpdateSiteToStorageMessage(string domainNftAddress, Span<byte> bagId, decimal? amount = null, int? sendMode = null)
        {
            return CreateUpdateMessage(domainNftAddress, CategoryNameSite, CreateDnsRecordForStorage(bagId), amount, sendMode);
        }

        /// <summary>
        /// Creates message that will erase existing value from 'site' entry of domain contract.
        /// </summary>
        /// <param name="domainNftAddress">DNS Item NFT address.</param>
        /// <param name="amount">Amount of coins to send with this message (when null, <see cref="DefaultAmount">DefaultAmount</see> value will be used).</param>
        /// <param name="sendMode">SendMode for this message (when null, <see cref="DefaultSendMode">DefaultSendMode</see> value will be used).</param>
        /// <returns>Constructed and ready-to-be-sent Message (by editor/owner of this domain!).</returns>
        /// <remarks>DNS Item contract must already be deployed.</remarks>
        public Message CreateEraseSiteMessage(string domainNftAddress, decimal? amount = null, int? sendMode = null)
        {
            return CreateUpdateMessage(domainNftAddress, CategoryNameSite, null, amount, sendMode);
        }

        /// <summary>
        /// Creates message that will write <paramref name="bagId"/> into 'storage' entry of domain contract.
        /// </summary>
        /// <param name="domainNftAddress">DNS Item NFT address.</param>
        /// <param name="bagId">Storage BagId (256 bits) to write into contract data.</param>
        /// <param name="amount">Amount of coins to send with this message (when null, <see cref="DefaultAmount">DefaultAmount</see> value will be used).</param>
        /// <param name="sendMode">SendMode for this message (when null, <see cref="DefaultSendMode">DefaultSendMode</see> value will be used).</param>
        /// <returns>Constructed and ready-to-be-sent Message (by editor/owner of this domain!).</returns>
        /// <remarks>DNS Item contract must already be deployed.</remarks>
        public Message CreateUpdateStorageMessage(string domainNftAddress, Span<byte> bagId, decimal? amount = null, int? sendMode = null)
        {
            return CreateUpdateMessage(domainNftAddress, CategoryNameStorage, CreateDnsRecordForStorage(bagId), amount, sendMode);
        }

        /// <summary>
        /// Creates message that will erase existing value from 'storage' entry of domain contract.
        /// </summary>
        /// <param name="domainNftAddress">DNS Item NFT address.</param>
        /// <param name="amount">Amount of coins to send with this message (when null, <see cref="DefaultAmount">DefaultAmount</see> value will be used).</param>
        /// <param name="sendMode">SendMode for this message (when null, <see cref="DefaultSendMode">DefaultSendMode</see> value will be used).</param>
        /// <returns>Constructed and ready-to-be-sent Message (by editor/owner of this domain!).</returns>
        /// <remarks>DNS Item contract must already be deployed.</remarks>
        public Message CreateEraseStorageMessage(string domainNftAddress, decimal? amount = null, int? sendMode = null)
        {
            return CreateUpdateMessage(domainNftAddress, CategoryNameStorage, null, amount, sendMode);
        }

        /// <summary>
        /// Creates message that will write <paramref name="nextResolverAddress"/> into 'dns_next_resolver' entry of domain contract.
        /// </summary>
        /// <param name="domainNftAddress">DNS Item NFT address.</param>
        /// <param name="nextResolverAddress">Next resolver contract address to write into this contract data.</param>
        /// <param name="amount">Amount of coins to send with this message (when null, <see cref="DefaultAmount">DefaultAmount</see> value will be used).</param>
        /// <param name="sendMode">SendMode for this message (when null, <see cref="DefaultSendMode">DefaultSendMode</see> value will be used).</param>
        /// <returns>Constructed and ready-to-be-sent Message (by editor/owner of this domain!).</returns>
        /// <remarks>DNS Item contract must already be deployed.</remarks>
        public Message CreateUpdateNextResolverMessage(string domainNftAddress, string nextResolverAddress, decimal? amount = null, int? sendMode = null)
        {
            return CreateUpdateMessage(domainNftAddress, CategoryNameNextDnsResolver, CreateDnsRecordForNextResolver(nextResolverAddress), amount, sendMode);
        }

        /// <summary>
        /// Creates message that will erase existing value from 'dns_next_resolver' entry of domain contract.
        /// </summary>
        /// <param name="domainNftAddress">DNS Item NFT address.</param>
        /// <param name="amount">Amount of coins to send with this message (when null, <see cref="DefaultAmount">DefaultAmount</see> value will be used).</param>
        /// <param name="sendMode">SendMode for this message (when null, <see cref="DefaultSendMode">DefaultSendMode</see> value will be used).</param>
        /// <returns>Constructed and ready-to-be-sent Message (by editor/owner of this domain!).</returns>
        /// <remarks>DNS Item contract must already be deployed.</remarks>
        public Message CreateEraseNextResolverMessage(string domainNftAddress, decimal? amount = null, int? sendMode = null)
        {
            return CreateUpdateMessage(domainNftAddress, CategoryNameNextDnsResolver, null, amount, sendMode);
        }

        protected Message CreateUpdateMessage(string domainNftAddress, string category, Cell? dnsRecord, decimal? amount = null, int? sendMode = null)
        {
            var body = new CellBuilder()
                .StoreUInt(OPChangeDnsRecord)
                .StoreULong(0) // query_id
                .StoreBytes(EncodeCategory(category));

            if (dnsRecord != null)
            {
                body.StoreRef(dnsRecord);
            }

            return new Message(new AccountAddress(domainNftAddress))
            {
                Amount = TonUtils.Coins.ToNano(amount ?? DefaultAmount),
                Data = new DataRaw(new Boc(body.Build()).SerializeToBase64(), string.Empty),
                SendMode = sendMode ?? DefaultSendMode,
            };
        }

        protected static Cell CreateDnsRecordForWallet(string walletAddress)
        {
            return new CellBuilder()
                .StoreUShort(DnsEntryTypeWallet)
                .StoreAddressIntStd(walletAddress)
                .StoreByte(0) // flags
                .Build();
        }

        protected static Cell CreateDnsRecordForNextResolver(string nextResolverAddress)
        {
            return new CellBuilder()
                .StoreUShort(DnsEntryTypeNextDnsResolver)
                .StoreAddressIntStd(nextResolverAddress)
                .Build();
        }

        protected static Cell CreateDnsRecordForAdnl(ReadOnlySpan<byte> adnl)
        {
            if (adnl.Length != 32)
            {
                throw new ArgumentOutOfRangeException(nameof(adnl), "ADNL address must be 32 bytes long");
            }

            return new CellBuilder()
                .StoreUShort(DnsEntryTypeAdnl)
                .StoreBytes(adnl)
                .StoreByte(0)
                .Build();
        }

        protected static Cell CreateDnsRecordForStorage(ReadOnlySpan<byte> bagId)
        {
            if (bagId.Length != 32)
            {
                throw new ArgumentOutOfRangeException(nameof(bagId), "BagId must be 32 bytes long");
            }

            return new CellBuilder()
                .StoreUShort(DnsEntryTypeStorageBagId)
                .StoreBytes(bagId)
                .Build();
        }
    }
}
