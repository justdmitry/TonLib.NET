using TonLibDotNet.Types.Tvm;

namespace TonLibDotNet.Types.Smc
{
    [TLSchema("smc.runResult gas_used:int53 stack:vector<tvm.StackEntry> exit_code:int32 = smc.RunResult")]
    public class RunResult : TypeBase
    {
        public RunResult(List<StackEntry> stack)
        {
            Stack = stack ?? throw new ArgumentNullException(nameof(stack));
        }

        public long GasUsed { get; set; }

        public List<StackEntry> Stack { get; set; }

        public long ExitCode { get; set; }
    }
}
