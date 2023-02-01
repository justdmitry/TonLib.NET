namespace TonLibDotNet.Requests
{
    public class SyncTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.Sync);

            Assert.NotNull(baseObj);

            Assert.IsType<Sync>(baseObj);
        }
    }
}
