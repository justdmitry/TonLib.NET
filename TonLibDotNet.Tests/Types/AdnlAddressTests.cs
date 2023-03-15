namespace TonLibDotNet.Types
{
    public class AdnlAddressTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.AdnlAddress);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<AdnlAddress>(baseObj);

            Assert.Equal("xfupfrvndwrx7ryltn2zmrgqlwmcdcv2zuhifqgachk3a4xsvi2c4qh", obj.Value);
        }
    }
}
