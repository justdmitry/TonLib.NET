using System.Text.Json.Serialization;
using TonLibDotNet.Types.Tvm;

namespace TonLibDotNet.Types.Smc
{
    [TLSchema("smc.runResult gas_used:int53 stack:vector<tvm.StackEntry> exit_code:int32 = smc.RunResult")]
    public class RunResult : TypeBase
    {
        [JsonConstructor]
        public RunResult(List<StackEntry> stack)
        {
            Stack = stack ?? throw new ArgumentNullException(nameof(stack));
        }

        public RunResult(params StackEntry[] stack)
        {
            Stack = stack.ToList();
        }

        public long GasUsed { get; set; }

        public List<StackEntry> Stack { get; set; }

        public long ExitCode { get; set; }
    }
}
