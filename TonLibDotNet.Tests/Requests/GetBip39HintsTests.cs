namespace TonLibDotNet.Requests
{
    public class GetBip39HintsTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.GetBip39Hints);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<GetBip39Hints>(baseObj);
            Assert.Equal("au", obj.Prefix);
        }
    }
}
