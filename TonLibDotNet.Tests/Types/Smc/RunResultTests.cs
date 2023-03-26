using TonLibDotNet.Types.Tvm;

namespace TonLibDotNet.Types.Smc
{
    public class RunResultTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.Smc_RunResult);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<RunResult>(baseObj);

            Assert.Equal(479, obj.GasUsed);
            Assert.Single(obj.Stack);
            Assert.IsType<StackEntryNumber>(obj.Stack[0]);
            Assert.Equal(123, obj.ExitCode);
        }
    }
}
