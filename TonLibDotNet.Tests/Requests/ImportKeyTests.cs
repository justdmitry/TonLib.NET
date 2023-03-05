namespace TonLibDotNet.Requests
{
    public class ImportKeyTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.ImportKey);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<ImportKey>(baseObj);

            Assert.NotNull(obj.ExportedKey);

            Assert.NotEmpty(obj.ExportedKey.WordList);
            Assert.Equal(new[] { "fabric", "draw", "forward", "olive", "lamp", "purpose", "sight", "loan", "elevator", "diet", "clutch", "fit", "eagle", "useless", "tiger", "useless", "veteran", "night", "curtain", "spend", "crack", "intact", "resource", "orbit" }, obj.ExportedKey.WordList);
            Assert.Equal("MTIz", obj.LocalPassword);
            Assert.Equal("NDU2", obj.MnemonicPassword);
        }
    }
}
