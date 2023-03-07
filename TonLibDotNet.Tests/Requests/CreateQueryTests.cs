using TonLibDotNet.Types;
using TonLibDotNet.Types.Wallet;

namespace TonLibDotNet.Requests
{
    public class CreateQueryTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.CreateQuery);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<CreateQuery>(baseObj);

            var ikr = Assert.IsType<InputKeyRegular>(obj.PrivateKey);
            Assert.Equal("PuZ...pU9", ikr.Key.PublicKey);
            Assert.Equal("zB...WY=", ikr.Key.Secret);

            Assert.NotNull(obj.Address);
            Assert.Equal("EQA...dT0", obj.Address.Value);

            Assert.Equal(60, obj.Timeout);

            _ = Assert.IsType<ActionMsg>(obj.Action);
            _ = Assert.IsType<V3InitialAccountState>(obj.InitialAccountState);
        }
    }
}
