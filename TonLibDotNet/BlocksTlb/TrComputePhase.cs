using TonLibDotNet.Cells;

namespace TonLibDotNet.BlocksTlb
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks><code>
    /// tr_phase_compute_skipped$0 reason:ComputeSkipReason = TrComputePhase;
    ///
    /// tr_phase_compute_vm$1 success:Bool msg_state_used:Bool
    ///     account_activated:Bool gas_fees:Grams
    ///     ^[gas_used:(VarUInteger 7)
    ///         gas_limit:(VarUInteger 7) gas_credit:(Maybe(VarUInteger 3))
    ///         mode:int8 exit_code:int32 exit_arg:(Maybe int32)
    ///         vm_steps:uint32
    ///         vm_init_state_hash:bits256 vm_final_state_hash:bits256]
    ///     = TrComputePhase;
    /// </code></remarks>
    public abstract class TrComputePhase
    {
        public virtual bool IsSkipped { get; } = false;

        public virtual bool IsVm { get; } = false;

        public static TrComputePhase CreateFrom(Slice src)
        {
            return src.LoadBit() ? new Vm(src) : new Skipped(src);
        }

        public class Skipped : TrComputePhase
        {
            public Skipped(Slice src)
            {
                Reason = ComputeSkipReason.CreateFrom(src);
            }

            public override bool IsSkipped => true;

            public ComputeSkipReason Reason { get; set; }
        }

        public class Vm : TrComputePhase
        {
            public Vm(Slice src)
            {
                Success = src.LoadBit();
                MsgStateUsed = src.LoadBit();
                AccountActivated = src.LoadBit();
                GasFees = src.LoadCoins();
                GasUsed = src.LoadRef();
            }

            public override bool IsVm => true;

            public bool Success { get; set; }

            public bool MsgStateUsed { get; set; }

            public bool AccountActivated { get; set; }

            public long GasFees { get; set; }

            public Cell GasUsed { get; set; }
        }
    }
}
