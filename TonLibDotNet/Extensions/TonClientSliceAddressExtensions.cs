using TonLibDotNet.Cells;

namespace TonLibDotNet
{
    public static class TonClientSliceAddressExtensions
    {
        /*
         tblchk.pdf
         3.1.2. TL-B scheme for addresses.
            addr_std$10 anycast:(Maybe Anycast)
                workchain_id:int8 address:uint256
                = MsgAddressInt
         */
        public static string LoadAddressIntStd(this Slice slice, bool bounceable = true, bool testnetOnly = false, bool urlSafe = true)
        {
            var header = slice.PreloadBits(3);
            if (!header[0] || header[1])
            {
                throw new InvalidOperationException("Not an addr_std: '10' not found");
            }

            if (header[2])
            {
                throw new NotImplementedException("Anycast_Info is not supported yet.");
            }

            slice.EnsureCanLoad(3 + 8 + 256);

            slice.SkipBits(3); // Skip checked bits

            var workchainId = slice.LoadByte();

            Span<byte> accountId = stackalloc byte[32];
            slice.LoadBytesTo(accountId);

            return AddressValidator.MakeAddress(workchainId, accountId, bounceable, testnetOnly, urlSafe);
        }
    }
}
