namespace TonLibDotNet.Types.Raw
{
    public class FullAccountStateTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.RawFullAccountState);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<FullAccountState>(baseObj);

            Assert.Equal(136219372128177, obj.Balance);
            Assert.Equal("te6cckEBAQEAcQAA3v8AIN0gggFMl7ohggEznLqxn3Gw7UTQ0x/THzHXC//jBOCk8mCDCNcYINMf0x/TH/gjE7vyY+1E0NMf0x/T/9FRMrryoVFEuvKiBPkBVBBV+RDyo/gAkyDXSpbTB9QC+wDo0QGkyMsfyx/L/8ntVBC9ba0=", obj.Code);
            Assert.Equal("te6cckEBAQEAKgAAUAAAACMpqaMXb7DhKt9TV4heY445fd1j4cnc24PkHSaiiBxRdV3sZK6FBhF3", obj.Data);

            Assert.NotNull(obj.LastTransactionId);
            Assert.Equal(34948872000007, obj.LastTransactionId.Lt);
            Assert.Equal("j2fRsAIg2Abex+P+GCRu9dNcmGs36Wd9U0gedEGX8oA=", obj.LastTransactionId.Hash);

            Assert.NotNull(obj.BlockId);
            Assert.Equal(-1, obj.BlockId.Workchain);
            Assert.Equal(-9223372036854775808, obj.BlockId.Shard);
            Assert.Equal(27016796, obj.BlockId.Seqno);
            Assert.Equal("ld4MbnHygFd/z7oG3LCnBZtH+2iIDu5N7tRnjeSGwJE=", obj.BlockId.RootHash);
            Assert.Equal("N6CepThwpP7E0EIxEWCHks/VLuUT+ZuEwLHZpWyfO54=", obj.BlockId.FileHash);

            Assert.Equal("---", obj.FrozenHash);

            Assert.Equal(new DateTimeOffset(2023, 2, 1, 16, 1, 8, TimeSpan.Zero), obj.SyncUtime);
        }
    }
}
