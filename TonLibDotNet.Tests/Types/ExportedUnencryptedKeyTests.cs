namespace TonLibDotNet.Types
{
    public class ExportedUnencryptedKeyTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.ExportedUnencryptedKey);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<ExportedUnencryptedKey>(baseObj);

            Assert.Equal("Vn9nPsOY7YZ3iNnpV5Opsk7oY1eDclOkm5ht0N0XtrE=", obj.Data);
        }
    }
}
