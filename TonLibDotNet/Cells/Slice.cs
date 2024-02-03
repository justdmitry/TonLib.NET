namespace TonLibDotNet.Cells
{
    public class Slice
    {
        private readonly ArraySegment<bool> data;
        private readonly IReadOnlyList<Cell>? refs;

        private int index;
        private int refIndex;

        public Slice(ArraySegment<bool> data, IReadOnlyList<Cell>? refs = null)
        {
            this.data = data;
            this.index = 0;
            this.refs = refs;
            this.refIndex = 0;
        }

        public int Length => data.Count - index;

        public void EndRead()
        {
            if (Length > 0)
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

            return count <= Length;
        }

        public void EnsureCanLoad(int count)
        {
            if (!TryCanLoad(count))
            {
                throw new InvalidOperationException("No enough data");
            }
        }

        public bool TryCanLoadRef()
        {
            return refs != null && refIndex < refs.Count;
        }

        public void EnsureCanLoadRef()
        {
            if (!TryCanLoadRef())
            {
                throw new InvalidOperationException("No enough refs");
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
            for (var i = extraBits; i < totalBits; i++)
            {
                currentByte = (currentByte << 1) | (data[index++] ? 1 : 0);

                if ((i & 0b111) == 0b111)
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

        public Cell LoadRef()
        {
            EnsureCanLoadRef();
            return refs![refIndex++];
        }

        public Cell? TryLoadRef()
        {
            return TryCanLoadRef() ? refs![refIndex++] : null;
        }

        public Cell PreloadRef()
        {
            EnsureCanLoadRef();
            return refs![refIndex];
        }

        public void SkipRef()
        {
            EnsureCanLoadRef();
            refIndex++;
        }
    }
}
