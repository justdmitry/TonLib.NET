using TonLibDotNet.Cells;

namespace TonLibDotNet
{
    public static class TonClientCellsSugar
    {
        /// <summary>
        /// Memory-efficient alternative to <code>Convert.ToBase64String(boc.Serialize(...).ToArray())</code>
        /// </summary>
        public static string SerializeToBase64(this Boc boc, bool withCrc = true)
        {
            using var stream = boc.Serialize(withCrc);
            var buffer = stream.GetBuffer(); // does not create copy, but can contain unused bytes
            return Convert.ToBase64String(buffer[..(int)stream.Length]);
        }

        /// <summary>
        /// Shortcut for <code>new Boc(cell)</code>.
        /// </summary>
        public static Boc ToBoc(this Cell cell)
        {
            return new Boc(cell);
        }
    }
}
