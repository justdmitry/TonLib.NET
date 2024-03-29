﻿namespace TonLibDotNet.Requests
{
    public class GetAccountStateTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.GetAccountState);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<GetAccountState>(baseObj);
            Assert.NotNull(obj.AccountAddress);
            Assert.Equal("EQCJTkhd1W2wztkVNp_dsKBpv2SIoUWoIyzI7mQrbSrj_NSh", obj.AccountAddress.Value);
        }
    }
}
