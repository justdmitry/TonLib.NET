using TonLibDotNet.Types;

namespace TonLibDotNet.Requests
{
    public class ExportUnencryptedKeyTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.ExportUnencryptedKey);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<ExportUnencryptedKey>(baseObj);

            Assert.NotNull(obj.InputKey);

            var ikr = Assert.IsType<InputKeyRegular>(obj.InputKey);

            Assert.NotNull(ikr.Key);

            Assert.Equal("PuYJ...gEIP", ikr.Key.PublicKey);
            Assert.Equal("B6Ya...Aorg=", ikr.Key.Secret);
            Assert.Equal("AQIDBAU=", ikr.LocalPassword);
        }
    }
}
