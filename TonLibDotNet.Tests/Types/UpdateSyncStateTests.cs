namespace TonLibDotNet.Types
{
    public class UpdateSyncStateTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.UpdateSyncState);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<UpdateSyncState>(baseObj);

            Assert.NotNull(obj.SyncState);
            var ssip = Assert.IsType<UpdateSyncState.SyncStateInProgress>(obj.SyncState);

            Assert.Equal(27005885, ssip.FromSeqno);
            Assert.Equal(27011756, ssip.ToSeqno);
            Assert.Equal(27011750, ssip.CurrentSeqno);
        }
    }
}
