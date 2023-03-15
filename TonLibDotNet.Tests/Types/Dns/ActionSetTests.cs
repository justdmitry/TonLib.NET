namespace TonLibDotNet.Types.Dns
{
    public class ActionSetTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.Dns_ActionSet);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<ActionSet>(baseObj);
            var entry = Assert.IsType<Entry>(obj.Entry);

            Assert.Equal("test2.ton", entry.Name);
            Assert.Equal("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA=", entry.Category);

            var entry2 = Assert.IsType<EntryDataAdnlAddress>(entry.Value);

            var adnl = Assert.IsType<AdnlAddress>(entry2.AdnlAddress);

            Assert.Equal("xfupfrvndwrx7ryltn2zmrgqlwmcdcv2zuhifqgachk3a4xsvi2c4qh", adnl.Value);
        }
    }
}
