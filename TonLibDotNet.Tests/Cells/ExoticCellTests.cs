namespace TonLibDotNet.Cells
{
    public class ExoticCellTests
    {
        private readonly Xunit.Abstractions.ITestOutputHelper outputHelper;

        public ExoticCellTests(Xunit.Abstractions.ITestOutputHelper testOutputHelper)
        {
            this.outputHelper = testOutputHelper;
        }

        [Fact]
        public void Issue9()
        {
            var source = "te6ccgECMwEABGoAAQgN9gLWAQlGA0I0rXIU3k/T5YpIO61lji2qYaoATvD6VByERzLig6MRAB4CIgWBcAIDBChIAQGscZahZEm3AZ4UdJ/dPrM78V0dRavHQpL6eB97AT6ldgAdIgEgBQYiASAHCChIAQHKGp/PF1wER/+4CaUIDKX/eiapHWm68vYD67aG7oIwLwAcKEgBAeTKJ8h5EGoDWsQX5S59aBqKEgGcUuslml9kIesPIr/xABoiASAJCihIAQGWtVL44MWCN4nl/16k7mooWkIJjktOgRcEunDS3G4YTAAZIgEgCwwiASANDihIAQFDMibwmVAs8NhR0vm0G4EXGAegDpqMaAKdOZZ1PUGZJQAXIgEgDxAoSAEBFMTW8x4iujqXyxnw8M83KBMqkBeg9hSxxJlKaLkyEEIAFihIAQFqEyscibISEP49oA3DFI7Fcfi2zrgrOknT+xRCrBp/cgAVIgEgERIoSAEBu07mjLXgshhCe8gFAFgsHjeW6/NqkV6Fxa14vTiI7PEAFCIBIBMUIgEgFRYoSAEBOIvEqlpTU8HT73NGqNTIyUfRO/BaYBRGBpMxfBxiFeEAEyIBIBcYKEgBAUh34ZMzMw36QL8MM4rIbf21AnC2DmiMjE0+Eh65fyL6ABEiASAZGihIAQEGTGRyw+F92fB5yTy6s98UTgmH7DBtHZdF8VBsPRklXAARIgEgGxwoSAEB/BfsrcrzVzEDfmc08kBJD/7BxVRla10dUnORaYpz6n0ADyhIAQHvse+SWPH2lxdldL8f459f7z7KvGWFcBPSbrGpRaV5XAAOIgEgHR4iASAfIChIAQF79/6raH+JRIIJDxvgQdONMUIl8iQP61pgZpPuvMz93AAOIgEgISIoSAEBYuNlD9WWXdgwDYNmaX0yyyJIgl/mVFvQ4k/IGHsqaLoADChIAQEAjwF4BPkGST68ndJz8VuEXANhYkaqFUFfdMNQY2W4aQALIgEgIyQoSAEB7RZAAe+jhAHJO+s3Y6pjPBk/tngCueKrAq9mkimXsiUACCIBICUmIgEgJygoSAEByDxVfFYpwwCYDWc25QK6r714aHKw5RZ9rs82FXvsrYwAByhIAQFS4LvxJSS5mzuQn7cnCOQ3z6cheqohnRtThY0mHsX6KwAGIgEgKSoiASArLChIAQFu6fvcPFG8zl5yRNjWIbkY12TL37CPuXe+AEmdv11h5AAFKEgBAd+Jh9SZp1gUOfTYKKQ6wdwq0c+FtJXImheyS/8DsjwnAAIiASAtLihIAQHN5Y49qCxJWMDRh+OrDf/PYSYtSwYpLkyjRiFjJQNNwwACIgEgLzAoSAEBvANMZEHZMeuHcivPXq5suoJLSyKv0cfU+OUXylmOoh8AACIBIDEyAGG6O8UAZQlH0cmyoTAQ+MguYNfwWNZkYDD7AOZL4rCYBgYFpCc8AAGb1MKAAAGyYyyCKEgBAZUuZYA8cQmbA3gOMBwAGz/KEhKvaCe2L5hiQuFEibfbAAI=";

            var boc = Boc.ParseFromBase64(source);
            outputHelper.WriteLine(boc.DumpCells());

            var result = boc.SerializeToBase64(false);
            Assert.Equal(source, result);
        }
    }
}
