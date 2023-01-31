namespace TonLibDotNet.Types
{
    /// <remarks>
    /// TL Schema:
    /// <code>error code:int32 message:string = Error;</code>
    /// </remarks>
    public class Error : TypeBase
    {
        public const string ErrorTypeName = "@error";

        public Error()
        {
            this.TypeName = ErrorTypeName;
        }

        public int Code { get; set; }

        public string Message { get; set; } = string.Empty;
    }
}
