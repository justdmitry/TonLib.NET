namespace TonLibDotNet.Types.Dns
{
    public class EntryDataAdnlAddressTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.Dns_EntryDataAdnlAddress);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<EntryDataAdnlAddress>(baseObj);
            var adr = Assert.IsType<AdnlAddress>(obj.AdnlAddress);

            Assert.Equal("xfupfrvndwrx7ryltn2zmrgqlwmcdcv2zuhifqgachk3a4xsvi2c4qh", adr.Value);
        }
    }
}
