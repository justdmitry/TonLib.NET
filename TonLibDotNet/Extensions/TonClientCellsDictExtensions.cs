using TonLibDotNet.Cells;

namespace TonLibDotNet
{
    /// <summary>
    /// LoadDict and StoreDict implementations.
    /// </summary>
    /// <remarks>
    /// TL-B schema for Hashmap (HashmapE) from https://docs.ton.org/develop/data-formats/tl-b-types#hashmap:
    /// <![CDATA[
    ///
    ///    hm_edge#_ {n:#} {X:Type} {l:#} {m:#} label:(HmLabel ~l n)
    ///                {n = (~m) + l} node:(HashmapNode m X) = Hashmap n X;
    ///
    ///    hmn_leaf#_ {X:Type} value:X = HashmapNode 0 X;
    ///    hmn_fork#_ {n:#} {X:Type} left:^(Hashmap n X)
    ///                right:^(Hashmap n X) = HashmapNode (n + 1) X;
    ///
    ///    hml_short$0 {m:#} {n:#} len:(Unary ~n) {n <= m} s:(n * Bit) = HmLabel ~n m;
    ///    hml_long$10 {m:#} n:(#<= m) s:(n * Bit) = HmLabel ~n m;
    ///    hml_same$11 {m:#} v:Bit n:(#<= m) = HmLabel ~n m;
    ///
    ///    unary_zero$0 = Unary ~0;
    ///    unary_succ$1 {n:#} x:(Unary ~n) = Unary ~(n + 1);
    ///
    ///    hme_empty$0 {n:#} {X:Type} = HashmapE n X;
    ///    hme_root$1 {n:#} {X:Type} root:^(Hashmap n X) = HashmapE n X;
    ///
    /// ]]>
    /// </remarks>
    /// <seealso href="https://docs.ton.org/develop/data-formats/tl-b-types#hashmap">Hashmap (HashmapE) type in TL-B.</seealso>
    public static class TonClientCellsDictExtensions
    {
        /// <summary>
        /// Returns <see cref="Cell" /> with dict data, or throws exception. <br/>
        /// Use <see cref="ParseDict"/> to read actual data from returned cell.
        /// </summary>
        /// <remarks>
        /// Checks next bit in slice: when 1 (dictionary exists) - returns next ref from current slice, when 0 (dictionary is empty) - throws exception.
        /// </remarks>
        /// <exception cref="InvalidOperationException">Dictionary is empty</exception>
        public static Cell LoadDict(this Slice slice)
        {
            return TryLoadDict(slice) ?? throw new InvalidOperationException("No dictionary found");
        }

        /// <summary>
        /// Returns <see cref="Cell" /> with dict data, if available (or null if dictionary is empty).<br/>
        /// Use <see cref="ParseDict"/> to read actual data from returned cell.
        /// </summary>
        /// <remarks>
        /// Checks next bit in slice: when 1 (dictionary exists) - returns next ref from current slice, when 0 (dictionary is empty) - returns null.
        /// </remarks>
        public static Cell? TryLoadDict(this Slice slice)
        {
            return slice.LoadBit() ? slice.LoadRef() : null;
        }

        /// <summary>
        /// Chains <see cref="LoadDict"/> and <see cref="ParseDict"/> calls and returns actual dict with data.
        /// </summary>
        /// <remarks>
        /// Sometimes <see href="https://github.com/ton-blockchain/dns-contract/blob/main/func/nft-item.fc#L21">arbitrary cells are stored using store_dict()</see>, so not every <see cref="LoadDict"/> result should be parsed.
        /// </remarks>
        /// <exception cref="InvalidOperationException">Dictionary is empty</exception>
        public static Dictionary<TKey, TValue> LoadAndParseDict<TKey, TValue>(this Slice slice, int keyBitLength, Func<Slice, TKey> keyReader, Func<Slice, TValue> valueReader, IEqualityComparer<TKey>? comparer = null)
            where TKey : notnull
        {
            var cell = LoadDict(slice);
            return ParseDict(cell, keyBitLength, keyReader, valueReader, comparer);
        }

        /// <summary>
        /// Chains <see cref="TryLoadDict"/> and <see cref="ParseDict"/> calls and returns actual dict with data (or null).
        /// </summary>
        /// <remarks>
        /// Sometimes <see href="https://github.com/ton-blockchain/dns-contract/blob/main/func/nft-item.fc#L21">arbitrary cells are stored using store_dict()</see>, so not every <see cref="TryLoadDict"/> result should be parsed.
        /// </remarks>
        public static Dictionary<TKey, TValue>? TryLoadAndParseDict<TKey, TValue>(this Slice slice, int keyBitLength, Func<Slice, TKey> keyReader, Func<Slice, TValue> valueReader, IEqualityComparer<TKey>? comparer = null)
            where TKey : notnull
        {
            var cell = TryLoadDict(slice);
            return cell == null ? null : ParseDict(cell, keyBitLength, keyReader, valueReader, comparer);
        }

        /// <summary>
        /// Parses dictionary from Cell (previously loaded by <see cref="LoadDict(Slice)"/>).
        /// </summary>
        public static Dictionary<TKey, TValue> ParseDict<TKey, TValue>(this Cell cell, int keyBitLength, Func<Slice, TKey> keyReader, Func<Slice, TValue> valueReader, IEqualityComparer<TKey>? comparer = null)
            where TKey : notnull
        {
            var items = new List<(Slice key, Slice value)>();
            ParseDictImpl(cell, Array.Empty<bool>(), keyBitLength, items);

            return items.ToDictionary(
                x =>
                {
                    var val = keyReader(x.key);
                    x.key.EndRead();
                    return val;
                },
                x =>
                {
                    var valueSrc = x.value.TryCanLoad(1) ? x.value : x.value.LoadRef().BeginRead();
                    var val = valueReader(valueSrc);
                    if (val is not Slice)
                    {
                        x.value.EndRead();
                    }
                    return val;
                },
                comparer);
        }

        /// <summary>
        /// Stores <see cref="Cell"/> with dict data into builder.
        /// </summary>
        /// <remarks>
        /// For non-null cells, stores 1 and adds dict cell as reference to current builder. For null cells, stores 0.
        /// </remarks>
        public static CellBuilder StoreDict(this CellBuilder builder, Cell? dict)
        {
            if (dict == null)
            {
                return builder.StoreBit(false);
            }
            else
            {
                return builder.StoreBit(true).StoreRef(dict);
            }
        }

        /// <summary>
        /// Serializes dict into <see cref="Cell"/> and stores that cell into builder.
        /// </summary>
        /// <remarks>
        /// For null and empty dicts - stores 0 bit only.
        /// </remarks>
        public static CellBuilder StoreDict<TKey, TValue>(this CellBuilder builder, Dictionary<TKey, TValue>? dict, int keyBitLength, Action<CellBuilder, TKey> keyWriter, Action<CellBuilder, TValue> valueWriter)
            where TKey : notnull
        {
            if (dict == null || dict.Count == 0)
            {
                return builder.StoreBit(false);
            }

            builder.StoreBit(true);

            var list = dict.Select(x =>
                {
                    var cbk = new CellBuilder();
                    keyWriter(cbk, x.Key);

                    if (keyBitLength != cbk.Length)
                    {
                        throw new InvalidOperationException($"Key length mismatch on item '{x.Value}': {keyBitLength} expected, but {cbk.Length} found.");
                    }

                    return (cbk.GetAllBits(), x.Value);
                })
                .OrderBy(x => x.Item1, ArraySegmentOfBoolComparer.Instance)
                .ToArray();

            builder.StoreRef(StoreDictImpl(new ArraySegment<(ArraySegment<bool> key, TValue value)>(list), list[0].Item1.Count, 0, valueWriter).Build());

            return builder;
        }

        private static void ParseDictImpl(Cell edge, ReadOnlySpan<bool> keySoFar, int maxBitsForKey, List<(Slice key, Slice value)> items)
        {
            var source = edge.BeginRead();

            Span<bool> label = stackalloc bool[0];

            var lbl1 = source.LoadBit();
            if (!lbl1) // htl_short$0
            {
                var len = 0;
                while (source.LoadBit())
                {
                    len++;
                }

                if (len > 0)
                {
                    label = stackalloc bool[len];
                    source.LoadBitsTo(label);
                }
            }
            else
            {
                var n_len = (int)Math.Ceiling(Math.Log2(maxBitsForKey + 1));
                var lbl2 = source.LoadBit();
                if (lbl2) // hml_same$11
                {
                    var v = source.LoadBit();
                    var n = source.LoadInt(n_len);
                    label = stackalloc bool[n];
                    label.Fill(v);
                }
                else // hml_long$10
                {
                    var n = source.LoadInt(n_len);
                    label = stackalloc bool[n];
                    source.LoadBitsTo(label);
                }
            }

            var label_left = maxBitsForKey - label.Length;

            if (label_left == 0)
            {
                var key = new bool[keySoFar.Length + maxBitsForKey];
                keySoFar.CopyTo(key);
                label.CopyTo(key.AsSpan(keySoFar.Length));
                items.Add((new Slice(key), source));
            }
            else
            {
                Span<bool> key = stackalloc bool[keySoFar.Length + label.Length + 1];
                keySoFar.CopyTo(key);
                label.CopyTo(key[keySoFar.Length..]);
                key[^1] = false;
                ParseDictImpl(source.LoadRef(), key, label_left - 1, items);

                key[^1] = true;
                ParseDictImpl(source.LoadRef(), key, label_left - 1, items);
            }
        }

        private static CellBuilder StoreDictImpl<TValue>(ArraySegment<(ArraySegment<bool> key, TValue value)> items, int keyLength, int keyPosition, Action<CellBuilder, TValue> valueWriter)
        {
            static CellBuilder CreateWithLabel(ArraySegment<bool> label, int lengthLeft)
            {
                if (label.Count == 0) // short with 0 length
                {
                    return new CellBuilder().StoreBit(false).StoreBit(false);
                }

                var n = (int)Math.Ceiling(Math.Log2(lengthLeft + 1));
                var ones = label.Count(x => x);
                if (ones == 0 || ones == label.Count) // store as same
                {
                    return new CellBuilder().StoreBit(true).StoreBit(true).StoreBit(label[0]).StoreInt(label.Count, n);
                }

                var longLength = 2 + n + label.Count;
                var shortLength = 1 + label.Count + 1 + label.Count;
                if (longLength < shortLength) // long
                {
                    return new CellBuilder().StoreBit(true).StoreBit(false).StoreInt(label.Count, n).StoreBits(label);
                }
                else // short
                {
                    var cb = new CellBuilder();
                    cb.StoreBit(false);
                    for(var i = 0; i < label.Count; i++)
                    {
                        cb.StoreBit(true);
                    }

                    cb.StoreBit(false);
                    return cb.StoreBits(label);
                }
            }

            CellBuilder cb;

            if (items.Count == 1)
            {
                var label = items[0].key.Slice(keyPosition);
                cb = CreateWithLabel(label, label.Count);
                valueWriter(cb, items[0].value);
                return cb;
            }

            var labelLength = 0;
            var items0 = 0;
            var items1 = 0;

            do
            {
                items0 = items1 = 0;
                foreach (var (key, _) in items)
                {
                    if (key[keyPosition + labelLength])
                    {
                        items1++;
                    }
                    else
                    {
                        items0++;
                    }
                }

                labelLength++;
            }
            while (items0 == 0 || items1 == 0);

            cb = CreateWithLabel(items[0].key.Slice(keyPosition, labelLength - 1), keyLength - keyPosition);

            cb.StoreRef(StoreDictImpl(items.Slice(0, items0), keyLength, keyPosition + labelLength, valueWriter));
            cb.StoreRef(StoreDictImpl(items.Slice(items0), keyLength, keyPosition + labelLength, valueWriter));

            return cb;
        }
    }
}
