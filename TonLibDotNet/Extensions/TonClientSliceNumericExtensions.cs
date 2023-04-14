//using TonLibDotNet.Cells;

//namespace TonLibDotNet
//{
//    public static class TonClientSliceNumericExtensions
//    {
//        public static uint LoadUint(this Slice slice, int fromSize = 32)
//        {
//            if (fromSize < 1 || fromSize > 4)
//            {
//                throw new ArgumentOutOfRangeException(nameof(fromSize));
//            }

//            Span<byte> data = stackalloc byte[4];
//            slice.LoadBitsTo(data[..fromSize]);
//        }
//    }
//}
