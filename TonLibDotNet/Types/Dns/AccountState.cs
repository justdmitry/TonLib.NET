namespace TonLibDotNet.Types.Dns
{
    [TLSchema("dns.accountState wallet_id:int64 = AccountState")]
    public class AccountState
    {
        public long WalletId { get; set; }
    }
}
