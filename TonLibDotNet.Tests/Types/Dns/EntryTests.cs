namespace TonLibDotNet.Types.Dns
{
    public class EntryTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.Dns_Entry);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<Entry>(baseObj);

            Assert.Equal("toncenter.", obj.Name);
            Assert.Equal("GfAkQe5Yj9sm7iSyVo3QNcPJIG4Rq5eb5i5VVYodF/8=", obj.Category);

            var entry = Assert.IsType<EntryDataNextResolver>(obj.Value);
            var adr = Assert.IsType<AccountAddress>(entry.Resolver);
            Assert.Equal("EQAiIsvar4OYBn8BGBf9flfin6tl5poBx4MgJe4CQJYasy51", adr.Value);
        }
    }
}
