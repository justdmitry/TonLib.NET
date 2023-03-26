namespace TonLibDotNet.Types.Smc
{
    [TLSchema("smc.methodIdNumber number:int32 = smc.MethodId")]
    public class MethodIdNumber : MethodId
    {
        public MethodIdNumber(long number)
        {
            Number = number;
        }

        public long Number { get; set; }
    }
}
