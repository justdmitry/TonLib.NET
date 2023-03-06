namespace TonLibDotNet.Types.Wallet
{
    public class HighloadV1InitialAccountStateTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.Wallet_HighloadV1InitialAccountState);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<HighloadV1InitialAccountState>(baseObj);

            Assert.Equal(4085333890, obj.WalletId);
            Assert.Equal("PuZi0henKVaHtfaaRZs-DedaOxKUlWkS_jBE1qYLQ4m9mpU9", obj.PublicKey);
        }
    }
}
