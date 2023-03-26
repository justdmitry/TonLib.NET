namespace TonLibDotNet.Types.Smc
{
    [TLSchema("smc.methodIdName name:string = smc.MethodId")]
    public class MethodIdName : MethodId
    {
        public MethodIdName(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}
