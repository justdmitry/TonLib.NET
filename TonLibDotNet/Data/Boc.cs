using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace TonLibDotNet.Data
{
    /// <seealso href="https://docs.ton.org/develop/data-formats/cell-boc#packing-a-bag-of-cells">Cell & Bag of Cells(BoC)</seealso>
    /// <seealso href="https://docs.ton.org/ton.pdf">First useful document</seealso>
    /// <seealso href="https://docs.ton.org/tblkch.pdf">Second useful document</seealso>
    /// <seealso href="https://github.com/ton-community/ton-docs/tree/main/static">Other PDFs</seealso>
    public class Boc
    {
        protected const byte HeaderByte1 = 0xb5;
        protected const byte HeaderByte2 = 0xee;
        protected const byte HeaderByte3 = 0x9c;
        protected const byte HeaderByte4 = 0x72;

        public IReadOnlyList<Cell> RootCells { get; init; }

        public Boc(params Cell[] rootCells)
        {
            ArgumentNullException.ThrowIfNull(rootCells);

            if (rootCells.Length == 0)
            {
                throw new ArgumentException("Must have at least one cell", nameof(rootCells));
            }

            if (rootCells.Length != 1)
            {
                // Raise an issue with details how multi-root BOCs are stored.
                throw new NotImplementedException("Can't work with non-single root cell");
            }

            RootCells = rootCells.ToList().AsReadOnly();
        }

        public static bool TryParseFromBase64(string base64data, [NotNullWhen(true)] out Boc? boc)
        {
            return TryParseFromBytes(Convert.FromBase64String(base64data), out boc);
        }

        public static bool TryParseFromBytes(ReadOnlySpan<byte> bytes, [NotNullWhen(true)] out Boc? boc)
        {
            boc = null;

            int pos = 0;

            if (bytes.Length < 11)
            {
                throw new ArgumentOutOfRangeException(nameof(bytes), "Too little data: need at least 11 for correct header.");
            }

            if (bytes[pos++] != HeaderByte1
                || bytes[pos++] != HeaderByte2
                || bytes[pos++] != HeaderByte3
                || bytes[pos++] != HeaderByte4)
            {
                return false;
            }

            var flagAndSize = bytes[pos++];

            // number of bytes needed to store the number of cells
            var hasChecksum = (flagAndSize & 0x40) != 0;
            var bytesForNumberOfCells = flagAndSize & 0b111;
            if (bytesForNumberOfCells != 1)
            {
                // Raise an issue with details how to handle cell-refs in this case (because cell-ref is 1 byte only).
                throw new NotImplementedException("Can't handle more than 255 cells");
            }

            // number of bytes to store the size of the serialized cells
            var bytesForSizeOfCells = bytes[pos++];

            // number of cells (read only 1 byte, due to NotImplementedException() earlier)
            var numberOfCells = bytes[pos++];

            // number of root cells
            var numberOfRootCells = bytes[pos++];
            if (numberOfRootCells != 1)
            {
                // Raise an issue with details how multi-root BOCs are stored.
                throw new NotImplementedException("Can't work with non-single root cell");
            }

            // absent, always 0 (in current implementations)
            //// var absent = bytes[pos++];
            pos++;

            // size of serialized cells
            var sizeOfCells = bytesForSizeOfCells switch
            {
                1 => bytes[pos++],
                2 => bytes[pos++] << 8 | bytes[pos++],
                3 => bytes[pos++] << 16 | bytes[pos++] << 8 | bytes[pos++],
                4 => bytes[pos++] << 24 | bytes[pos++] << 16 | bytes[pos++] << 8 | bytes[pos++],
                _ => throw new NotImplementedException("Please add tests for such huge amount of data"),
            };

            // root cell index
            var rootCellIndex = bytes[pos++];

            var requiredLength = pos + sizeOfCells + (hasChecksum ? 4 : 0);
            if (bytes.Length != requiredLength)
            {
                throw new ArgumentOutOfRangeException(nameof(bytes), $"Invalid data length: expected {requiredLength} bytes, found only {bytes.Length}.");
            }

            if (hasChecksum)
            {
                var actualCrc = Crc32C.ComputeChecksum(bytes[..^4]);
                Span<byte> actualCrcBytes = stackalloc byte[4];
                BinaryPrimitives.WriteUInt32LittleEndian(actualCrcBytes, actualCrc);
                var expectedCrc = bytes[^4..];
                if (!expectedCrc.SequenceEqual(actualCrcBytes))
                {
                    throw new ArgumentOutOfRangeException(nameof(bytes), $"CRC does not match: expected {Convert.ToHexString(expectedCrc)}, found {Convert.ToHexString(actualCrcBytes)}.");
                }
            }

            var data = new (bool isOrdinary, int start, int length, bool isAugmented, byte[] links)[numberOfCells];
            for (var i = 0; i < numberOfCells; i++)
            {
                var d1 = bytes[pos++];
                var numberOfLinks = d1 & 0b111;
                var isOrdinary = (d1 & 0b1000) == 0;
                var d2 = bytes[pos++];
                var isAugmented = (d2 % 2) != 0;
                var bytesOfData = d2 / 2 + (isAugmented ? 1 : 0);
                var dataStart = pos;
                pos += bytesOfData;
                var refs = numberOfLinks == 0 ? Array.Empty<byte>() : bytes.Slice(pos, numberOfLinks).ToArray();
                pos += numberOfLinks;

                data[i] = (isOrdinary, dataStart, bytesOfData, isAugmented, refs);
            }

            var cells = new Cell[numberOfCells];
            for (var i = numberOfCells - 1; i >= 0; i--)
            {
                var item = data[i];
                if (!item.isOrdinary)
                {
                    continue;
                }

                var content = bytes.Slice(item.start, item.length);
                var refs = item.links.Select(x => cells[x]).ToArray();
                cells[i] = new Cell(content, item.isAugmented, refs);
            }

            boc = new Boc(cells[rootCellIndex]);
            return true;
        }

        public MemoryStream Serialize(bool withCrc)
        {
            if (RootCells.Count != 1)
            {
                // Raise an issue with details how multi-root BOCs are stored.
                throw new NotImplementedException("Can't work with non-single root cell");
            }

            static void AppendRefs(Cell cell, List<Cell> list)
            {
                foreach(var r in cell.Refs)
                {
                    if (list.Contains(r))
                    {
                        list.Remove(r);
                    }

                    list.Add(r);
                }

                foreach(var r in cell.Refs)
                {
                    AppendRefs(r, list);
                }
            }

            var list = new List<Cell>
            {
                RootCells[0]
            };
            AppendRefs(RootCells[0], list);

            if (list.Count > 255)
            {
                // Raise an issue with details how to handle cell-refs in this case (because cell-ref is 1 byte only).
                throw new NotImplementedException("Can't handle more than 255 cells");
            }

            var totalLength = 0;
            foreach(var c in list)
            {
                totalLength += 2;
                totalLength += c.Content.Length;
                totalLength += c.Refs.Count;
            }

            var ms = new MemoryStream();
            ms.WriteByte(HeaderByte1);
            ms.WriteByte(HeaderByte2);
            ms.WriteByte(HeaderByte3);
            ms.WriteByte(HeaderByte4);

            byte flagAndSize = 0x01; // 0 flags, 1 byte for cell count
            if (withCrc)
            {
                flagAndSize |= 0x40;
            }

            ms.WriteByte(flagAndSize);

            var totalLengthBytes = BitConverter.GetBytes(totalLength);
            var bytesForSizeOfCells = totalLengthBytes.Reverse().SkipWhile(x => x == 0).Count();
            ms.WriteByte((byte)bytesForSizeOfCells);

            var numberOfCells = (byte)list.Count;
            ms.WriteByte(numberOfCells);

            ms.WriteByte((byte)RootCells.Count);

            ms.WriteByte(0); // absent, always 0

            foreach(var b in totalLengthBytes.Take(bytesForSizeOfCells))
            {
                ms.WriteByte(b);
            }

            ms.WriteByte(0); // root cell index

            foreach(var cell in list)
            {
                ms.WriteByte((byte)cell.Refs.Count);
                ms.WriteByte((byte)(Math.Ceiling(cell.BitsCount / 8f) + Math.Floor(cell.BitsCount / 8f)));
                ms.Write(cell.Content);
                foreach(var r in cell.Refs)
                {
                    var index = (byte)list.IndexOf(r);
                    ms.WriteByte(index);
                }
            }

            if (withCrc)
            {
                var crc = Crc32C.ComputeChecksum(ms.ToArray());
                Span<byte> crcBytes = stackalloc byte[4];
                BinaryPrimitives.WriteUInt32LittleEndian(crcBytes, crc);
                ms.Write(crcBytes);
            }

            ms.Position = 0;
            return ms;
        }

        /// <summary>
        /// Dumps all Cells into string, like explorers do.
        /// </summary>
        public string DumpCells()
        {
            static void DumpImpl(StringBuilder sb, Cell cell, int deep)
            {
                sb.Append(new string(' ', deep + 2));
                sb.Append("x{");
                sb.Append(Convert.ToHexString(cell.Content));
                sb.AppendLine(cell.IsAugmented ? "_}" : "}");
                foreach(var r in cell.Refs)
                {
                    DumpImpl(sb, r, deep + 1);
                }
            }

            var sb = new StringBuilder();

            foreach (var rc in RootCells)
            {
                DumpImpl(sb, rc, 0);
            }

            return sb.ToString();
        }

        protected bool HasCycles(Cell cell, Cell[] visited)
        {
            foreach(var r in cell.Refs)
            {
                if (r == cell || visited.Contains(r))
                {
                    return true;
                }

                var newVisited = new Cell[visited.Length + 1];
                newVisited[0] = cell;
                visited.CopyTo(newVisited, 1);

                if (HasCycles(r, newVisited))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
