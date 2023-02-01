namespace TonLibDotNet.Types
{
    [TLSchema("updateSyncState")] // no full schema :(
    public class UpdateSyncState : TypeBase
    {
        public UpdateSyncState(SyncStateBase syncState)
        {
            SyncState = syncState ?? throw new ArgumentNullException(nameof(syncState));
        }

        public SyncStateBase SyncState { get; set; }

        public abstract class SyncStateBase : TypeBase
        {
            // Nothing
        }

        [TLSchema("syncStateInProgress")] // no full schema :(
        public class SyncStateInProgress : SyncStateBase
        {
            public int FromSeqno { get; set; }

            public int ToSeqno { get; set; }

            public int CurrentSeqno { get; set; }
        }

        [TLSchema("syncStateDone")] // no full schema :(
        public class SyncStateDone : SyncStateBase
        {
            // Nothing
        }
    }
}
