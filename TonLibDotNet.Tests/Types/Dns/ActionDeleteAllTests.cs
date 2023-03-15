namespace TonLibDotNet.Types.Dns
{
    public class ActionDeleteAllTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.Dns_ActionDeleteAll);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<ActionDeleteAll>(baseObj);
        }
    }
}
