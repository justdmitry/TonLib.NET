using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using TonLibDotNet.Utils;

namespace TonLibDotNet.Cells
{
    /// <seealso href="https://github.com/ton-community/ton-docs/tree/main/static">Other PDFs</seealso>
    /*
     tblchk.pdf
     5.3.9. TL-B scheme for serializing bags of cells
        serialized_boc#b5ee9c72 has_idx:(## 1) has_crc32c:(## 1)
            has_cache_bits:(## 1) flags:(## 2) { flags = 0 }
            size:(## 3) { size <= 4 }
            off_bytes:(## 8) { off_bytes <= 8 }
            cells:(##(size * 8))
            roots:(##(size * 8)) { roots >= 1 }
            absent:(##(size * 8)) { roots + absent <= cells }
            tot_cells_size:(##(off_bytes * 8))
            root_list:(roots * ##(size * 8))
            index:has_idx?(cells * ##(off_bytes * 8))
            cell_data:(tot_cells_size * [ uint8 ])
            crc32c:has_crc32c?uint32
            = BagOfCells
     */
    public class Boc
    {
        protected const byte HeaderByte1 = 0xb5;
        protected const byte HeaderByte2 = 0xee;
        protected const byte HeaderByte3 = 0x9c;
        protected const byte HeaderByte4 = 0x72;

        protected const byte FlagHasIndex = 0b1_0_0_00_000;
        protected const byte FlagHasCrc = 0b0_1_0_00_000;

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

        public static bool TryParseFromHex(string hex, [NotNullWhen(true)] out Boc? boc)
        {
            return TryParseFromBytes(Convert.FromHexString(hex), out boc);
        }

        public static bool TryParseFromBytes(ReadOnlySpan<byte> bytes, [NotNullWhen(true)] out Boc? boc)
        {
            (boc, _) = TryParseImpl(bytes);
            return (boc != null);
        }

        public static Boc ParseFromBase64(string base64data)
        {
            return ParseFromBytes(Convert.FromBase64String(base64data));
        }

        public static Boc ParseFromHex(string hex)
        {
            return ParseFromBytes(Convert.FromHexString(hex));
        }

        public static Boc ParseFromBytes(ReadOnlySpan<byte> bytes)
        {
            var (boc, err) = TryParseImpl(bytes);

            if (boc == null)
            {
                throw new BocParseException(err);
            }

            return boc;
        }

        public MemoryStream Serialize(bool withCrc)
        {
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

            var list = new List<Cell>(RootCells);
            foreach (var rc in RootCells)
            {
                AppendRefs(rc, list);
            }

            var bytesForCellIndexes = GetBytesToStore(list.Count);

            var totalLength = 0;
            foreach(var c in list)
            {
                totalLength += 2;
                totalLength += c.Content.Length;
                totalLength += bytesForCellIndexes * c.Refs.Count;
            }

            var ms = new MemoryStream();
            ms.WriteByte(HeaderByte1);
            ms.WriteByte(HeaderByte2);
            ms.WriteByte(HeaderByte3);
            ms.WriteByte(HeaderByte4);

            byte flagAndSize = bytesForCellIndexes;
            if (withCrc)
            {
                flagAndSize |= FlagHasCrc;
            }

            ms.WriteByte(flagAndSize);

            // off_bytes
            var bytesForSizeOfCells = GetBytesToStore(totalLength);
            ms.WriteByte(bytesForSizeOfCells);

            // cells
            WriteValue(ms, list.Count, bytesForCellIndexes);

            // roots
            WriteValue(ms, RootCells.Count, bytesForCellIndexes);

            // absent
            WriteValue(ms, 0, bytesForCellIndexes);

            // tot_cells_size
            WriteValue(ms, totalLength, bytesForSizeOfCells);

            foreach(var rc in RootCells)
            {
                var idx = list.IndexOf(rc);
                WriteValue(ms, idx, bytesForCellIndexes);
            }

            foreach(var cell in list)
            {
                ms.WriteByte((byte)cell.Refs.Count);
                ms.WriteByte((byte)(Math.Ceiling(cell.BitsCount / 8f) + Math.Floor(cell.BitsCount / 8f)));
                ms.Write(cell.Content);
                foreach(var r in cell.Refs)
                {
                    var index = (byte)list.IndexOf(r);
                    WriteValue(ms, index, bytesForCellIndexes);
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

        protected static (Boc?, string? error) TryParseImpl(ReadOnlySpan<byte> bytes)
        {
            int pos = 0;

            if (bytes.Length < 11)
            {
                return (null, "Too little data: need at least 11 for correct header.");
            }

            if (bytes[pos++] != HeaderByte1
                || bytes[pos++] != HeaderByte2
                || bytes[pos++] != HeaderByte3
                || bytes[pos++] != HeaderByte4)
            {
                return (null, "Not a BOC (invalid header bytes).");
            }

            var flagAndSize = bytes[pos++];

            var hasIndex = (flagAndSize & FlagHasIndex) != 0;
            var hasChecksum = (flagAndSize & FlagHasCrc) != 0;
            var bytesForNumberOfCells = (byte)(flagAndSize & 0b111);

            // number of bytes to store the size of the serialized cells
            var bytesForSizeOfCells = bytes[pos++];

            // number of cells (read only 1 byte, due to NotImplementedException() earlier)
            var numberOfCells = ReadValue(bytes, ref pos, bytesForNumberOfCells);

            // number of root cells
            var numberOfRootCells = ReadValue(bytes, ref pos, bytesForNumberOfCells);

            // absent, always 0 (in current implementations)
            _ = ReadValue(bytes, ref pos, bytesForNumberOfCells);

            // size of serialized cells
            var sizeOfCells = ReadValue(bytes, ref pos, bytesForSizeOfCells);

            // root cell index
            var rootCellIndexes = new int[numberOfRootCells];
            for (var i = 0; i < numberOfRootCells; i++)
            {
                rootCellIndexes[i] = ReadValue(bytes, ref pos, bytesForNumberOfCells);
            }

            if (hasIndex)
            {
                pos += numberOfCells * bytesForSizeOfCells;
            }

            var requiredLength = pos + sizeOfCells + (hasChecksum ? 4 : 0);
            if (bytes.Length != requiredLength)
            {
                return (null, $"Invalid data length: expected {requiredLength} bytes, found {bytes.Length}.");
            }

            if (hasChecksum)
            {
                var actualCrc = Crc32C.ComputeChecksum(bytes[..^4]);
                Span<byte> actualCrcBytes = stackalloc byte[4];
                BinaryPrimitives.WriteUInt32LittleEndian(actualCrcBytes, actualCrc);
                var expectedCrc = bytes[^4..];
                if (!expectedCrc.SequenceEqual(actualCrcBytes))
                {
                    return (null, $"CRC does not match: expected {Convert.ToHexString(expectedCrc)}, found {Convert.ToHexString(actualCrcBytes)}.");
                }
            }

            var data = new (bool isOrdinary, int start, int length, bool isAugmented, int[] links)[numberOfCells];
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

                var refs = new int[numberOfLinks];
                for (var j = 0; j < numberOfLinks; j++)
                {
                    refs[j] = ReadValue(bytes, ref pos, bytesForNumberOfCells);
                }

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

            var rootCells = rootCellIndexes.Select(x => cells[x]).ToArray();
            return (new Boc(rootCells), null);
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

        private static byte GetBytesToStore(int value)
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            if (value <= 0xFF)
            {
                return 1;
            }

            if (value <= 0xFFFF)
            {
                return 2;
            }

            if (value <= 0xFFFFFF)
            {
                return 3;
            }

            return 4;
        }

        private static void WriteValue(MemoryStream ms, int value, byte len)
        {
            Span<byte> bytes = stackalloc byte[4];
            BinaryPrimitives.WriteInt32BigEndian(bytes, value);
            ms.Write(bytes[^len..]);
        }

        private static int ReadValue(ReadOnlySpan<byte> bytes, ref int pos, byte len)
        {
            if (len <= 0 || len > 4)
            {
                throw new ArgumentOutOfRangeException(nameof(len));
            }

            Span<byte> buf = stackalloc byte[4];
            bytes.Slice(pos, len).CopyTo(buf[(4-len)..]);

            pos += len;

            return BinaryPrimitives.ReadInt32BigEndian(buf);
        }
    }
}
