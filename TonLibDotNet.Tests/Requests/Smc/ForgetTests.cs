namespace TonLibDotNet.Requests.Smc
{
    public class ForgetTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.Smc_Forget);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<Forget>(baseObj);

            Assert.Equal(1, obj.Id);
        }
    }
}
