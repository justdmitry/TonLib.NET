using TonLibDotNet.Cells;

namespace TonLibDotNet.BlocksTlb
{
    public class TransactionTests
    {
        /*
         * Transaction https://tonscan.org/tx/8DW9F1LSXj62Etlu9XC5Lf5hT0n%2F2a9aPYQOHxWkW9w=
         *     same as https://tonviewer.com/transaction/f035bd1752d25e3eb612d96ef570b92dfe614f49ffd9af5a3d840e1f15a45bdc
         *     same as https://explorer.toncoin.org/transaction?account=0:83dfd552e63729b472fcbcc8c45ebcc6691702558b68ec7527e1ba403a0f31a8&lt=44333298000001&hash=f035bd1752d25e3eb612d96ef570b92dfe614f49ffd9af5a3d840e1f15a45bdc
         */
        const string DataBase64 = @"te6cckECBwEAAUoAA7F4Pf1VLmNym0cvy8yMRevMZpFwJVi2jsdSfhukA6DzGoAAAoUiZ6oIEv
I+ZP+csu5b/Ww2AH3IyZraBX+XRMwdA4+SfIPFgNdgAAKFIX/r3BZbqkgwABQmKAECAwEBoAQAgnKPZ4CIWhhMygQJE3Wmow8XqXr4BABWnZ3b
b88QOnKWAZDgk9ogaasitJqaQ86vh2+QyzkjPZExXG+5Y6krzj7YAAsMTEhASSABq0gAbKlIy4tYiy0htZWJsr9rQB+v4FdFbNVrBLh6ZEsPUc
UAIPf1VLmNym0cvy8yMRevMZpFwJVi2jsdSfhukA6DzGoEBAYihpwAAFCkTJmzhMt1SOzABQFtc2LQnAAAAY1hEf9OgBY0V4XYoAAIANQF40Uz
/r/7eXN0fHrQfDFksLTrZDXRZ0b5b9I25iw1wAYAEAAAAADwn6auad9mOw==";

        [Fact]
        public void ParseTest()
        {
            var slice = Boc.ParseFromBase64(DataBase64).RootCells[0].BeginRead();
            var tx = new BlocksTlb.Transaction(slice);

            Assert.NotNull(tx);

            Assert.Equal("83DFD552E63729B472FCBCC8C45EBCC6691702558B68EC7527E1BA403A0F31A8", Convert.ToHexString(tx.AccountAddr));
            Assert.Equal((ulong)44333298000001, tx.Lt);
            Assert.Equal("2F23E64FF9CB2EE5BFD6C36007DC8C99ADA057F9744CC1D038F927C83C580D76", Convert.ToHexString(tx.PrevTransHash));
            Assert.Equal((ulong)44333055000001, tx.PrevTransLt);
            Assert.Equal((uint)1706730627, tx.Now);
            Assert.Equal((ulong)0, tx.OutmsgCnt);
            Assert.True(tx.OriginalStatus.IsActive);
            Assert.True(tx.EndStatus.IsActive);

            var d = tx.Description as TransactionDescr.Ord;
            Assert.NotNull(d);

            Assert.True(d.CreditFirst);

            Assert.NotNull(d.StoragePh);
            Assert.Equal(49, d.StoragePh.StorageFeesCollected);
            Assert.Equal(0, d.StoragePh.StorageFeesDue);
            Assert.True(d.StoragePh.StatusChange.IsUnchanged);

            Assert.NotNull(d.CreditPh);
            Assert.Equal(0, d.CreditPh.DueFeesCollected);
            Assert.NotNull(d.CreditPh.Credit);
            Assert.Equal(1, d.CreditPh.Credit.Grams);

            Assert.NotNull(d.ComputePh);
            Assert.True(d.ComputePh.IsSkipped);
            Assert.True(((TrComputePhase.Skipped)d.ComputePh).Reason.IsNoGas);

            Assert.Null(d.Action);

            Assert.True(d.Aborted);
        }
    }
}
