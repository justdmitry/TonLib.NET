using TonLibDotNet.Types;

namespace TonLibDotNet.Requests.Raw
{
    [TLSchema("raw.getTransactionsV2 private_key:InputKey account_address:accountAddress from_transaction_id:internal.transactionId count:# try_decode_messages:Bool = raw.Transactions")]
    public class GetTransactionsV2 : RequestBase<Types.Raw.Transactions>
    {
        public GetTransactionsV2(AccountAddress accountAddress, Types.Internal.TransactionId fromTransactionId, int count)
        {
            AccountAddress = accountAddress ?? throw new ArgumentNullException(nameof(accountAddress));
            FromTransactionId = fromTransactionId ?? throw new ArgumentNullException(nameof(fromTransactionId));
            Count = count;
        }

        public InputKey? PrivateKey { get; set; }

        public AccountAddress AccountAddress { get; set; }

        public Types.Internal.TransactionId FromTransactionId { get; set; }

        public int Count { get; set; }

        public bool TryDecodeMessages { get; set; }
    }
}
