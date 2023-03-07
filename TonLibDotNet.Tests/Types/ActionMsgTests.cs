namespace TonLibDotNet.Types
{
    public class ActionMsgTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.ActionMsg);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<ActionMsg>(baseObj);

            Assert.Single(obj.Messages);
            Assert.NotNull(obj.Messages[0]);

            Assert.True(obj.AllowSendToUninited);
        }
    }
}
