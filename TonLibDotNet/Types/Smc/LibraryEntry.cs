namespace TonLibDotNet.Types.Smc
{
    [TLSchema("smc.libraryEntry hash:int256 data:bytes = smc.LibraryEntry")]
    public class LibraryEntry : TypeBase
    {
        public string Hash { get; set; } = string.Empty;

        public string Bytes { get; set; } = string.Empty;
    }
}
