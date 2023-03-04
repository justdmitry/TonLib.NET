namespace TonLibDotNet.Types
{
    public class KeyTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.Key);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<Key>(baseObj);

            Assert.Equal("Pua...ywkQ", obj.PublicKey);
            Assert.Equal("gSu4...WTnQe0=", obj.Secret);
        }
    }
}
