using TonLibDotNet.Types;

namespace TonLibDotNet.Requests
{
    public class PackAccountAddressTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.PackAccountAddress);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<PackAccountAddress>(baseObj);

            Assert.NotNull(obj.AccountAddress);
            var paa = Assert.IsType<UnpackedAccountAddress>(obj.AccountAddress);

            Assert.Equal(0, paa.WorkchainId);
            Assert.True(paa.Bounceable);
            Assert.False(paa.Testnet);
            Assert.Equal("iU5IXdVtsM7ZFTaf3bCgab9kiKFFqCMsyO5kK20q4/w=", paa.Addr);
        }
    }
}
