namespace TonLibDotNet.Types.Uninited
{
    [TLSchema("uninited.accountState frozen_hash:bytes = AccountState")]
    public class AccountState : Types.AccountState
    {
        public string FrozenHash { get; set; } = string.Empty;
    }
}
