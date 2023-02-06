namespace TonLibDotNet.Requests
{
    public class UnpackAccountAddressTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.UnpackAccountAddress);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<UnpackAccountAddress>(baseObj);

            Assert.Equal("EQCJTkhd1W2wztkVNp_dsKBpv2SIoUWoIyzI7mQrbSrj_NSh", obj.AccountAddress);
        }
    }
}
