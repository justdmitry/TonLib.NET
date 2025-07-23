using TonLibDotNet.Types;

namespace TonLibDotNet.Requests.Raw
{
    [TLSchema("raw.getTransactionsV2 private_key:InputKey account_address:accountAddress from_transaction_id:internal.transactionId count:# try_decode_messages:Bool = raw.Transactions;")]
    public class GetTransactionsV2 : RequestBase<Types.Raw.Transactions>
    {
        public GetTransactions(AccountAddress accountAddress, Types.Internal.TransactionId fromTransactionId, int count = 3, bool tryDecodeMessages = false)
        {
            AccountAddress = accountAddress ?? throw new ArgumentNullException(nameof(accountAddress));
            FromTransactionId = fromTransactionId ?? throw new ArgumentNullException(nameof(fromTransactionId));
            Count = count;
            TryDecodeMessages = tryDecodeMessages;
        }

        public InputKey? PrivateKey { get; set; }

        public AccountAddress AccountAddress { get; set; }

        public Types.Internal.TransactionId FromTransactionId { get; set; }

        public int Count { get; set; }

        public bool TryDecodeMessages { get; set; }
    }
}
