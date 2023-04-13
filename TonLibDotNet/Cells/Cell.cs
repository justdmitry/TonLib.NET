namespace TonLibDotNet.Cells
{
    public class Cell
    {
        public const int MaxContentLength = 128;
        public const int MaxBitsCount = 1023;
        public const int MaxRefs = 4;

        public Cell(ReadOnlySpan<byte> content, bool isAugmented, ICollection<Cell>? refs = null)
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
        }

        public byte[] Content { get; init; }

        public bool IsAugmented { get; init; }

        public IReadOnlyList<Cell> Refs { get; protected set; }

        public int BitsCount { get; protected set; }

        public Slice BeginRead()
        {
            var bits = new bool[BitsCount];

            for(var i = 0; i < BitsCount; i++)
            {
                bits[i] = (Content[i / 8] & (0b1000_0000 >> (i & 0b111))) != 0;
            }

            return new Slice(new ArraySegment<bool>(bits, 0, BitsCount));
        }
    }
}