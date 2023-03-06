namespace TonLibDotNet.Types.Wallet
{
    public class V3InitialAccountStateTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.Wallet_V3InitialAccountState);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<V3InitialAccountState>(baseObj);

            Assert.Equal(4085333890, obj.WalletId);
            Assert.Equal("PuZi0henKVaHtfaaRZs-DedaOxKUlWkS_jBE1qYLQ4m9mpU9", obj.PublicKey);
        }
    }
}
