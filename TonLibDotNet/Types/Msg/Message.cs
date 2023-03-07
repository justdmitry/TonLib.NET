namespace TonLibDotNet.Types.Msg
{
    [TLSchema("msg.message destination:accountAddress public_key:string amount:int64 data:msg.Data send_mode:int32 = msg.Message")]
    public class Message : TypeBase
    {
        public Message(AccountAddress destination)
        {
            Destination = destination ?? throw new ArgumentNullException(nameof(destination));
        }

        public AccountAddress Destination { get; set; }

        public string? PublicKey { get; set; }

        public long Amount { get; set; }

        public Data? Data { get; set; }

        /// <summary>
        /// Mode and zero or several flags.
        /// </summary>
        /// <remarks>
        /// Modes:
        /// <b>0</b> - Ordinary message
        /// <b>64</b> - Carry all the remaining value of the inbound message in addition to the value initially indicated in the new message.
        /// <b>128</b> - Carry all the remaining balance of the current smart contract instead of the value originally indicated in the message.
        ///
        /// Flags:
        /// <b>+1</b> - Pay transfer fees separately from the message value.
        /// <b>+2</b> - Ignore any errors arising while processing this message during the action phase.
        /// <b>+32</b> - Current account must be destroyed if its resulting balance is zero (often used with Mode 128).
        /// </remarks>
        /// <seealso href="https://ton.org/docs/develop/func/stdlib/#send_raw_message"/>
        public int SendMode { get; set; }
    }
}
