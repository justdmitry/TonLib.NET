namespace TonLibDotNet.Types.Internal
{
    public class TransactionIdTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.Internal_TransactionId);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<TransactionId>(baseObj);

            Assert.Equal(34942442000005, obj.Lt);
            Assert.Equal("vhl79O/OZ4LCC7cdkKj22uNv5diIrIltOLujpFOLwTk=", obj.Hash);
        }
    }
}
