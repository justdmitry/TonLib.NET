using System.Text;
using TonLibDotNet.Cells;

namespace TonLibDotNet
{
    public static class TonClientCellsStringExtensions
    {
        /// <summary>
        /// Loads remaining content of Slice and tries to convert it to UTF-8 string.
        /// </summary>
        /// <param name="slice">Slice to load string from.</param>
        /// <returns>Decoded string (or null, if slice is empty).</returns>
        /// <exception cref="ArgumentOutOfRangeException">When remaining content in Slice has invalid number of bits (not a multiple of 8).</exception>
        public static string? LoadString(this Slice slice)
        {
            if (slice.Length == 0)
            {
                return null;
            }

            if ((slice.Length & 0b111) != 0)
            {
                throw new ArgumentOutOfRangeException($"Can't load {slice.Length} bits to bytes", nameof(slice));
            }

            var byteCount = slice.Length / 8;
            Span<byte> bytes = stackalloc byte[byteCount];
            slice.LoadBitsToBytesTo(slice.Length, bytes);

            return Encoding.UTF8.GetString(bytes);
        }

        /// <summary>
        /// Saves string as UTF-8 byte sequence.
        /// </summary>
        /// <param name="builder">Builder to store string at.</param>
        /// <param name="value">String to store.</param>
        /// <remarks>
        /// Method will fail if string is too long and doesn't fit into current cell builder. Use <see cref="StoreStringSnake">StoreStringSnake</see> to store long string in a chain of cells.
        /// </remarks>
        public static CellBuilder StoreString(this CellBuilder builder, string? value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return builder;
            }

            var byteCount = Encoding.UTF8.GetMaxByteCount(value.Length);
            Span<byte> bytes = stackalloc byte[byteCount];
            byteCount = Encoding.UTF8.GetBytes(value, bytes);

            builder.StoreBytes(bytes[..byteCount]);

            return builder;
        }

        /// <summary>
        /// Loads string saved in "snake" format (when part of the data stored in a cell and the rest of the data in the first child cell (and so recursively)).
        /// </summary>
        /// <param name="slice">Slice to load string from.</param>
        /// <returns>Decoded string (or null, if slice is empty).</returns>
        /// <param name="omitPrefix">When <b>true</b>, the 0x00 prefix is not read / checked.</param>
        /// <exception cref="InvalidOperationException">When <paramref name="omitPrefix"/> is <b>false</b> and value is not prefixed with 0x00 byte.</exception>
        /// <exception cref="InvalidDataException">When bytes can't be decoded to valid utf-8 string.</exception>
        /// <remarks>
        /// See also:
        /// <see cref="StoreStringSnake">StoreStringSnake</see>
        /// and
        /// <seealso href="https://github.com/ton-blockchain/TEPs/blob/master/text/0064-token-data-standard.md#data-serialization">TEP-64</seealso>
        /// .
        /// </remarks>
        public static string? LoadStringSnake(this Slice slice, bool omitPrefix = false)
        {
            if (!omitPrefix)
            {
                var prefix = slice.PreloadByte();
                if (prefix == 0x00)
                {
                    slice.SkipBits(8); // preloaded
                }
                else
                {
                    throw new InvalidOperationException("The 0x00 prefix was not found.");
                }
            }

            Span<byte> bytes = stackalloc byte[128];
            Span<char> chars = stackalloc char[128];
            var decoder = Encoding.UTF8.GetDecoder();
            var res = new StringBuilder(1024);
            var completed = false;
            var remaining = 0;
            Span<byte> partial = stackalloc byte[1];

            while (slice.Length > 0)
            {
                var bytesCount = slice.Length / 8;
                if (bytesCount > 0)
                {
                    slice.LoadBytesTo(bytes[..bytesCount]);
                    decoder.Convert(bytes[..bytesCount], chars, false, out _, out var charsCount, out completed);
                    res.Append(chars[..charsCount]);
                }

                remaining = slice.Length;
                partial.Clear();

                if (remaining > 0)
                {
                    slice.LoadBitsToBytesTo(remaining, partial);
                }

                slice.EndRead();

                var cell = slice.TryLoadRef();
                if (cell == null)
                {
                    break;
                }

                slice = cell.BeginRead();

                if (remaining > 0)
                {
                    remaining = 8 - remaining;
                    bytes[0] = (byte)(partial[0] << remaining);
                    slice.LoadBitsToBytesTo(remaining, partial);
                    bytes[0] |= partial[0];
                    decoder.Convert(bytes[..1], chars, false, out _, out var charsCount, out completed);
                    res.Append(chars[..charsCount]);
                }
            }

            // flush
            decoder.Convert(bytes[..0], chars, true, out _, out var lastCharsCount, out completed);
            res.Append(chars[..lastCharsCount]);

            if (remaining != 0 || !completed)
            {
                throw new InvalidDataException("Decoding to UTF-8 was not 'completed'");
            }

            return res.ToString();
        }

        /// <summary>
        /// Saves string in "snake" format (when part of the data stored in a cell and the rest of the data in the first child cell (and so recursively)).
        /// </summary>
        /// <param name="builder">Builder to store string at.</param>
        /// <param name="value">String to store.</param>
        /// <param name="omitPrefix">When <b>true</b>, the 0x00 prefix is not written.</param>
        /// <remarks>
        /// See also:
        /// <see cref="LoadStringSnake">LoadStringSnake</see>
        /// and
        /// <seealso href="https://github.com/ton-blockchain/TEPs/blob/master/text/0064-token-data-standard.md#data-serialization">TEP-64</seealso>
        /// .
        /// </remarks>
        public static CellBuilder StoreStringSnake(this CellBuilder builder, string? value, bool omitPrefix = false)
        {
            if (!omitPrefix)
            {
                builder.StoreByte(0x00);
            }

            if (string.IsNullOrEmpty(value))
            {
                return builder;
            }

            var byteCount = Encoding.UTF8.GetMaxByteCount(value.Length);
            Span<byte> bytes = stackalloc byte[byteCount];

            byteCount = Encoding.UTF8.GetBytes(value, bytes);
            bytes = bytes[..byteCount];

            var currentBuilder = builder;
            while (bytes.Length > 0)
            {
                var bitsLeft = Cell.MaxBitsCount - currentBuilder.Length;
                var bytesLeft = bitsLeft / 8;

                if (bytesLeft > 0)
                {
                    var toWrite = Math.Min(bytesLeft, bytes.Length);
                    currentBuilder.StoreBytes(bytes[0..toWrite]);
                    bytes = bytes[toWrite..];
                }

                if (bytes.Length == 0)
                {
                    break;
                }

                var nextBuilder = new CellBuilder();
                currentBuilder.StoreRef(nextBuilder);
                currentBuilder = nextBuilder;
            }

            return builder;
        }

        /// <summary>
        /// Loads string in "chunked" format (in dictionary chunk_index -> chunk).
        /// </summary>
        /// <param name="slice">Slice to load string from.</param>
        /// <returns>Decoded string (or null, if slice is empty).</returns>
        /// <param name="omitPrefix">When <b>true</b>, the 0x01 prefix is not read / checked.</param>
        /// <exception cref="InvalidOperationException">When <paramref name="omitPrefix"/> is <b>false</b> and value is not prefixed with 0x01 byte.</exception>
        /// <exception cref="InvalidDataException">When bytes can't be decoded to valid utf-8 string.</exception>
        /// <remarks>
        /// See also:
        /// <see cref="StoreStringChunked">StoreStringChunked</see>
        /// and
        /// <seealso href="https://github.com/ton-blockchain/TEPs/blob/master/text/0064-token-data-standard.md#data-serialization">TEP-64</seealso>
        /// .
        /// </remarks>
        public static string? LoadStringChunked(this Slice slice, bool omitPrefix = false)
        {
            if (!omitPrefix)
            {
                var prefix = slice.PreloadByte();
                if (prefix == 0x01)
                {
                    slice.SkipBits(8); // preloaded
                }
                else
                {
                    throw new InvalidOperationException("The 0x01 prefix was not found.");
                }
            }

            Span<byte> bytes = stackalloc byte[128];
            Span<char> chars = stackalloc char[128];
            var decoder = Encoding.UTF8.GetDecoder();
            var res = new StringBuilder(1024);
            var completed = false;
            var remaining = 0;
            Span<byte> partial = stackalloc byte[1];

            var dict = slice.LoadAndParseDict(32, s => s.LoadInt(), s => s);

            for (var i = 0; i < dict.Count; i++)
            {
                var s = dict[i];

                if (remaining > 0)
                {
                    remaining = 8 - remaining;
                    bytes[0] = (byte)(partial[0] << remaining);
                    s.LoadBitsToBytesTo(remaining, partial);
                    bytes[0] |= partial[0];
                    decoder.Convert(bytes[..1], chars, false, out _, out var charsCount, out completed);
                    res.Append(chars[..charsCount]);
                }

                var bytesCount = s.Length / 8;
                if (bytesCount > 0)
                {
                    s.LoadBytesTo(bytes[..bytesCount]);
                    decoder.Convert(bytes[..bytesCount], chars, false, out _, out var charsCount, out completed);
                    res.Append(chars[..charsCount]);
                }

                remaining = s.Length;
                partial.Clear();

                if (remaining > 0)
                {
                    s.LoadBitsToBytesTo(remaining, partial);
                }

                s.EndRead();
            }

            // flush
            decoder.Convert(bytes[..0], chars, true, out _, out var lastCharsCount, out completed);
            res.Append(chars[..lastCharsCount]);

            if (remaining != 0 || !completed)
            {
                throw new InvalidDataException("Decoding to UTF-8 was not 'completed'");
            }

            return res.ToString();
        }

        /// <summary>
        /// Saves string in "chunked" format (in dictionary chunk_index -> chunk).
        /// </summary>
        /// <param name="builder">Builder to store string at.</param>
        /// <param name="value">String to store.</param>
        /// <param name="omitPrefix">When <b>true</b>, the 0x01 prefix is not written.</param>
        /// <remarks>
        /// See also:
        /// <see cref="LoadStringChunked">LoadStringchunked</see>
        /// and
        /// <seealso href="https://github.com/ton-blockchain/TEPs/blob/master/text/0064-token-data-standard.md#data-serialization">TEP-64</seealso>
        /// .
        /// </remarks>
        public static CellBuilder StoreStringChunked(this CellBuilder builder, string? value, bool omitPrefix = false)
        {
            if (!omitPrefix)
            {
                builder.StoreByte(0x01);
            }

            if (string.IsNullOrEmpty(value))
            {
                return builder;
            }

            var bytes = Encoding.UTF8.GetBytes(value);
            var bytesPerCell = (Cell.MaxBitsCount - 70) / 8;
            var lastCellBytes = bytes.Length % bytesPerCell;
            var cellsCount = bytes.Length / bytesPerCell + Math.Sign(lastCellBytes);

            var dict = Enumerable.Range(0, cellsCount).ToDictionary(x => x, x => (x, x == cellsCount - 1 ? lastCellBytes : bytesPerCell));

            builder.StoreDict(dict, 32, (b, k) => b.StoreInt(k), (b, v) => b.StoreBytes(bytes.AsSpan(v.Item1 * bytesPerCell, v.Item2)));

            return builder;
        }
    }
}
