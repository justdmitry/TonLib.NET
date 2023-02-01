namespace TonLibDotNet.Types.Raw
{
    public class TransactionsTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.Raw_Transactions);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<Transactions>(baseObj);

            Assert.NotNull(obj.TransactionsList);
            Assert.Equal(2, obj.TransactionsList.Count);
            Assert.Equal("j2fRsAIg2Abex+P+GCRu9dNcmGs36Wd9U0gedEGX8oA=", obj.TransactionsList[0].TransactionId.Hash);
            Assert.Equal("04SaPBuJe5OoLjP4A943p86Y2OLqrRcwv9wQbc1b9YQ=", obj.TransactionsList[1].TransactionId.Hash);

            Assert.NotNull(obj.PreviousTransactionId);
            Assert.Equal(34925318000006, obj.PreviousTransactionId.Lt);
            Assert.Equal("YsAb3Pf4oUwn7H6MFZCnH26CqdosbJnAlzfg3KTW4H4=", obj.PreviousTransactionId.Hash);
        }
    }
}
