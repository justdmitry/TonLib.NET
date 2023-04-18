using TonLibDotNet.Types.Tvm;

namespace TonLibDotNet
{
    public static class TonClientTvmSugar
    {
        /// <summary>
        /// Shortcut for <code>((StackEntryCell)stackEntry).Cell</code>
        /// </summary>
        public static Cell ToTvmCell(this StackEntry stackEntry)
        {
            return ((StackEntryCell)stackEntry).Cell;
        }

        /// <summary>
        /// Shortcut for <code>((StackEntryList)stackEntry).List</code>
        /// </summary>
        public static List ToTvmList(this StackEntry stackEntry)
        {
            return ((StackEntryList)stackEntry).List;
        }

        /// <summary>
        /// Shortcut for <code>((NumberDecimal)((StackEntryNumber)stackEntry).Number).Number</code>
        /// </summary>
        public static string ToTvmNumberDecimal(this StackEntry stackEntry)
        {
            return ((NumberDecimal)((StackEntryNumber)stackEntry).Number).Number;
        }

        /// <summary>
        /// Shortcut for <code>((StackEntrySlice)stackEntry).Slice</code>
        /// </summary>
        public static Slice ToTvmSlice(this StackEntry stackEntry)
        {
            return ((StackEntrySlice)stackEntry).Slice;
        }

        /// <summary>
        /// Shortcut for <code>((StackEntryTuple)stackEntry).Tuple</code>
        /// </summary>
        public static Types.Tvm.Tuple ToTvmTuple(this StackEntry stackEntry)
        {
            return ((StackEntryTuple)stackEntry).Tuple;
        }
    }
}
