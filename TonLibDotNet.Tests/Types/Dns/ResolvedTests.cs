namespace TonLibDotNet.Types.Dns
{
    public class ResolvedTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.Dns_Resolved);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<Resolved>(baseObj);

            Assert.Single(obj.Entries);

            var entry = Assert.IsType<Dns.Entry>(obj.Entries[0]);

            Assert.Equal("toncenter.", entry.Name);
        }
    }
}
