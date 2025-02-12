using System.Buffers.Binary;
using System.Security.Cryptography;

namespace TonLibDotNet.Cells
{
    public class Cell
    {
        public const int MaxContentLength = 128;
        public const int MaxBitsCount = 1023;
        public const int MaxRefs = 4;

        public Cell(ReadOnlySpan<byte> content, bool isAugmented, ICollection<Cell>? refs = null)
            : this(false, refs?.Max(x => x.Level) ?? 0, content, isAugmented, refs)
        {
            // Nothing.
        }

        public Cell(bool isExotic, byte level, ReadOnlySpan<byte> content, bool isAugmented, ICollection<Cell>? refs = null)
        {
            if (content.Length > MaxContentLength)
            {
                throw new ArgumentOutOfRangeException(nameof(content), $"Too many bytes ({content.Length}), no more than {MaxContentLength} are allowed");
            }

            if (isAugmented && content[^1] == 0x00)
            {
                throw new ArgumentOutOfRangeException(nameof(isAugmented), "Last byte is 0x00. Either remove extra bytes, or set isAugmented to false.");
            }

            if (refs != null && refs.Count > MaxRefs)
            {
                throw new ArgumentOutOfRangeException(nameof(refs), $"Too many refs ({refs.Count}), only {MaxRefs} are allowed.");
            }

            this.IsExotic = isExotic;
            this.Level = level;
            this.Content = content.ToArray();
            BitsCount = this.Content.Length * 8;
            if (isAugmented)
            {
                var lastByte = content[^1];
                for (var i = 0; i < 8; i++)
                {
                    BitsCount--;
                    if ((lastByte & (1 << i)) != 0)
                    {
                        break;
                    }
                }
            }

            if (BitsCount > MaxBitsCount)
            {
                throw new ArgumentOutOfRangeException(nameof(content), $"Too many bits found ({BitsCount}), no more than {MaxBitsCount} are allowed");
            }

            this.IsAugmented = isAugmented;

            this.Refs = refs?.ToList().AsReadOnly() ?? new List<Cell>().AsReadOnly();
            this.Depth = Refs.Count == 0 ? (short)0 : (short)(Refs.Max(x => x.Depth) + 1);
        }

        public bool IsExotic { get; init; }

        public byte Level { get; init; }

        public short Depth { get; init; }

        public byte[] Content { get; init; }

        public bool IsAugmented { get; init; }

        public IReadOnlyList<Cell> Refs { get; protected set; }

        public int BitsCount { get; protected set; }

        private byte[]? hashValue;

        public static (byte refCount, bool isExotic, byte level, int dataLength, bool isAugmented) ParseDescriptors(byte d1, byte d2)
        {
            var refCount = d1 & 0b111;
            var isExotic = (d1 & 0b1000) != 0;
            var level = d1 / 32;
            var isAugmented = (d2 % 2) != 0;
            var dataLength = d2 / 2 + (isAugmented ? 1 : 0);
            return ((byte)refCount, isExotic, (byte)level, dataLength, isAugmented);
        }

        /// <summary>
        /// Builds cell d1 and d2 descriptors.
        /// </summary>
        /// <remarks>
        /// <para>tvm.pdf, 3.1.4. Standard cell representation:</para>
        /// <para>
        /// Byte d1 equals r + 8s + 32l, where 0 ≤ r ≤ 4 is the quantity of cell references contained
        ///   in the cell, 0 ≤ l ≤ 3 is the level of the cell, and 0 ≤ s ≤ 1 is 1 for
        ///   exotic cells and 0 for ordinary cells.
        /// </para>
        /// <para>Byte d2 equals ⌊b / 8⌋+⌈b / 8⌉, where 0 ≤ b ≤ 1023 is the quantity of data bits in c.</para>
        /// </remarks>
        public (byte d1, byte d2) GetDescriptors()
        {
            var d1 = 32 * Level + (IsExotic ? 8 : 0) + Refs.Count;
            var d2 = Math.Ceiling(BitsCount / 8f) + Math.Floor(BitsCount / 8f);
            return ((byte)d1, (byte)d2);
        }

        public Slice BeginRead()
        {
            var bits = new bool[BitsCount];

            for (var i = 0; i < BitsCount; i++)
            {
                bits[i] = (Content[i / 8] & (0b1000_0000 >> (i & 0b111))) != 0;
            }

            return new Slice(new ArraySegment<bool>(bits, 0, BitsCount), Refs);
        }

        public byte[] Hash()
        {
            if (hashValue == null)
            {
                var bytes = new byte[2 + Content.Length + Refs.Count * (2 + 32)];

                var (d1, d2) = GetDescriptors();
                bytes[0] = d1;
                bytes[1] = d2;
                Content.CopyTo(bytes, 2);

                var pos = 2 + Content.Length;

                Span<byte> depth = stackalloc byte[2];
                foreach (var r in Refs)
                {
                    BinaryPrimitives.WriteInt16BigEndian(depth, r.Depth);
                    depth.CopyTo(bytes.AsSpan(pos, 2));
                    pos += 2;
                }

                foreach (var r in Refs)
                {
                    r.Hash().CopyTo(bytes, pos);
                    pos += 32;
                }

                hashValue = SHA256.HashData(bytes);
            }

            return hashValue;
        }
    }
}