using TonLibDotNet.Cells;

namespace TonLibDotNet.BlocksTlb
{
    /// <remarks>
    /// <code>
    /// acc_state_uninit$00   = AccountStatus;
    /// acc_state_frozen$01   = AccountStatus;
    /// acc_state_active$10   = AccountStatus;
    /// acc_state_nonexist$11 = AccountStatus;
    /// </code></remarks>
    public abstract class AccountStatus
    {
        public virtual bool IsUninit { get; } = false;

        public virtual bool IsFrozen { get; } = false;

        public virtual bool IsActive { get; } = false;

        public virtual bool IsNonexist { get; } = false;

        public static AccountStatus CreateFrom(Slice src)
        {
            return src.LoadUInt(2) switch
            {
                0 => Uninit.Instance,
                1 => Frozen.Instance,
                2 => Active.Instance,
                3 => Nonexist.Instance,
                _ => throw new InvalidOperationException(), // Can't get here because 2 bits are always between 0 and 3.
            };
        }

        public class Uninit : AccountStatus
        {
            public static readonly Uninit Instance = new();

            public override bool IsUninit => true;
        }

        public class Frozen : AccountStatus
        {
            public static readonly Frozen Instance = new();

            public override bool IsFrozen => true;
        }

        public class Active : AccountStatus
        {
            public static readonly Active Instance = new();

            public override bool IsActive => true;
        }

        public class Nonexist : AccountStatus
        {
            public static readonly Nonexist Instance = new();

            public override bool IsNonexist => true;
        }
    }
}
