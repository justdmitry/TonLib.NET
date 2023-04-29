using TonLibDotNet.Cells;

namespace TonLibDotNet
{
    public static class TonClientCellsDictExtensions
    {
        public static Dictionary<TKey, TValue> LoadDict<TKey, TValue>(this Slice slice, int bitsForKey, int bitsForValue, Func<Slice, TKey> keyReader, Func<Slice, TValue> valueReader)
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
    }
}
