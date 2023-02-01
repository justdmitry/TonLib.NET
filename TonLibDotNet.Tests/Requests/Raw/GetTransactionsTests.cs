namespace TonLibDotNet.Requests.Raw
{
    public class GetTransactionsTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.Raw_GetTransactions);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<GetTransactions>(baseObj);

            Assert.NotNull(obj.AccountAddress);
            Assert.Equal("EQCJTkhd1W2wztkVNp_dsKBpv2SIoUWoIyzI7mQrbSrj_NSh", obj.AccountAddress.Value);

            Assert.NotNull(obj.FromTransactionId);
            Assert.Equal(34948872000007, obj.FromTransactionId.Lt);
            Assert.Equal("j2fRsAIg2Abex+P+GCRu9dNcmGs36Wd9U0gedEGX8oA=", obj.FromTransactionId.Hash);
        }
    }
}
