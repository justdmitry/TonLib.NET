using TonLibDotNet.Types.Dns;

namespace TonLibDotNet.Types
{
    public class ActionDnsTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.ActionDns);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<ActionDns>(baseObj);

            Assert.Equal(2, obj.Actions.Count);

            Assert.IsType<ActionDelete>(obj.Actions[0]);
            Assert.IsType<ActionDeleteAll>(obj.Actions[1]);
        }
    }
}
