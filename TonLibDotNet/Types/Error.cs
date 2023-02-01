namespace TonLibDotNet.Types
{
    [TLSchema("error code:int32 message:string = Error")]
    public class Error : TypeBase
    {
        public int Code { get; set; }

        public string Message { get; set; } = string.Empty;
    }
}
