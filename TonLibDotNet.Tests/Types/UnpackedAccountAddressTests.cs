namespace TonLibDotNet.Types
{
    public class UnpackedAccountAddressTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.UnpackedAccountAddress);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<UnpackedAccountAddress>(baseObj);

            Assert.Equal(0, obj.WorkchainId);
            Assert.True(obj.Bounceable);
            Assert.False(obj.Testnet);
            Assert.Equal("iU5IXdVtsM7ZFTaf3bCgab9kiKFFqCMsyO5kK20q4/w=", obj.Addr);
        }
    }
}
