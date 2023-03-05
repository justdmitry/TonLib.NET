namespace TonLibDotNet.Types
{
    public class ExportedEncryptedKeyTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.ExportedEncryptedKey);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<ExportedEncryptedKey>(baseObj);

            Assert.Equal("iMX/cKz...zpg==", obj.Data);
        }
    }
}
