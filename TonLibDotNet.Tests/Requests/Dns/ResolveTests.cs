using TonLibDotNet.Types;

namespace TonLibDotNet.Requests.Dns
{
    public class ResolveTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.Dns_Resolve);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<Resolve>(baseObj);

            Assert.Equal("toncenter.", obj.Name);

            var adr = Assert.IsType<AccountAddress>(obj.AccountAddress);
            Assert.Equal("EQC3dNlesgVD8YbAazcauIrXBPfiVhMMr5YYk2in0Mtsz0Bz", adr.Value);

            Assert.Equal("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA=", obj.Category);
            Assert.Equal(777, obj.Ttl);
        }
    }
}
