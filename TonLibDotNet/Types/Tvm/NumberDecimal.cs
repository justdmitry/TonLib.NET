using System.Globalization;
using System.Numerics;
using System.Text.Json.Serialization;

namespace TonLibDotNet.Types.Tvm
{
    [TLSchema("tvm.numberDecimal number:string = tvm.Number")]
    public class NumberDecimal : Number
    {
        [JsonConstructor]
        public NumberDecimal(string number)
        {
            Number = number;
        }

        public NumberDecimal(int number)
        {
            Number = number.ToString(CultureInfo.InvariantCulture);
        }

        public NumberDecimal(long number)
        {
            Number = number.ToString(CultureInfo.InvariantCulture);
        }

        public NumberDecimal(BigInteger number)
        {
            Number = number.ToString(CultureInfo.InvariantCulture);
        }

        public NumberDecimal(byte[] number)
        {
            Number = new BigInteger(number, true, true).ToString(CultureInfo.InvariantCulture);
        }

        public string Number { get; set; }

        public int NumberAsInt()
        {
            return int.Parse(Number, CultureInfo.InvariantCulture);
        }

        public long NumberAsLong()
        {
            return long.Parse(Number, CultureInfo.InvariantCulture);
        }

        public BigInteger NumberAsBigInteger()
        {
            return BigInteger.Parse(Number, CultureInfo.InvariantCulture);
        }

        public byte[] NumberAsBigIntegerBytes()
        {
            return BigInteger.Parse(Number, CultureInfo.InvariantCulture).ToByteArray(true, true);
        }
    }
}
