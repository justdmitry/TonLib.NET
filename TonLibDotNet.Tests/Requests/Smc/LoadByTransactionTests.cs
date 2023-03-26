using TonLibDotNet.Types.Internal;

namespace TonLibDotNet.Requests.Smc
{
    public class LoadByTransactionTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.Smc_LoadByTransaction);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<LoadByTransaction>(baseObj);

            Assert.Equal("EQBYLTm4nsvoqJRvs_L-IGNKwWs5RKe19HBK_lFadf19FUfb", obj.AccountAddress.Value);

            var itid = Assert.IsType<TransactionId>(obj.TransactionId);

            Assert.Equal(9984095000003, itid.Lt);
            Assert.Equal("Q54pwYEYuIj41\u002Bgcw0azQKVZT0hune9g45AYdWWzoUY=", itid.Hash);
        }
    }
}
