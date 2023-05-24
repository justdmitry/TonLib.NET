namespace TonLibDotNet.Utils
{
    public class TonLibNonZeroExitCodeException : Exception
    {
        public TonLibNonZeroExitCodeException(long exitCode)
            : base($"Non-zero ExitCode received ({exitCode}) from TonLib. Something went wrong.")
        {
            if (exitCode == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(exitCode), "Must be not equal to zero.");
            }

            ExitCode = exitCode;
        }

        public long ExitCode { get; set; }
    }
}
