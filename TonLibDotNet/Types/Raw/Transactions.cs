using System.Text.Json.Serialization;

namespace TonLibDotNet.Types.Raw
{
    [TLSchema("raw.transactions transactions:vector<raw.transaction> previous_transaction_id:internal.transactionId = raw.Transactions")]
    public class Transactions : TypeBase
    {
        public Transactions(List<Transaction> transactionsList, Internal.TransactionId previousTransactionId)
        {
            TransactionsList = transactionsList ?? throw new ArgumentNullException(nameof(transactionsList));
            PreviousTransactionId = previousTransactionId ?? throw new ArgumentNullException(nameof(previousTransactionId));
        }

        [JsonPropertyName("transactions")]
        public List<Transaction> TransactionsList { get; set; }

        public Internal.TransactionId PreviousTransactionId { get; set; }
    }
}
