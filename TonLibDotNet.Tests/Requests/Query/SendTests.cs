namespace TonLibDotNet.Requests.Query
{
    public class SendTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.Query_Send);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<Send>(baseObj);

            Assert.Equal(1, obj.Id);
        }
    }
}
