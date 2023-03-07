namespace TonLibDotNet.Types.Msg
{
    [TLSchema("msg.dataEncrypted source:accountAddress data:msg.Data = msg.DataEncrypted")]
    public class DataEncrypted : TypeBase
    {
        public DataEncrypted(AccountAddress source, Data data)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));
            Data = data ?? throw new ArgumentNullException(nameof(data));
        }

        public AccountAddress Source { get; set; }

        public Data Data { get; set; }
    }
}
