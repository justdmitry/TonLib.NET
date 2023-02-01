namespace TonLibDotNet.Types.Wallet
{
    public class V3AccountStateTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.Wallet_V3AccountState);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<V3AccountState>(baseObj);

            Assert.Equal(698983191, obj.WalletId);
            Assert.Equal(35, obj.Seqno);
        }
    }
}
