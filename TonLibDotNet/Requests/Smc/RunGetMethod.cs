using TonLibDotNet.Types.Smc;
using TonLibDotNet.Types.Tvm;

namespace TonLibDotNet.Requests.Smc
{
    [TLSchema("smc.runGetMethod id:int53 method:smc.MethodId stack:vector<tvm.StackEntry> = smc.RunResult")]
    public class RunGetMethod : RequestBase<RunResult>
    {
        public RunGetMethod(long id, MethodId method, List<StackEntry>? stack)
        {
            Id = id;
            Method = method ?? throw new ArgumentNullException(nameof(method));
            Stack = stack;
        }

        public long Id { get; set; }

        public MethodId Method { get; set; }

        public List<StackEntry>? Stack { get; set; }
    }
}
