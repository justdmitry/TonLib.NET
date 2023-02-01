namespace TonLibDotNet.Types.Raw
{
    public class TransactionTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.Raw_Transaction);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<Transaction>(baseObj);

            Assert.Equal(new DateTimeOffset(2023, 2, 1, 15, 1, 17, TimeSpan.Zero), obj.Utime);
            Assert.Equal("te6cC9u5d6", obj.Data);

            Assert.NotNull(obj.TransactionId);
            Assert.Equal(34948872000007, obj.TransactionId.Lt);
            Assert.Equal("j2fRsAIg2Abex+P+GCRu9dNcmGs36Wd9U0gedEGX8oA=", obj.TransactionId.Hash);
            Assert.Equal(12, obj.Fee);
            Assert.Equal(13, obj.StorageFee);
            Assert.Equal(15, obj.OtherFee);

            Assert.NotNull(obj.InMsg);
            Assert.Equal("lqKW0iTyhcZ77pPDD4owkVfw2qNdxbh+QQt4YwoJz8c=", obj.InMsg.BodyHash);

            Assert.NotNull(obj.OutMsgs);
            Assert.Single(obj.OutMsgs);
            Assert.Equal("7bbKzPJQlgSoXts1birKs/81068A59f3Tm9QUdxlPUQ=", obj.OutMsgs[0].BodyHash);
        }
    }
}
