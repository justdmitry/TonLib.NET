namespace TonLibDotNet.Types
{
    public class InputKeyTests
    {
        [Fact]
        public void DeserializeFakeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.InputKey_Fake);

            Assert.NotNull(baseObj);

            _ = Assert.IsType<InputKeyFake>(baseObj);
        }

        [Fact]
        public void DeserializeRegularOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.InputKey_Regular);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<InputKeyRegular>(baseObj);

            Assert.NotNull(obj.Key);
            Assert.Equal("PuY...qoCQ", obj.Key.PublicKey);
            Assert.Equal("K5F...JhgI=", obj.Key.Secret);

            Assert.Equal("PQ...Q=", obj.LocalPassword);
        }
    }
}
