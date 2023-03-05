using TonLibDotNet.Types;

namespace TonLibDotNet.Requests
{
    public class ChangeLocalPasswordTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.ChangeLocalPassword);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<ChangeLocalPassword>(baseObj);

            Assert.NotNull(obj.InputKey);

            var ik = Assert.IsType<InputKeyRegular>(obj.InputKey);

            Assert.NotNull(ik.Key);
            Assert.Equal("Pub...NZz4", ik.Key.PublicKey);
            Assert.Equal("yXO...nA=", ik.Key.Secret);
            Assert.Equal("AQIDBAU=", ik.LocalPassword);

            Assert.Equal("BwYF", obj.NewLocalPassword);
        }
    }
}
