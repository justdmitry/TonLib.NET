namespace TonLibDotNet.Types.Tvm
{
    public class NumberDecimalTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.Tvm_NumberDecimal);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<NumberDecimal>(baseObj);

            Assert.Equal("1674271323360", obj.Number);
        }
    }
}
