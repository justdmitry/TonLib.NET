using TonLibDotNet.Cells;

namespace TonLibDotNet
{
    public static class TonClientCellsSugar
    {
        /// <summary>
        /// Shortcut for <code>Boc.ParseFromBase64(cell.Cell.Bytes)</code>
        /// </summary>
        public static Boc ToBoc(this Types.Tvm.StackEntryCell cell)
        {
            return Boc.ParseFromBase64(cell.Cell.Bytes);
        }

        /// <summary>
        /// Shortcut for <code>Boc.ParseFromBase64(cell.Bytes)</code>
        /// </summary>
        public static Boc ToBoc(this Types.Tvm.Cell cell)
        {
            return Boc.ParseFromBase64(cell.Bytes);
        }

        /// <summary>
        /// Memory-efficient alternative to <code>Convert.ToBase64String(boc.Serialize(...).ToArray())</code>
        /// </summary>
        public static string SerializeToBase64(this Boc boc, bool withCrc = true)
        {
            using var stream = boc.Serialize(withCrc);
            var buffer = stream.GetBuffer(); // does not create copy, but can contain unused bytes
            return Convert.ToBase64String(buffer[..(int)stream.Length]);
        }
    }
}
