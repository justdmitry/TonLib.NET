namespace TonLibDotNet.Utils
{
    public class ArraySegmentOfBoolComparer : IComparer<ArraySegment<bool>>
    {
        public static readonly ArraySegmentOfBoolComparer Instance = new ();

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
