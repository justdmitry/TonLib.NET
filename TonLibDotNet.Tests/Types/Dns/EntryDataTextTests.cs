namespace TonLibDotNet.Types.Dns
{
    public class EntryDataTextTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.Dns_EntryDataText);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<EntryDataText>(baseObj);

            Assert.Equal("Hello, World", obj.Text);
        }
    }
}
