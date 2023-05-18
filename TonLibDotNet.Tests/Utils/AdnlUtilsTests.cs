namespace TonLibDotNet.Utils
{
    public class AdnlUtilsTests
    {
        [Theory]
        [InlineData("UWYYz2y+kAT2iD50LJouPKU+0C4+NvTO9iqY7h5EkXQ=", "viwmggpns7jabhwra7hile2fy6kkpwqfy7dn5go6yvjr3q6isixjwkf")]
        public void Encode(string source, string result)
        {
            var bytes = Convert.FromBase64String(source);
            Assert.Equal(result, AdnlUtils.Instance.Encode(bytes));
        }

        [Theory]
        [InlineData("viwmggpns7jabhwra7hile2fy6kkpwqfy7dn5go6yvjr3q6isixjwkf", "UWYYz2y+kAT2iD50LJouPKU+0C4+NvTO9iqY7h5EkXQ=")]
        public void Decode(string source, string result)
        {
            var bytes = AdnlUtils.Instance.Decode(source);
            Assert.Equal(result, Convert.ToBase64String(bytes));
        }
    }
}
