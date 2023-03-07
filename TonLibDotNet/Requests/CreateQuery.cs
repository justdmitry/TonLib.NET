using TonLibDotNet.Types;
using TonLibDotNet.Types.Query;

namespace TonLibDotNet.Requests
{
    [TLSchema("createQuery private_key:InputKey address:accountAddress timeout:int32 action:Action initial_account_state:InitialAccountState = query.Info")]
    public class CreateQuery : RequestBase<Info>
    {
        public CreateQuery(InputKey privateKey, AccountAddress address, Types.Action action)
        {
            PrivateKey = privateKey ?? throw new ArgumentNullException(nameof(privateKey));
            Address = address ?? throw new ArgumentNullException(nameof(address));
            Action = action ?? throw new ArgumentNullException(nameof(action));
        }

        public InputKey PrivateKey { get; set; }

        public AccountAddress Address { get; set; }

        public long Timeout { get; set; }

        public Types.Action Action { get; set; }

        public InitialAccountState? InitialAccountState { get; set; }
    }
}
