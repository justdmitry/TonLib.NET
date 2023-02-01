namespace TonLibDotNet.Types.Raw
{
    [TLSchema("raw.message source:accountAddress destination:accountAddress value:int64 fwd_fee:int64 ihr_fee:int64 created_lt:int64 body_hash:bytes msg_data:msg.Data = raw.Message")]
    public class Message : TypeBase
    {
        public Message(AccountAddress source, AccountAddress destination, Msg.Data msgData)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));
            Destination = destination ?? throw new ArgumentNullException(nameof(destination));
            MsgData = msgData ?? throw new ArgumentNullException(nameof(msgData));
        }

        public AccountAddress Source { get; set; }

        public AccountAddress Destination { get; set; }

        public long Value { get; set; }

        public long FwdFee { get; set; }

        public long IhrFee { get; set; }

        public long CreatedLt { get; set; }

        public string BodyHash { get; set; } = string.Empty;

        public Msg.Data MsgData { get; set; }
    }
}
