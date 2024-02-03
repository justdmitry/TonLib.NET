using TonLibDotNet.Cells;

namespace TonLibDotNet.BlocksTlb
{
    /// <remarks><code>
    /// cskip_no_state$00 = ComputeSkipReason;
    /// cskip_bad_state$01 = ComputeSkipReason;
    /// cskip_no_gas$10 = ComputeSkipReason;
    /// cskip_suspended$110 = ComputeSkipReason;
    /// </code></remarks>
    public abstract class ComputeSkipReason
    {
        public virtual bool IsNoState { get; } = false;

        public virtual bool IsBadState { get; } = false;

        public virtual bool IsNoGas { get; } = false;

        public virtual bool IsSuspended { get; } = false;

        public static ComputeSkipReason CreateFrom(Slice src)
        {
            if (src.LoadBit())
            {
                if (src.LoadBit())
                {
                    if (!src.LoadBit())
                    {
                        return Suspended.Instance;
                    }
                    else
                    {
                        throw new InvalidOperationException("Failed to load: unexpected bit value");
                    }
                }
                else
                {
                    return NoGas.Instance;
                }
            }
            else
            {
                return src.LoadBit() ? BadState.Instance : NoState.Instance;
            }
        }

        public class NoState : ComputeSkipReason
        {
            public static readonly NoState Instance = new();

            public override bool IsNoState => true;
        }

        public class BadState : ComputeSkipReason
        {
            public static readonly BadState Instance = new();

            public override bool IsBadState => true;
        }

        public class NoGas : ComputeSkipReason
        {
            public static readonly NoGas Instance = new();

            public override bool IsNoGas => true;
        }

        public class Suspended : ComputeSkipReason
        {
            public static readonly Suspended Instance = new();

            public override bool IsSuspended => true;
        }
    }
}
