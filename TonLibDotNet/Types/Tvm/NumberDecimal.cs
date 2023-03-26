namespace TonLibDotNet.Types.Tvm
{
    [TLSchema("tvm.numberDecimal number:string = tvm.Number")]
    public class NumberDecimal : Number
    {
        public NumberDecimal(string number)
        {
            Number = number;
        }

        public string Number { get; set; }
    }
}
