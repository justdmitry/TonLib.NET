using System.Buffers.Binary;
using System.Numerics;
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

        /// <summary>
        /// Load 'coins' amount as 'long' value.
        /// </summary>
        /// <remarks>It's safe to load coins into 'long' while total TON supply is unchanged (5*10^18), details are in <see href="https://t.me/tondev/122940"/>.</remarks>
        /// <exception cref="InvalidOperationException">When actual stored value is larger than 'long' (it's not amount of TON coins in that case).</exception>
        /// <seealso href="https://t.me/tondev/122940">Explanation (in RUS) why 120bits are used to store 63bits-max values.</seealso>
        public static long LoadCoins(this Slice slice)
        {
            Span<byte> bytes = stackalloc byte[8];

            if (!TryLoadCoinsToBytes(slice, bytes, out var length))
            {
                throw new InvalidOperationException($"Can't load {length} actual bytes to 'long'. Use LoadCoinsToBigInt() instead.");
            }

            var value = BinaryPrimitives.ReadInt64BigEndian(bytes);

            if (value < 0)
            {
                throw new InvalidOperationException($"Negaive value loaded. Use LoadCoinsToULong() instead.");
            }

            return value;
        }

        /// <summary>
        /// Load 'coins' amount as 'ulong' value. Use this if you prefer 'ulong' over 'long' in your app.
        /// </summary>
        /// <remarks>It's safe to load coins into 'long' while total TON supply is unchanged (5*10^18), details are in <see href="https://t.me/tondev/122940"/>.</remarks>
        /// <exception cref="InvalidOperationException">When actual stored value is larger than 'ulong' (it's not amount of TON coins in that case).</exception>
        /// <seealso href="https://t.me/tondev/122940">Explanation (in RUS) why 120bits are used to store 63bits-max values.</seealso>
        public static ulong LoadCoinsToULong(this Slice slice)
        {
            Span<byte> bytes = stackalloc byte[8];

            if (!TryLoadCoinsToBytes(slice, bytes, out var length))
            {
                throw new InvalidOperationException($"Can't load {length} actual bytes to 'ulong'. Use other LoadCoinsToBigInt()");
            }

            return BinaryPrimitives.ReadUInt64BigEndian(bytes);
        }

        /// <summary>
        /// Load 'coins' amount as 'BigInteger' value.
        /// </summary>
        /// <remarks>TON supply (5*10^18) fits into 'long' value (details are in <see href="https://t.me/tondev/122940"/>), so this function may be used to load other (jetton?) amounts.</remarks>
        public static BigInteger LoadCoinsToBigInt(this Slice slice)
        {
            Span<byte> bytes = stackalloc byte[15];

            if (!TryLoadCoinsToBytes(slice, bytes, out var length))
            {
                throw new Exception($"Failed to load {length} bytes as Grams");
            }

            if (length == 0)
            {
                return BigInteger.Zero;
            }

            return new BigInteger(bytes, true, true);
        }

        /// <summary>
        /// Store 'coins' amount given by 'long' value (negative values are not allowed).
        /// </summary>
        /// <remarks>It's safe to store coins as 'long' while total TON supply is unchanged (5*10^18), details are in <see href="https://t.me/tondev/122940"/>.</remarks>
        /// <exception cref="ArgumentOutOfRangeException">When 'value' is negative.</exception>
        /// <seealso href="https://t.me/tondev/122940">Explanation (in RUS) why 120bits are used to store 63bits-max values.</seealso>
        public static CellBuilder StoreCoins(this CellBuilder builder, long value)
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Can't be negative");
            }

            if (value == 0)
            {
                return builder.StoreBit(false).StoreBit(false).StoreBit(false).StoreBit(false);
            }

            Span<byte> bytes = stackalloc byte[8];
            BinaryPrimitives.WriteInt64BigEndian(bytes, value);
            return StoreGramsBytes(builder, bytes);
        }

        /// <summary>
        /// Store 'coins' amount given by 'ulong' value. Use this if you prefer 'ulong' over 'long' in your app.
        /// </summary>
        /// <remarks>It's safe to store coins as 'long' while total TON supply is unchanged (5*10^18), details are in <see href="https://t.me/tondev/122940"/>.</remarks>
        /// <seealso href="https://t.me/tondev/122940">Explanation (in RUS) why 120bits are used to store 63bits-max values.</seealso>
        public static CellBuilder StoreCoins(this CellBuilder builder, ulong value)
        {
            if (value == 0)
            {
                return builder.StoreBit(false).StoreBit(false).StoreBit(false).StoreBit(false);
            }

            Span<byte> bytes = stackalloc byte[8];
            BinaryPrimitives.WriteUInt64BigEndian(bytes, value);
            return StoreGramsBytes(builder, bytes);
        }

        /// <summary>
        /// Store 'coins' amount given by 'BigInteger' value. Use this if you store coins other than TON (jettons?).
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">When 'value' is negative, or when 'value' is too big and need more than 15 bytes to store.</exception>
        /// <seealso href="https://t.me/tondev/122940">Explanation (in RUS) why 120bits are used to store 63bits-max values.</seealso>
        public static CellBuilder StoreCoins(this CellBuilder builder, BigInteger value)
        {
            if (value.Sign == -1)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Can't be negative");
            }

            if (value.IsZero)
            {
                return builder.StoreBit(false).StoreBit(false).StoreBit(false).StoreBit(false);
            }

            var length = value.GetByteCount(true);
            if (length > 15)
            {
                throw new ArgumentOutOfRangeException(nameof(value), $"Value is too big: {length} bytes needed, but only 15 allowed.");
            }

            Span<byte> bytes = stackalloc byte[length];
            if (!value.TryWriteBytes(bytes, out _, true, true))
            {
                throw new Exception($"Failed to store {length} bytes as Grams");
            }

            return StoreGramsBytes(builder, bytes);
        }

        private static CellBuilder StoreGramsBytes(this CellBuilder builder, ReadOnlySpan<byte> value)
        {
            var skip = 0;
            for (skip = 0; skip < value.Length; skip++)
            {
                if (value[skip] != 0)
                {
                    break;
                }
            }

            builder.StoreInt(value.Length - skip, 4);
            builder.StoreBytes(value[skip..]);

            return builder;
        }

        private static bool TryLoadCoinsToBytes(this Slice slice, Span<byte> buffer, out byte length)
        {
            Span<byte> lengthBytes = stackalloc byte[1];
            slice.PreloadBitsToBytesTo(4, lengthBytes);

            length = lengthBytes[0];

            if (buffer.Length < length)
            {
                return false;
            }

            slice.EnsureCanLoad(4 + length * 8);

            slice.SkipBits(4); // preloaded earlier

            if (length == 0)
            {
                // zero grams
                return true;
            }

            slice.LoadBitsToBytesTo(length * 8, buffer);
            return true;
        }
    }
}
