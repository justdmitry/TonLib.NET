namespace TonLibDotNet.Requests.Smc
{
    public class GetCodeTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.Smc_GetCode);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<GetCode>(baseObj);

            Assert.Equal(1, obj.Id);
        }
    }
}
