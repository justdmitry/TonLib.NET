namespace TonLibDotNet.Utils
{
    public class AddressValidatorParseTests
    {
        [Fact]
        public void ParseDefault()
        {
            // Wallet Bot
            Assert.True(AddressValidator.TryParseAddress("EQDd3NPNrWCvTA1pOJ9WetUdDCY_pJaNZVq0JMaara-TIp90", out var workchainId, out var accountId, out var bounceable, out var testnetOnly, out var urlSafe));

            Assert.Equal(0, workchainId);
            Assert.Equal("dddcd3cdad60af4c0d69389f567ad51d0c263fa4968d655ab424c69aadaf9322", Convert.ToHexString(accountId), true);
            Assert.True(bounceable);
            Assert.False(testnetOnly);
            Assert.True(urlSafe);
        }

        [Fact]
        public void ParseMasterchain()
        {
            // BSC Bridge
            Assert.True(AddressValidator.TryParseAddress("Ef9NXAIQs12t2qIZ-sRZ26D977H65Ol6DQeXc5_gUNaUys5r", out var workchainId, out var accountId, out var bounceable, out var testnetOnly, out var urlSafe));

            Assert.Equal(AddressValidator.MasterchainId, workchainId);
            Assert.Equal("4d5c0210b35daddaa219fac459dba0fdefb1fae4e97a0d0797739fe050d694ca", Convert.ToHexString(accountId), true);
        }

        [Fact]
        public void ParseNonBounceable()
        {
            // Wallet Bot
            Assert.True(AddressValidator.TryParseAddress("UQDd3NPNrWCvTA1pOJ9WetUdDCY_pJaNZVq0JMaara-TIsKx", out var workchainId, out var accountId, out var bounceable, out var testnetOnly, out var urlSafe));

            Assert.Equal(0, workchainId);
            Assert.Equal("dddcd3cdad60af4c0d69389f567ad51d0c263fa4968d655ab424c69aadaf9322", Convert.ToHexString(accountId), true);
            Assert.False(bounceable);
        }

        [Fact]
        public void ParseUrlUnsafe()
        {
            // Wallet Bot
            Assert.True(AddressValidator.TryParseAddress("EQDd3NPNrWCvTA1pOJ9WetUdDCY/pJaNZVq0JMaara+TIp90", out var workchainId, out var accountId, out var bounceable, out var testnetOnly, out var urlSafe));

            Assert.Equal(0, workchainId);
            Assert.Equal("dddcd3cdad60af4c0d69389f567ad51d0c263fa4968d655ab424c69aadaf9322", Convert.ToHexString(accountId), true);
            Assert.False(urlSafe);
        }

        [Fact]
        public void FailsOnWrongStringLength()
        {
            Assert.False(AddressValidator.TryParseAddress("EQDd3NPNrWCvTA1pOJ9WetUdDC", out _, out _, out _, out _, out _));
        }

        [Fact]
        public void FailsOnNonBase64()
        {
            // Wallet Bot, with '~' in first position
            Assert.False(AddressValidator.TryParseAddress("~QDd3NPNrWCvTA1pOJ9WetUdDCY_pJaNZVq0JMaara-TIp90", out _, out _, out _, out _, out _));
        }

        [Fact]
        public void FailsOnWrongChecksum()
        {
            // Wallet Bot, but last char changed from 0 to 1
            Assert.False(AddressValidator.TryParseAddress("EQDd3NPNrWCvTA1pOJ9WetUdDCY_pJaNZVq0JMaara-TIp91", out _, out _, out _, out _, out _));
        }

        [Fact]
        public void FailsOnSafeUnsafeMix()
        {
            // Wallet Bot, with '_' changed to '+', but '_' not changed to '/'
            Assert.False(AddressValidator.TryParseAddress("EQDd3NPNrWCvTA1pOJ9WetUdDCY_pJaNZVq0JMaara+TIp90", out _, out _, out _, out _, out _));
        }
    }
}
