namespace TonLibDotNet.Types.Dns
{
    [TLSchema("dns.actionDelete name:string category:int256 = dns.Action")]
    public class ActionDelete : Action
    {
        public string Name { get; set; } = string.Empty;

        public string Category { get; set; } = Int256_AllZeroes;
    }
}
