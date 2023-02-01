namespace TonLibDotNet.Types
{
    public class AccountAddressTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.AccountAddress);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<AccountAddress>(baseObj);

            Assert.Equal("EQCJTkhd1W2wztkVNp_dsKBpv2SIoUWoIyzI7mQrbSrj_NSh", obj.Value);
        }
    }
}
