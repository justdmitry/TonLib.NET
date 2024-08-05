using TonLibDotNet.Types;

namespace TonLibDotNet
{
    public class TonClientException : Exception
    {
        public TonClientException(int code, string message)
            : base(message)
        {
            Code = code;
        }

        public TonClientException(int code, string message, Exception? innerException)
            : base(message, innerException)
        {
            Code = code;
        }

        public int Code { get; set; }

        public TypeBase? ActualAnswer { get; set; }
    }
}
