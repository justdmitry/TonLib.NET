using System.Runtime.Serialization;

namespace TonLibDotNet
{
    [Serializable]
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

        protected TonClientException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Code = info.GetInt32(nameof(Code));
        }

        public int Code { get; set; }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(Code), Code);
        }
    }
}
