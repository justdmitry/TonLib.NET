using TonLibDotNet.Types;

namespace TonLibDotNet.Requests
{
    public class ExportEncryptedKeyTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.ExportEncryptedKey);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<ExportEncryptedKey>(baseObj);

            Assert.NotNull(obj.InputKey);

            var ikr = Assert.IsType<InputKeyRegular>(obj.InputKey);

            Assert.NotNull(ikr.Key);

            Assert.Equal("PuY...EIP", ikr.Key.PublicKey);
            Assert.Equal("B6Y...rg=", ikr.Key.Secret);
            Assert.Equal("AQIDBAU=", ikr.LocalPassword);

            Assert.Equal("FQMHCw==", obj.KeyPassword);
        }
    }
}
