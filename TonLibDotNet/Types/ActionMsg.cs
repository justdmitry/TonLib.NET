using TonLibDotNet.Types.Msg;

namespace TonLibDotNet.Types
{
    [TLSchema("actionMsg messages:vector<msg.message> allow_send_to_uninited:Bool = Action")]
    public class ActionMsg : Action
    {
        public ActionMsg(List<Message> messages)
        {
            Messages = messages ?? throw new ArgumentNullException(nameof(messages));
        }

        public List<Message> Messages { get; set; }

        public bool AllowSendToUninited { get; set; }
    }
}
