using TonLibDotNet.Types;
using TonLibDotNet.Types.Smc;

namespace TonLibDotNet.Requests.Smc
{
    [TLSchema("smc.load account_address:accountAddress = smc.Info")]
    public class Load : RequestBase<Info>
    {
        public Load(AccountAddress accountAddress)
        {
            AccountAddress = accountAddress ?? throw new ArgumentNullException(nameof(accountAddress));
        }

        public AccountAddress AccountAddress { get; set; }
    }
}
