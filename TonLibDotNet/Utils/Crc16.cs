namespace TonLibDotNet.Utils
{
    /// <summary>
    /// Based on http://sanity-free.org/133/crc_16_ccitt_in_csharp.html
    /// </summary>
    public class Crc16
    {
        private readonly ushort[] table = new ushort[256];

        public Crc16(ushort initialValue, ushort poly)
        {
            this.InitialValue = initialValue;

            ushort temp, a;
            for (int i = 0; i < table.Length; ++i)
            {
                temp = 0;
                a = (ushort)(i << 8);
                for (int j = 0; j < 8; ++j)
                {
                    if (((temp ^ a) & 0x8000) != 0)
                    {
                        temp = (ushort)((temp << 1) ^ poly);
                    }
                    else
                    {
                        temp <<= 1;
                    }
                    a <<= 1;
                }
                table[i] = temp;
            }
        }

        public static Crc16 Ccitt { get; } = new Crc16(0x0000, 0x1021);

        public ushort InitialValue { get; private set; }

        public ushort ComputeChecksum(ReadOnlySpan<byte> bytes)
        {
            ushort crc = InitialValue;
            for (int i = 0; i < bytes.Length; ++i)
            {
                crc = (ushort)((crc << 8) ^ table[(crc >> 8) ^ (0xff & bytes[i])]);
            }
            return crc;
        }
    }
}