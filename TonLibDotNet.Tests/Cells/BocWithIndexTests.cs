namespace TonLibDotNet.Cells
{
    public class BocWithIndexTests
    {
        // Init BOC of some testnet contract
        const string DataBase64 = "te6ccuEBAQEAAgAEAABmLc6k";

        [Fact]
        public void ParseTest()
        {
            var boc = Boc.ParseFromBase64(DataBase64);
            Assert.NotNull(boc);
            Assert.Single(boc.RootCells);
        }
    }
}
