using TonLibDotNet.Types;

namespace TonLibDotNet.Requests
{
    public class ExportKeyTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.ExportKey);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<ExportKey>(baseObj);

            Assert.NotNull(obj.InputKey);

            var ikr = Assert.IsType<InputKeyRegular>(obj.InputKey);

            Assert.NotNull(ikr.Key);

            Assert.Equal("PuY...CQ", ikr.Key.PublicKey);
            Assert.Equal("K5F..I=", ikr.Key.Secret);
            Assert.Equal("PQ...Q=", ikr.LocalPassword);
        }
    }
}
