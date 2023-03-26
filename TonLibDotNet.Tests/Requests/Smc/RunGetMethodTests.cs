using TonLibDotNet.Types.Smc;

namespace TonLibDotNet.Requests.Smc
{
    public class RunGetMethodTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.Smc_RunGetMethod);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<RunGetMethod>(baseObj);

            Assert.Equal(1, obj.Id);

            var m = Assert.IsType<MethodIdName>(obj.Method);

            Assert.Equal("counter", m.Name);
        }
    }
}
