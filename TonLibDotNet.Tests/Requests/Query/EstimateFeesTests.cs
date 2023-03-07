namespace TonLibDotNet.Requests.Query
{
    public class EstimateFeesTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.Query_EstimateFees);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<EstimateFees>(baseObj);

            Assert.Equal(1, obj.Id);
            Assert.True(obj.IgnoreChksig);
        }
    }
}
