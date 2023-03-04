namespace TonLibDotNet.Requests
{
    public class DeleteAllKeysTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.DeleteAllKeys);

            Assert.NotNull(baseObj);

            Assert.IsType<DeleteAllKeys>(baseObj);
        }
    }
}
