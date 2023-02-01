namespace TonLibDotNet.Types
{
    public class ErrorTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.Error);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<Error>(baseObj);

            Assert.Equal(400, obj.Code);
            Assert.Equal("INVALID_ACCOUNT_ADDRESSFailed to parse account address", obj.Message);
        }
    }
}
