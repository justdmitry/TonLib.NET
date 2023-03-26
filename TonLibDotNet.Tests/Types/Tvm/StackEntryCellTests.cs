namespace TonLibDotNet.Types.Tvm
{
    public class StackEntryCellTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.Tvm_StackEntryCell);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<StackEntryCell>(baseObj);

            var obj2 = Assert.IsType<Cell>(obj.Cell);

            Assert.Equal("te6cckEBAQEAJAAAQ4AW7psr1kCofjDYDWbjVxFa4J78SsJhlfLDEm0U+hltmfDtDcL7", obj2.Bytes);
        }
    }
}
