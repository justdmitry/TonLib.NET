namespace TonLibDotNet.Utils
{
    public class AddressUtilsTests
    {
        [Theory]
        [InlineData("EQBYTuYbLf8INxFtD8tQeNk5ZLy-nAX9ahQbG_yl1qQ-GEMS", true, "EQBYTuYbLf8INxFtD8tQeNk5ZLy-nAX9ahQbG_yl1qQ-GEMS")]
        [InlineData("EQBYTuYbLf8INxFtD8tQeNk5ZLy-nAX9ahQbG_yl1qQ-GEMS", false, "UQBYTuYbLf8INxFtD8tQeNk5ZLy-nAX9ahQbG_yl1qQ-GB7X")]
        [InlineData("UQBYTuYbLf8INxFtD8tQeNk5ZLy-nAX9ahQbG_yl1qQ-GB7X", false, "UQBYTuYbLf8INxFtD8tQeNk5ZLy-nAX9ahQbG_yl1qQ-GB7X")]
        [InlineData("UQBYTuYbLf8INxFtD8tQeNk5ZLy-nAX9ahQbG_yl1qQ-GB7X", true, "EQBYTuYbLf8INxFtD8tQeNk5ZLy-nAX9ahQbG_yl1qQ-GEMS")]
        public void SetBounceableOk(string source, bool flag, string expected)
        {
            Assert.Equal(expected, AddressUtils.Instance.SetBounceable(source, flag));
        }
    }
}
