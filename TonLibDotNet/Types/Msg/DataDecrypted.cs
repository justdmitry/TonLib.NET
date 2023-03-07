namespace TonLibDotNet.Types.Msg
{
    [TLSchema("msg.dataDecrypted proof:bytes data:msg.Data = msg.DataDecrypted")]
    public class DataDecrypted : TypeBase
    {
        public DataDecrypted(string proof, Data data)
        {
            Proof = proof ?? throw new ArgumentNullException(nameof(proof));
            Data = data ?? throw new ArgumentNullException(nameof(data));
        }

        public string Proof { get; set; }

        public Data Data { get; set; }
    }
}
