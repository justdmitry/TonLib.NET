namespace TonLibDotNet.Types.Tvm
{
    public class StackEntryNumberTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.Tvm_StackEntryNumber);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<StackEntryNumber>(baseObj);

            var obj2 = Assert.IsType<NumberDecimal>(obj.Number);

            Assert.Equal("1674271323360", obj2.Number);
        }
    }
}
