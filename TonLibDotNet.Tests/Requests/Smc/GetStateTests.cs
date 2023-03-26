namespace TonLibDotNet.Requests.Smc
{
    public class GetStateTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.Smc_GetState);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<GetState>(baseObj);

            Assert.Equal(1, obj.Id);
        }
    }
}
