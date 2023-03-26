namespace TonLibDotNet.Types.Smc
{
    [TLSchema("smc.libraryResult result:(vector smc.libraryEntry) = smc.LibraryResult")]
    public class LibraryResult : TypeBase
    {
        public LibraryResult()
        {
            // I hadn't found any sample addresses to check their data and make a sample.
            // Please open ticket if you have any.
            throw new NotImplementedException();
        }
    }
}
