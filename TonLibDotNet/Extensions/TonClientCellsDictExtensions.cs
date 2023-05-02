using TonLibDotNet.Cells;

namespace TonLibDotNet
{
    public static class TonClientCellsDictExtensions
    {
        public static Dictionary<TKey, TValue> LoadDict<TKey, TValue>(this Slice slice, int bitsForKey, Func<Slice, TKey> keyReader, Func<Slice, TValue> valueReader)
            where TKey : notnull
        {
            if (!slice.LoadBit())
            {
                return new Dictionary<TKey, TValue>();
            }

            var cell = slice.LoadRef();
            var items = new List<(Slice key, Slice value)>();
            LoadDictImpl(cell, Array.Empty<bool>(), bitsForKey, items);
            return items.ToDictionary(x =>
            {
                var val = keyReader(x.key);
                x.key.EndRead();
                return val;
            },
            x =>
            {
                var val = valueReader(x.value);
                x.value.EndRead();
                return val;
            });
        }

        public static CellBuilder StoreDict<TKey, TValue>(this CellBuilder builder, Dictionary<TKey, TValue>? dict, Action<CellBuilder, TKey> keyWriter, Action<CellBuilder, TValue> valueWriter)
            where TKey : notnull
        {
            if (dict == null || dict.Count == 0)
            {
                builder.StoreBit(false);
            }
            else
            {
                builder.StoreBit(true);
                var keyLength = -1;
                var list = dict.Select(x =>
                    {
                        var cbk = new CellBuilder();
                        keyWriter(cbk, x.Key);

                        if (keyLength == -1)
                        {
                            keyLength = cbk.Length;
                        }
                        else if (keyLength != cbk.Length)
                        {
                            throw new InvalidOperationException($"Key length mismatch on item '{x.Value}': {keyLength} expected, but {cbk.Length} found.");
                        }

                        return (cbk.GetAllBits(), x.Value);
                    })
                    .OrderBy(x => x.Item1, new ArraySegmentComparer())
                    .ToArray();
                builder.StoreRef(StoreDictImpl(new ArraySegment<(ArraySegment<bool> key, TValue value)>(list), list[0].Item1.Count, 0, valueWriter).Build());
            }

            return builder;
        }

        /*
            hm_edge#_ {n:#} {X:Type} {l:#} {m:#} label:(HmLabel ~l n)
                      {n = (~m) + l} node:(HashmapNode m X) = Hashmap n X

            hmn_leaf#_ {X:Type} value:X = HashmapNode 0 X
            hmn_fork#_ {n:#} {X:Type} left:^(Hashmap n X)
                       right:^(Hashmap n X) = HashmapNode (n + 1) X

            hml_short$0 {m:#} {n:#} len:(Unary ~n) {n <= m} s:(n * Bit) = HmLabel ~n m
            hml_long$10 {m:#} n:(#<= m) s:(n * Bit) = HmLabel ~n m
            hml_same$11 {m:#} v:Bit n:(#<= m) = HmLabel ~n m

            unary_zero$0 = Unary ~0
            unary_succ$1 {n:#} x:(Unary ~n) = Unary ~(n + 1)

            hme_empty$0 {n:#} {X:Type} = HashmapE n X
            hme_root$1 {n:#} {X:Type} root:^(Hashmap n X) = HashmapE n X
         */
        private static void LoadDictImpl(Cell edge, ReadOnlySpan<bool> keySoFar, int maxBitsForKey, List<(Slice key, Slice value)> items)
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
                LoadDictImpl(source.LoadRef(), key, label_left - 1, items);

                key[^1] = true;
                LoadDictImpl(source.LoadRef(), key, label_left - 1, items);
            }
        }

        private static CellBuilder StoreDictImpl<TValue>(ArraySegment<(ArraySegment<bool> key, TValue value)> items, int keyLength, int keyPosition, Action<CellBuilder, TValue> valueWriter)
        {
            CellBuilder WriteLabel(CellBuilder cellBuilder, ArraySegment<bool> label, int lengthLeft)
            {
                if (label.Count == 0)
                {
                    // short with 0 length
                    return cellBuilder.StoreBit(false).StoreBit(false);
                }

                var n = (int)Math.Ceiling(Math.Log2(lengthLeft + 1));
                var ones = label.Count(x => x);
                if (ones == 0 || ones == label.Count)
                {
                    // store as same
                    return cellBuilder.StoreBit(true).StoreBit(true).StoreBit(label[0]).StoreInt(label.Count, n);
                }

                var longLength = 2 + n + label.Count;
                var shortLength = 1 + label.Count + 1 + label.Count;
                if (longLength < shortLength)
                {
                    // long
                    return cellBuilder.StoreBit(true).StoreBit(false).StoreInt(label.Count, n).StoreBits(label);
                }
                else
                {
                    // short
                    cellBuilder.StoreBit(false);
                    for(var i = 0; i < label.Count; i++)
                    {
                        cellBuilder.StoreBit(true);
                    }

                    cellBuilder.StoreBit(false);
                    return cellBuilder.StoreBits(label);
                }
            }

            var cb = new CellBuilder();

            if (items.Count == 1)
            {
                var label = items[0].key.Slice(keyPosition);
                WriteLabel(cb, label, label.Count);
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

            WriteLabel(cb, items[0].key.Slice(keyPosition, labelLength - 1), keyLength - keyPosition - labelLength);

            cb.StoreRef(StoreDictImpl(items.Slice(0, items0), keyLength, keyPosition + labelLength, valueWriter));
            cb.StoreRef(StoreDictImpl(items.Slice(items0), keyLength, keyPosition + labelLength, valueWriter));

            return cb;
        }

        private sealed class ArraySegmentComparer : IComparer<ArraySegment<bool>>
        {
            public int Compare(ArraySegment<bool> x, ArraySegment<bool> y)
            {
                var minLength = Math.Min(x.Count, y.Count);
                for (var i = 0; i < minLength; i++)
                {
                    var cmp = x[i].CompareTo(y[i]);
                    if (cmp != 0)
                    {
                        return cmp;
                    }
                }

                return x.Count.CompareTo(y.Count);
            }
        }
    }
}
