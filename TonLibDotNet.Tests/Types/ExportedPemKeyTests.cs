namespace TonLibDotNet.Types
{
    public class ExportedPemKeyTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.ExportedPemKey);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<ExportedPemKey>(baseObj);

            Assert.Equal("-----BEGIN ENCRYPTED PRIVATE KEY-----\nMIGbMFcG...WE71Nf4=\n-----END ENCRYPTED PRIVATE KEY-----\n", obj.Pem);
        }
    }
}
