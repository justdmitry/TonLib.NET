namespace TonLibDotNet.Utils
{
    public class TextUtilsTexts
    {
        [Theory]
        [InlineData("Hello, World!", "SGVsbG8sIFdvcmxkIQ==")]
        public void EncodeAsBase64(string source, string encoded)
        {
            Assert.Equal(encoded, TextUtils.Instance.EncodeAsBase64(source));
        }

        [Fact]
        public void EncodeAsBase64ForEmpty()
        {
            Assert.Equal(string.Empty, TextUtils.Instance.EncodeAsBase64(string.Empty));
        }

        [Fact]
        public void EncodeAsBase64ForNull()
        {
            Assert.Null(TextUtils.Instance.EncodeAsBase64(null));
        }

        [Theory]
        [InlineData("SGVsbG8sIFdvcmxkIQ==", true, "Hello, World!")]
        [InlineData("SGVsbG8s\r\nIFdvcmxkIQ==", true, "Hello, World!")]
        [InlineData("SGVsbG8sIFdvcmxkIQ===", false, "")]
        [InlineData("SGVsbG8sIFdvcmxkIQ=", false, "")]
        [InlineData("Hello, World!", false, "")]
        public void TryDecodeBase64(string source, bool success, string decoded)
        {
            Assert.Equal(success, TextUtils.Instance.TryDecodeBase64(source, out var value));

            if (success)
            {
                Assert.Equal(decoded, value);
            }
        }

        [Fact]
        public void TryDecodeBase64ForEmpty()
        {
            Assert.False(TextUtils.Instance.TryDecodeBase64(string.Empty, out _));
        }

        [Fact]
        public void TryDecodeBase64ForNull()
        {
            Assert.False(TextUtils.Instance.TryDecodeBase64(null, out _));
        }
    }
}
