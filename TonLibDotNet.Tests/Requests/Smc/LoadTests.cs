namespace TonLibDotNet.Requests.Smc
{
    public class LoadTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.Smc_Load);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<Load>(baseObj);

            Assert.Equal("EQBYLTm4nsvoqJRvs_L-IGNKwWs5RKe19HBK_lFadf19FUfb", obj.AccountAddress.Value);
        }
    }
}
