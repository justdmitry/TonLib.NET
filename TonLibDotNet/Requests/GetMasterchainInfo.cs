namespace TonLibDotNet.Requests
{
    /*
     *
     * Sample request:
     * {"@type":"blocks.getMasterchainInfo"}
     *
     * Sample response:
     * {"@type":"blocks.masterchainInfo",
     *  "last":{"@type":"ton.blockIdExt",
     *          "workchain":-1,
     *          "shard":"-9223372036854775808",
     *          "seqno":26984517,
     *          "root_hash":"k1nEMD+7m+DZAEyktBfy99GTAl/YvFVRBgw3tXr5Xt8=",
     *          "file_hash":"BKPbMdegxyBMEdeWTNY0RG4SI8Cw7tlqCOQkahlj0cM="},
     *  "state_root_hash":"/K7jV7AXp/MKqBL/+3XWpU3kFYp2ObdDG+b9minKUn8=",
     *  "init":{"@type":"ton.blockIdExt",
     *          "workchain":-1,
     *          "shard":"0",
     *          "seqno":0,
     *          "root_hash":"F6OpKZKqvqeFp6CQmFomXNMfMj2EnaUSOXN+Mh+wVWk=",
     *          "file_hash":"XplPz01CXAps5qeSWUtxcyBfdAo5zVb1N979KLSKD24="}}
     *
     */
    public class GetMasterchainInfo : RequestBase<Types.Blocks.MasterchainInfo>
    {
        public GetMasterchainInfo()
        {
            TypeName = "blocks.getMasterchainInfo";
        }
    }
}
