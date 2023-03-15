namespace TonLibDotNet.Types.Dns
{
    public class ActionDeleteTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.Dns_ActionDelete);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<ActionDelete>(baseObj);

            Assert.Equal("test.ton", obj.Name);
            Assert.Equal("AAA...AAA=", obj.Category);
        }
    }
}
