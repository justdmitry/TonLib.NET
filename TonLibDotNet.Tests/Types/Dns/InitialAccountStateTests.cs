namespace TonLibDotNet.Types.Dns
{
    public class InitialAccountStateTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.Dns_InitialAccountState);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<InitialAccountState>(baseObj);

            Assert.Equal("PuboprBeKvVkQVnCcT7lt3WkNMaHL9F4OM9oxnWOgQ-ZB9Nm", obj.PublicKey);
            Assert.Equal(698983191, obj.WalletId);
        }
    }
}
