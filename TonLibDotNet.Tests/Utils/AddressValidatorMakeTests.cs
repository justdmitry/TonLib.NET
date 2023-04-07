namespace TonLibDotNet.Utils
{
    public class AddressValidatorMakeTests
    {
        [Theory]
        // Wallet Bot
        [InlineData(0, "dddcd3cdad60af4c0d69389f567ad51d0c263fa4968d655ab424c69aadaf9322", "EQDd3NPNrWCvTA1pOJ9WetUdDCY_pJaNZVq0JMaara-TIp90")]
        // BSC Bridge
        [InlineData(-1, "4d5c0210b35daddaa219fac459dba0fdefb1fae4e97a0d0797739fe050d694ca", "Ef9NXAIQs12t2qIZ-sRZ26D977H65Ol6DQeXc5_gUNaUys5r")]
        public void ItWorks(int workchainId, string accountIdHex, string expected)
        {
            var accountId = Convert.FromHexString(accountIdHex);
            var account = AddressValidator.MakeAddress((byte)workchainId, accountId);
            Assert.Equal(expected, account);
        }

        [Theory]
        // Wallet Bot
        [InlineData(0, "dddcd3cdad60af4c0d69389f567ad51d0c263fa4968d655ab424c69aadaf9322", "UQDd3NPNrWCvTA1pOJ9WetUdDCY_pJaNZVq0JMaara-TIsKx")]
        public void NonBounceableWorks(int workchainId, string accountIdHex, string expected)
        {
            var accountId = Convert.FromHexString(accountIdHex);
            var account = AddressValidator.MakeAddress((byte)workchainId, accountId, bounceable: false);
            Assert.Equal(expected, account);
        }

        [Theory]
        // "test giver" smart contract
        [InlineData(-1, "fcb91a3a3816d0f7b8c2c76108b8a9bc5a6b7a55bd79f8ab101c52db29232260", "kf_8uRo6OBbQ97jCx2EIuKm8Wmt6Vb15-KsQHFLbKSMiYIny")]
        public void TestnetOnlyWorks(int workchainId, string accountIdHex, string expected)
        {
            var accountId = Convert.FromHexString(accountIdHex);
            var account = AddressValidator.MakeAddress((byte)workchainId, accountId, testnetOnly: true);
            Assert.Equal(expected, account);
        }

        [Theory]
        // "test giver" smart contract
        [InlineData(-1, "fcb91a3a3816d0f7b8c2c76108b8a9bc5a6b7a55bd79f8ab101c52db29232260", "kf/8uRo6OBbQ97jCx2EIuKm8Wmt6Vb15+KsQHFLbKSMiYIny")]
        public void UrlSafeWorks(int workchainId, string accountIdHex, string expected)
        {
            var accountId = Convert.FromHexString(accountIdHex);
            var account = AddressValidator.MakeAddress((byte)workchainId, accountId, testnetOnly: true, urlSafe: false);
            Assert.Equal(expected, account);
        }
    }
}
