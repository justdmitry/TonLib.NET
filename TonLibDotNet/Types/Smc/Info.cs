namespace TonLibDotNet.Types.Smc
{
    [TLSchema("smc.info id:int53 = smc.Info")]
    public class Info : TypeBase
    {
        public long Id { get; set; }
    }
}
