namespace TonLibDotNet.Types
{
    public class ConfigTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.Config);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<Config>(baseObj);

            Assert.Equal("test-value", obj.ConfigJson);
            Assert.Equal("test", obj.BlockchainName);
            Assert.True(obj.UseCallbacksForNetwork);
            Assert.True(obj.IgnoreCache);
        }
    }
}
