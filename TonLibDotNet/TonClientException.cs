using System.Runtime.Serialization;
using TonLibDotNet.Types;
using TonLibDotNet.Utils;

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

            var actualAnswerJson = info.GetString(nameof(ActualAnswer));
            if (!string.IsNullOrEmpty(actualAnswerJson))
            {
                ActualAnswer = new TonJsonSerializer().Deserialize(actualAnswerJson);
            }
        }

        public int Code { get; set; }

        public TypeBase? ActualAnswer { get; set; }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(Code), Code);
            info.AddValue(nameof(ActualAnswer), ActualAnswer == null ? string.Empty : new TonJsonSerializer().Serialize(ActualAnswer));
        }
    }
}
