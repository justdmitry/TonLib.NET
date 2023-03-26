namespace TonLibDotNet.Types.Smc
{
    public class MethodIdNumberTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.Smc_MethodIdNumber);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<MethodIdNumber>(baseObj);

            Assert.Equal(2, obj.Number);
        }
    }
}
