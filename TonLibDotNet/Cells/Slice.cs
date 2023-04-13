namespace TonLibDotNet.Cells
{
    public class Slice
    {
        private readonly ArraySegment<bool> data;

        private int index;

        public Slice(ArraySegment<bool> data)
        {
            this.data = data;
            this.index = 0;
            this.Length = data.Count;
        }

        public int Length { get; init; }

        public void EndRead()
        {
            if (index < Length)
            {
                throw new InvalidOperationException("Have more data");
            }
        }

        public bool TryCanLoad(int count)
        {
            if (count <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "Must be positive");
            }

            return index + count <= data.Count;
        }

        public void EnsureCanLoad(int count)
        {
            if (!TryCanLoad(count))
            {
                throw new InvalidOperationException("No enough data");
            }
        }

        public void SkipBits(int count)
        {
            EnsureCanLoad(count);
            index += count;
        }

        public bool LoadBit()
        {
            EnsureCanLoad(1);
            return data[index++];
        }

        public bool PreloadBit()
        {
            EnsureCanLoad(1);
            return data[index]; // no increment!
        }

        public ReadOnlySpan<bool> LoadBits(int count)
        {
            EnsureCanLoad(count);
            var res = data.Slice(index, count);
            index += count;
            return res;
        }

        public ReadOnlySpan<bool> PreloadBits(int count)
        {
            EnsureCanLoad(count);
            return data.Slice(index, count);
        }

        public void LoadBitsTo(Span<bool> buffer)
        {
            EnsureCanLoad(buffer.Length);
            ((ReadOnlySpan<bool>)data.Slice(index, buffer.Length)).CopyTo(buffer);
            index += buffer.Length;
        }

        public void PreloadBitsTo(Span<bool> buffer)
        {
            EnsureCanLoad(buffer.Length);
            ((ReadOnlySpan<bool>)data.Slice(index, buffer.Length)).CopyTo(buffer);
        }

        public Slice LoadSlice(int bitsCount)
        {
            EnsureCanLoad(bitsCount);
            var res = new ArraySegment<bool>(data.Array!, data.Offset + index, bitsCount);
            index += bitsCount;
            return new Slice(res);
        }

        public Slice PreloadSlice(int bitsCount)
        {
            EnsureCanLoad(bitsCount);
            var res = new ArraySegment<bool>(data.Array!, data.Offset + index, bitsCount);
            return new Slice(res);
        }

        public byte LoadByte()
        {
            EnsureCanLoad(8);
            var b = data[index++] ? 1 : 0;
            for (var i = 0; i < 7; i++)
            {
                b = (b << 1) | (data[index++] ? 1 : 0);
            }
            return (byte)(b & 0xFF);
        }

        public byte PreloadByte()
        {
            var res = LoadByte();
            index -= 8;
            return res;
        }

        public void LoadBitsToBytesTo(int bitCount, Span<byte> buffer)
        {
            EnsureCanLoad(bitCount);

            var totalBits = buffer.Length << 3;
            if (totalBits < bitCount)
            {
                throw new ArgumentException("Buffer is too small", nameof(buffer));
            }

            var extraBits = totalBits - bitCount;
            var currentByte = 0;
            var byteIndex = extraBits >> 3;
            for (var j = extraBits; j < totalBits; j++)
            {
                currentByte = (currentByte << 1) | (data[index++] ? 1 : 0);

                if ((j & 0b111) == 0b111)
                {
                    buffer[byteIndex++] = (byte)(currentByte & 0xFF);
                    currentByte = 0;
                }
            }
        }

        public void PreloadBitsToBytesTo(int bitCount, Span<byte> buffer)
        {
            LoadBitsToBytesTo(bitCount, buffer);
            index -= bitCount;
        }

        public byte[] LoadBitsToBytes(int bitCount)
        {
            EnsureCanLoad(bitCount);
            var byteCount = (bitCount >> 3) + Math.Sign(bitCount & 0b111);
            var buffer = new byte[byteCount];
            LoadBitsToBytesTo(bitCount, buffer);
            return buffer;
        }

        public byte[] PreloadBitsToBytes(int bitCount)
        {
            var buffer = LoadBitsToBytes(bitCount);
            index -= bitCount;
            return buffer;
        }

        public byte[] LoadBytes(int byteCount)
        {
            return LoadBitsToBytes(byteCount << 3);
        }

        public byte[] PreloadBytes(int byteCount)
        {
            return PreloadBitsToBytes(byteCount << 3);
        }

        public void LoadBytesTo(Span<byte> bytes)
        {
            LoadBitsToBytesTo(bytes.Length << 3, bytes);
        }

        public void PreloadBytesTo(Span<byte> bytes)
        {
            PreloadBitsToBytesTo(bytes.Length << 3, bytes);
        }
    }
}
