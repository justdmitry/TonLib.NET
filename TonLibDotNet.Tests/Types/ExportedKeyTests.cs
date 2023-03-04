namespace TonLibDotNet.Types
{
    public class ExportedKeyTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.ExportedKey);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<ExportedKey>(baseObj);

            Assert.NotEmpty(obj.WordList);
            Assert.Equal(new[] { "gasp", "shine", "patient", "poverty", "oak", "leaf", "actor", "shift", "festival", "route", "six", "track", "same", "home", "derive", "ready", "table", "eye", "card", "praise", "affair", "when", "ship", "trim" }, obj.WordList);
        }
    }
}
