namespace TonLibDotNet.Types
{
    public class OkTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.Ok);

            Assert.NotNull(baseObj);

            Assert.IsType<Ok>(baseObj);
        }
    }
}
