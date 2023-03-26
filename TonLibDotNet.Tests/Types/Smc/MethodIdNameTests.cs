namespace TonLibDotNet.Types.Smc
{
    public class MethodIdNameTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.Smc_MethodIdName);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<MethodIdName>(baseObj);

            Assert.Equal("counter", obj.Name);
        }
    }
}
