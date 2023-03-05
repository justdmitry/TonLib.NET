namespace TonLibDotNet.Requests
{
    public class ImportUnencryptedKeyTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.ImportUnencryptedKey);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<ImportUnencryptedKey>(baseObj);

            Assert.NotNull(obj.ExportedUnencryptedKey);

            Assert.Equal("Zdb...YM=", obj.ExportedUnencryptedKey.Data);
            Assert.Equal("AQIDBAU=", obj.LocalPassword);
        }
    }
}
