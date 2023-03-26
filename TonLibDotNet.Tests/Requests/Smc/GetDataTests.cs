namespace TonLibDotNet.Requests.Smc
{
    public class GetDataTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.Smc_GetData);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<GetData>(baseObj);

            Assert.Equal(1, obj.Id);
        }
    }
}
