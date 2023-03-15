namespace TonLibDotNet.Types.Dns
{
    public class EntryDataNextResolverTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.Dns_EntryDataNextResolver);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<EntryDataNextResolver>(baseObj);
            var adr = Assert.IsType<AccountAddress>(obj.Resolver);

            Assert.Equal("EQAiIsvar4OYBn8BGBf9flfin6tl5poBx4MgJe4CQJYasy51", adr.Value);
        }
    }
}
