namespace TonLibDotNet.Requests.LiteServer
{
    public class GetInfoTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.LiteServer_GetInfo);

            Assert.NotNull(baseObj);

            Assert.IsType<LiteServer.GetInfo>(baseObj);
        }
    }
}
