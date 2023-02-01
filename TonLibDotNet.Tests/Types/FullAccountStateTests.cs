namespace TonLibDotNet.Types
{
    public class FullAccountStateTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.FullAccountState);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<FullAccountState>(baseObj);

            Assert.NotNull(obj.Address);
            Assert.Equal("EQCJTkhd1W2wztkVNp_dsKBpv2SIoUWoIyzI7mQrbSrj_NSh", obj.Address.Value);
            Assert.Equal(136217239329079, obj.Balance);

            Assert.NotNull(obj.LastTransactionId);
            Assert.Equal(34942442000005, obj.LastTransactionId.Lt);
            Assert.Equal("vhl79O/OZ4LCC7cdkKj22uNv5diIrIltOLujpFOLwTk=", obj.LastTransactionId.Hash);

            Assert.NotNull(obj.BlockId);
            Assert.Equal(-1, obj.BlockId.Workchain);
            Assert.Equal(-9223372036854775808, obj.BlockId.Shard);
            Assert.Equal(27011756, obj.BlockId.Seqno);
            Assert.Equal("AhS55h8lmOyxtw29gWux0XZzXgiAbE69UsP44xIWdXM=", obj.BlockId.RootHash);
            Assert.Equal("TwDj86Fbvgvxte2UXpYtm2CpMSC/wIyxZph6mPDn99A=", obj.BlockId.FileHash);

            Assert.Equal(new DateTimeOffset(2023, 2, 1, 11, 11, 53, TimeSpan.Zero), obj.SyncUtime);

            Assert.NotNull(obj.AccountState);
            var was = Assert.IsType<Wallet.V3AccountState>(obj.AccountState);
            Assert.Equal(698983191, was.WalletId);
            Assert.Equal(35, was.Seqno);

            Assert.Equal(2, obj.Revision);
        }
    }
}
