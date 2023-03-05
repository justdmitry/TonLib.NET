namespace TonLibDotNet.Requests
{
    public class ImportEncryptedKeyTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.ImportEncryptedKey);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<ImportEncryptedKey>(baseObj);

            Assert.NotNull(obj.ExportedEncryptedKey);

            Assert.Equal("sAtI...YOYg=", obj.ExportedEncryptedKey.Data);
            Assert.Equal("AQIDBAU=", obj.LocalPassword);
            Assert.Equal("FQMHCw==", obj.KeyPassword);
        }
    }
}
