using TonLibDotNet.Types;
using TonLibDotNet.Types.Internal;
using TonLibDotNet.Types.Smc;

namespace TonLibDotNet.Requests.Smc
{
    [TLSchema("smc.loadByTransaction account_address:accountAddress transaction_id:internal.transactionId = smc.Info")]
    public class LoadByTransaction : RequestBase<Info>
    {
        public LoadByTransaction(AccountAddress accountAddress, TransactionId transactionId)
        {
            AccountAddress = accountAddress ?? throw new ArgumentNullException(nameof(accountAddress));
            TransactionId = transactionId ?? throw new ArgumentNullException(nameof(transactionId));
        }

        public AccountAddress AccountAddress { get; set; }

        public TransactionId TransactionId { get; set; }
    }
}
