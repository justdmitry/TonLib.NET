namespace TonLibDotNet.Requests
{
    public class CreateNewKeyTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.CreateNewKey);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<CreateNewKey>(baseObj);

            Assert.Equal("111", obj.LocalPassword);
            Assert.Equal("222", obj.MnemonicPassword);
            Assert.Equal("333", obj.RandomExtraSeed);
        }
    }
}
