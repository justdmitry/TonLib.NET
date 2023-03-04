namespace TonLibDotNet.Requests
{
    public class DeleteKeyTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.DeleteKey);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<DeleteKey>(baseObj);

            Assert.NotNull(obj.Key);
            Assert.Equal("Pub...HnG", obj.Key.PublicKey);
            Assert.Equal("gbV...bA=", obj.Key.Secret);
        }
    }
}
