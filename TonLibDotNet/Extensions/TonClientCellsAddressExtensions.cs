using TonLibDotNet.Cells;

namespace TonLibDotNet
{
    public static class TonClientCellsAddressExtensions
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

        public static string? TryLoadAddressIntStd(this Slice slice, bool bounceable = true, bool testnetOnly = false, bool urlSafe = true)
        {
            var header = slice.PreloadBits(2);
            if (!header[0] && !header[1])
            {
                slice.SkipBits(2); // skip checked bits
                return null;
            }

            return LoadAddressIntStd(slice, bounceable, testnetOnly, urlSafe);
        }

        public static CellBuilder StoreAddressIntStd(this CellBuilder builder, string address)
        {
            if (!AddressValidator.TryParseAddress(address, out var workchainId, out var accountId, out _, out _, out _))
            {
                throw new ArgumentException("Not a valid address", nameof(address));
            }

            builder.EnsureCanStore(3 + 8 + 256);

            builder.StoreBit(true);
            builder.StoreBit(false);
            builder.StoreBit(false); // anycast
            builder.StoreByte(workchainId);
            builder.StoreBytes(accountId);

            return builder;
        }
    }
}
