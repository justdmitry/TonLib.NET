using TonLibDotNet.Types.Smc;

namespace TonLibDotNet.Requests.Smc
{
    [TLSchema("smc.getLibraries library_list:(vector int256) = smc.LibraryResult")]
    public class GetLibraries : RequestBase<LibraryResult>
    {
        public GetLibraries(params string[] libraryList)
        {
            // I hadn't found any sample addresses to check their data and make a sample.
            // Please open ticket if you have any.
            throw new NotImplementedException();

            //LibraryList = libraryList;
        }

        public string[] LibraryList { get; set; }
    }
}
