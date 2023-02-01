using TonLibDotNet.Types;

namespace TonLibDotNet.Requests.Raw
{
    [TLSchema("raw.getTransactions private_key:InputKey account_address:accountAddress from_transaction_id:internal.transactionId = raw.Transactions")]
    public class GetTransactions : RequestBase<Types.Raw.Transactions>
    {
        public GetTransactions(AccountAddress accountAddress, Types.Internal.TransactionId fromTransactionId)
        {
            AccountAddress = accountAddress ?? throw new ArgumentNullException(nameof(accountAddress));
            FromTransactionId = fromTransactionId ?? throw new ArgumentNullException(nameof(fromTransactionId));
        }

        public InputKey? PrivateKey { get; set; }

        public AccountAddress AccountAddress { get; set; }

        public Types.Internal.TransactionId FromTransactionId { get; set; }
    }
}
