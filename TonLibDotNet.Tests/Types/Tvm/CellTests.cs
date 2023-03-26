namespace TonLibDotNet.Types.Tvm
{
    public class CellTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.Tvm_Cell);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<Cell>(baseObj);

            Assert.Equal("te6cckEBAQEACgAAEAAAAYXSWPTgZcbsLw==", obj.Bytes);
        }
    }
}
