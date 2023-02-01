namespace TonLibDotNet.Types
{
    public class OptionsInfoTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.Options_Info);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<OptionsInfo>(baseObj);
            Assert.NotNull(obj.ConfigInfo);
            Assert.Equal(698983191L, obj.ConfigInfo.DefaultWalletId);
            Assert.Equal("Puasxr0QfFZZnYISRphVse7XHKfW7pZU5SJarVHXvQ+rpzkD", obj.ConfigInfo.DefaultRwalletInitPublicKey);
        }
    }
}
