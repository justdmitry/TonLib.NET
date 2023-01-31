namespace TonLibDotNet.Requests
{
    /*
     *
     * Sample request:
     * {"@type":"liteServer.getInfo"}
     *
     * Sample response:
     * {"@type":"liteServer.info","now":1675154793,"version":257,"capabilities":"7"}
     *
     */
    public class LiteServerGetInfo : RequestBase<Types.LiteServer.Info>
    {
        public LiteServerGetInfo()
        {
            TypeName = "liteServer.getInfo";
        }
    }
}
