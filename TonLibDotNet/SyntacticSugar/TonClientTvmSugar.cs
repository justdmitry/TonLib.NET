using System.Numerics;
using TonLibDotNet.Types.Tvm;

namespace TonLibDotNet
{
    public static class TonClientTvmSugar
    {
        #region StackEntry values

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
        /// Shortcut for <code>(NumberDecimal)((StackEntryNumber)stackEntry).Number</code>
        /// </summary>
        public static NumberDecimal ToTvmNumberDecimal(this StackEntry stackEntry)
        {
            return (NumberDecimal)((StackEntryNumber)stackEntry).Number;
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

        #endregion

        #region Cell and Slice

        /// <summary>
        /// Converts content to <see cref="Cells.Boc"/>.
        /// </summary>
        /// <remarks>Shortcut for <code>Boc.ParseFromBase64(cell.Cell.Bytes)</code></remarks>
        public static Cells.Boc ToBoc(this StackEntryCell cell)
        {
            return Cells.Boc.ParseFromBase64(cell.Cell.Bytes);
        }

        /// <summary>
        /// Converts content to <see cref="Cells.Boc"/>.
        /// </summary>
        /// <remarks>Shortcut for <code>Boc.ParseFromBase64(cell.Bytes)</code></remarks>
        public static Cells.Boc ToBoc(this Cell cell)
        {
            return Cells.Boc.ParseFromBase64(cell.Bytes);
        }

        /// <summary>
        /// Converts content to <see cref="Cells.Boc"/>.
        /// </summary>
        /// <remarks>Shortcut for <code>Boc.ParseFromBase64(slice.Slice.Bytes)</code></remarks>
        public static Cells.Boc ToBoc(this StackEntrySlice slice)
        {
            return Cells.Boc.ParseFromBase64(slice.Slice.Bytes);
        }

        /// <summary>
        /// Converts content to <see cref="Cells.Boc"/>.
        /// </summary>
        /// <remarks>Shortcut for <code>Boc.ParseFromBase64(slice.Bytes)</code></remarks>
        public static Cells.Boc ToBoc(this Slice slice)
        {
            return Cells.Boc.ParseFromBase64(slice.Bytes);
        }

        /// <summary>
        /// Converts content to <see cref="Cells.Boc"/> if <paramref name="stackEntry"/> is instance of <see cref="StackEntryCell"/> or <see cref="StackEntrySlice"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">When type of <paramref name="stackEntry"/> is not supported.</exception>
        public static Cells.Boc ToBoc(this StackEntry stackEntry)
        {
            if (stackEntry is StackEntryCell cell)
            {
                return Cells.Boc.ParseFromBase64(cell.Cell.Bytes);
            }
            else if (stackEntry is StackEntrySlice slice)
            {
                return Cells.Boc.ParseFromBase64(slice.Slice.Bytes);
            }
            else
            {
                throw new InvalidOperationException("Only 'Cell' and 'Slice' stack entries are supported.");
            }
        }

        #endregion

        #region Number

        /// <summary>
        /// Converts content to <see cref="int"/> value if <paramref name="stackEntry"/> is instance of <see cref="StackEntryNumber"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">When type of <paramref name="stackEntry"/> is not supported.</exception>
        public static int ToInt(this StackEntry stackEntry) {
            if (stackEntry is StackEntryNumber number)
            {
                return ((NumberDecimal)number.Number).NumberAsInt();
            }
            else
            {
                throw new InvalidOperationException("Only 'Number' stack entries are supported.");
            }
        }

        /// <summary>
        /// Converts content to <see cref="long"/> value if <paramref name="stackEntry"/> is instance of <see cref="StackEntryNumber"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">When type of <paramref name="stackEntry"/> is not supported.</exception>
        public static long ToLong(this StackEntry stackEntry) {
            if (stackEntry is StackEntryNumber number)
            {
                return ((NumberDecimal)number.Number).NumberAsLong();
            }
            else
            {
                throw new InvalidOperationException("Only 'Number' stack entries are supported.");
            }
        }

        /// <summary>
        /// Converts content to <see cref="BigInteger"/> value if <paramref name="stackEntry"/> is instance of <see cref="StackEntryNumber"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">When type of <paramref name="stackEntry"/> is not supported.</exception>
        public static BigInteger ToBigInteger(this StackEntry stackEntry) {
            if (stackEntry is StackEntryNumber number)
            {
                return ((NumberDecimal)number.Number).NumberAsBigInteger();
            }
            else
            {
                throw new InvalidOperationException("Only 'Number' stack entries are supported.");
            }
        }

        /// <summary>
        /// Converts content to <see cref="BigInteger"/> and then to <see cref="byte">byte array</see> if <paramref name="stackEntry"/> is instance of <see cref="StackEntryNumber"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">When type of <paramref name="stackEntry"/> is not supported.</exception>
        public static byte[] ToBigIntegerBytes(this StackEntry stackEntry) {
            if (stackEntry is StackEntryNumber number)
            {
                return ((NumberDecimal)number.Number).NumberAsBigIntegerBytes();
            }
            else
            {
                throw new InvalidOperationException("Only 'Number' stack entries are supported.");
            }
        }

        #endregion
    }
}
