namespace TonLibDotNet.Requests
{
    public class ImportPemKeyTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.ImportPemKey);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<ImportPemKey>(baseObj);

            Assert.NotNull(obj.ExportedKey);

            Assert.Equal("-----BEGIN ENCRYPTED PRIVATE KEY-----\nMI...lJU=\n-----END ENCRYPTED PRIVATE KEY-----\n", obj.ExportedKey.Pem);
            Assert.Equal("AQIDBAU=", obj.LocalPassword);
            Assert.Equal("FQMHCw==", obj.KeyPassword);
        }
    }
}
