using TonLibDotNet.Types;

namespace TonLibDotNet.Requests
{
    public class InitTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.Init);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<Init>(baseObj);

            Assert.NotNull(obj.Options);

            Assert.NotNull(obj.Options.Config);
            Assert.Equal("test-value", obj.Options.Config.ConfigJson);
            Assert.Equal("test", obj.Options.Config.BlockchainName);
            Assert.True(obj.Options.Config.UseCallbacksForNetwork);
            Assert.True(obj.Options.Config.IgnoreCache);

            Assert.NotNull(obj.Options.KeystoreType);
            var ksd = Assert.IsType<KeyStoreTypeDirectory>(obj.Options.KeystoreType);
            Assert.Equal("D:/Temp/", ksd.Directory);
        }
    }
}
