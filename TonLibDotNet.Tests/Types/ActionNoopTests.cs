namespace TonLibDotNet.Types
{
    public class ActionNoopTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.ActionNoop);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<ActionNoop>(baseObj);
        }
    }
}
