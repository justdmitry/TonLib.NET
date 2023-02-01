namespace TonLibDotNet.Requests.Blocks
{
    public class GetMasterchainInfoTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.Blocks_GetMasterchainInfo);

            Assert.NotNull(baseObj);

            Assert.IsType<Blocks.GetMasterchainInfo>(baseObj);
        }
    }
}
