using TonLibDotNet.Cells;

namespace TonLibDotNet.BlocksTlb
{
    /// <remarks>
    /// <code>
    /// acst_unchanged$0 = AccStatusChange;  // x -> x
    /// acst_frozen$10   = AccStatusChange;  // init -> frozen
    /// acst_deleted$11  = AccStatusChange;  // frozen -> deleted
    /// </code></remarks>
    public abstract class AccStatusChange
    {
        public virtual bool IsUnchanged { get; } = false;

        public virtual bool IsFrozen { get; } = false;

        public virtual bool IsDeleted { get; } = false;

        public static AccStatusChange CreateFrom(Slice src)
        {
            if (src.LoadBit())
            {
                return src.LoadBit() ? Deleted.Instance : Frozen.Instance;
            }
            else
            {
                return Unchanged.Instance;
            }
        }

        public class Unchanged : AccStatusChange
        {
            public static readonly Unchanged Instance = new();

            public override bool IsUnchanged => true;
        }

        public class Frozen: AccStatusChange
        {
            public static readonly Frozen Instance = new();

            public override bool IsFrozen => true;
        }

        public class Deleted : AccStatusChange
        {
            public static readonly Deleted Instance = new();

            public override bool IsDeleted => true;
        }
    }
}
