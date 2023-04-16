using System.Buffers.Binary;
using System.Linq;
using TonLibDotNet.Cells;

namespace TonLibDotNet
{
    public static class TonClientCellsGramsExtensions
    {
        /*
         tblchk.pdf
         3.1.5. Representing Gram currency amounts.
            var_uint$_ {n:#} len:(#< n) value:(uint (len * 8)) = VarUInteger n
            var_int$_ {n:#} len:(#< n) value:(int (len * 8)) = VarInteger n
            nanograms$_ amount:(VarUInteger 16) = Grams

        If one wants to represent x nanograms, one selects an integer ℓ < 16 such
            that x < 2^8ℓ, and serializes first ℓ as an unsigned 4-bit integer, then x itself
            as an unsigned 8ℓ-bit integer. Notice that four zero bits represent a zero
            amount of Grams.
         */
        public static ulong LoadGrams(this Slice slice)
        {
            Span<byte> length = stackalloc byte[1];
            slice.PreloadBitsToBytesTo(4, length);
            slice.EnsureCanLoad(4 + length[0] * 8);

            slice.SkipBits(4); // preloaded earlier

            if (length[0] == 0)
            {
                return 0;
            }

            return slice.LoadULong(length[0] * 8);
        }

        public static CellBuilder StoreGrams(this CellBuilder builder, ulong value)
        {
            if (value == 0)
            {
                builder.StoreBit(false);
                builder.StoreBit(false);
                builder.StoreBit(false);
                builder.StoreBit(false);
                return builder;
            }

            Span<byte> bytes = stackalloc byte[8];
            BinaryPrimitives.WriteUInt64BigEndian(bytes, value);
            var skip = 0;
            for (skip = 0; skip < bytes.Length; skip++)
            {
                if (bytes[skip] != 0)
                {
                    break;
                }
            }

            var usedBytes = bytes.Length - skip;
            builder.StoreInt(usedBytes, 4);
            builder.StoreULong(value, usedBytes * 8);

            return builder;
        }
    }
}
