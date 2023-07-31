using System.Buffers.Binary;
using TonLibDotNet.Cells;

namespace TonLibDotNet
{
    public static class TonClientCellsNumericExtensions
    {
        #region UShort and Short

        public static ushort LoadUShort(this Slice slice, int size = 16)
        {
            if (size < 1 || size > 16)
            {
                throw new ArgumentOutOfRangeException(nameof(size));
            }

            Span<byte> data = stackalloc byte[2];
            slice.LoadBitsToBytesTo(size, data);
            return BinaryPrimitives.ReadUInt16BigEndian(data);
        }

        public static ushort PreloadUShort(this Slice slice, int size = 16)
        {
            if (size < 1 || size > 16)
            {
                throw new ArgumentOutOfRangeException(nameof(size));
            }

            Span<byte> data = stackalloc byte[2];
            slice.PreloadBitsToBytesTo(size, data);
            return BinaryPrimitives.ReadUInt16BigEndian(data);
        }

        public static CellBuilder StoreUShort(this CellBuilder builder, ushort value, int size = 16)
        {
            if (size < 1 || size > 16)
            {
                throw new ArgumentOutOfRangeException(nameof(size));
            }

            Span<byte> data = stackalloc byte[2];
            BinaryPrimitives.WriteUInt16BigEndian(data, value);
            return builder.StoreBytes(data, size);
        }

        public static short LoadShort(this Slice slice, int size = 16)
        {
            if (size < 1 || size > 16)
            {
                throw new ArgumentOutOfRangeException(nameof(size));
            }

            Span<byte> data = stackalloc byte[2];
            slice.LoadBitsToBytesTo(size, data);
            return BinaryPrimitives.ReadInt16BigEndian(data);
        }

        public static short PreloadShort(this Slice slice, int size = 16)
        {
            if (size < 1 || size > 16)
            {
                throw new ArgumentOutOfRangeException(nameof(size));
            }

            Span<byte> data = stackalloc byte[2];
            slice.PreloadBitsToBytesTo(size, data);
            return BinaryPrimitives.ReadInt16BigEndian(data);
        }

        public static CellBuilder StoreShort(this CellBuilder builder, short value, int size = 16)
        {
            if (size < 1 || size > 16)
            {
                throw new ArgumentOutOfRangeException(nameof(size));
            }

            Span<byte> data = stackalloc byte[2];
            BinaryPrimitives.WriteInt16BigEndian(data, value);
            return builder.StoreBytes(data, size);
        }

        #endregion

        #region UInt and Int

        public static uint LoadUInt(this Slice slice, int size = 32)
        {
            if (size < 1 || size > 32)
            {
                throw new ArgumentOutOfRangeException(nameof(size));
            }

            Span<byte> data = stackalloc byte[4];
            slice.LoadBitsToBytesTo(size, data);
            return BinaryPrimitives.ReadUInt32BigEndian(data);
        }

        public static uint PreloadUInt(this Slice slice, int size = 32)
        {
            if (size < 1 || size > 32)
            {
                throw new ArgumentOutOfRangeException(nameof(size));
            }

            Span<byte> data = stackalloc byte[4];
            slice.PreloadBitsToBytesTo(size, data);
            return BinaryPrimitives.ReadUInt32BigEndian(data);
        }

        public static CellBuilder StoreUInt(this CellBuilder builder, uint value, int size = 32)
        {
            if (size < 1 || size > 32)
            {
                throw new ArgumentOutOfRangeException(nameof(size));
            }

            Span<byte> data = stackalloc byte[4];
            BinaryPrimitives.WriteUInt32BigEndian(data, value);
            return builder.StoreBytes(data, size);
        }

        public static int LoadInt(this Slice slice, int size = 32)
        {
            if (size < 1 || size > 32)
            {
                throw new ArgumentOutOfRangeException(nameof(size));
            }

            Span<byte> data = stackalloc byte[4];
            slice.LoadBitsToBytesTo(size, data);
            return BinaryPrimitives.ReadInt32BigEndian(data);
        }

        public static int PreloadInt(this Slice slice, int size = 32)
        {
            if (size < 1 || size > 32)
            {
                throw new ArgumentOutOfRangeException(nameof(size));
            }

            Span<byte> data = stackalloc byte[4];
            slice.PreloadBitsToBytesTo(size, data);
            return BinaryPrimitives.ReadInt32BigEndian(data);
        }

        public static CellBuilder StoreInt(this CellBuilder builder, int value, int size = 32)
        {
            if (size < 1 || size > 32)
            {
                throw new ArgumentOutOfRangeException(nameof(size));
            }

            Span<byte> data = stackalloc byte[4];
            BinaryPrimitives.WriteInt32BigEndian(data, value);
            return builder.StoreBytes(data, size);
        }

        #endregion

        #region ULong and long

        public static ulong LoadULong(this Slice slice, int size = 64)
        {
            if (size < 1 || size > 64)
            {
                throw new ArgumentOutOfRangeException(nameof(size));
            }

            Span<byte> data = stackalloc byte[8];
            slice.LoadBitsToBytesTo(size, data);
            return BinaryPrimitives.ReadUInt64BigEndian(data);
        }

        public static ulong PreloadULong(this Slice slice, int size = 64)
        {
            if (size < 1 || size > 64)
            {
                throw new ArgumentOutOfRangeException(nameof(size));
            }

            Span<byte> data = stackalloc byte[8];
            slice.PreloadBitsToBytesTo(size, data);
            return BinaryPrimitives.ReadUInt64BigEndian(data);
        }

        public static CellBuilder StoreULong(this CellBuilder builder, ulong value, int size = 64)
        {
            if (size < 1 || size > 64)
            {
                throw new ArgumentOutOfRangeException(nameof(size));
            }

            Span<byte> data = stackalloc byte[8];
            BinaryPrimitives.WriteUInt64BigEndian(data, value);
            return builder.StoreBytes(data, size);
        }

        public static long LoadLong(this Slice slice, int size = 64)
        {
            if (size < 1 || size > 64)
            {
                throw new ArgumentOutOfRangeException(nameof(size));
            }

            Span<byte> data = stackalloc byte[8];
            slice.LoadBitsToBytesTo(size, data);
            return BinaryPrimitives.ReadInt64BigEndian(data);
        }

        public static long PreloadLong(this Slice slice, int size = 64)
        {
            if (size < 1 || size > 64)
            {
                throw new ArgumentOutOfRangeException(nameof(size));
            }

            Span<byte> data = stackalloc byte[8];
            slice.PreloadBitsToBytesTo(size, data);
            return BinaryPrimitives.ReadInt64BigEndian(data);
        }

        public static CellBuilder StoreLong(this CellBuilder builder, long value, int size = 64)
        {
            if (size < 1 || size > 64)
            {
                throw new ArgumentOutOfRangeException(nameof(size));
            }

            Span<byte> data = stackalloc byte[8];
            BinaryPrimitives.WriteInt64BigEndian(data, value);
            return builder.StoreBytes(data, size);
        }

        #endregion
    }
}
