using TonLibDotNet.Types.Wallet;

namespace TonLibDotNet.Requests
{
    public class GetAccountAddressTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.GetAccountAddress);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<GetAccountAddress>(baseObj);

            var ias = Assert.IsType<V3InitialAccountState>(obj.InitialAccountState);
            Assert.Equal("PuZ....mpU9", ias.PublicKey);
            Assert.Equal(698983191, ias.WalletId);

            Assert.Equal(3, obj.Revision);
            Assert.Equal(-1, obj.WorkchainId);
        }
    }
}
