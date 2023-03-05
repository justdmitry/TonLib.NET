using TonLibDotNet.Types;

namespace TonLibDotNet.Requests
{
    public class ExportPemKeyTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.ExportPemKey);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<ExportPemKey>(baseObj);

            Assert.NotNull(obj.InputKey);

            var ikr = Assert.IsType<InputKeyRegular>(obj.InputKey);

            Assert.NotNull(ikr.Key);

            Assert.Equal("PuZ...aLD", ikr.Key.PublicKey);
            Assert.Equal("Qe6...bc=", ikr.Key.Secret);
            Assert.Equal("AQIDBAU=", ikr.LocalPassword);

            Assert.Equal("FQMHCw==", obj.KeyPassword);
        }
    }
}
