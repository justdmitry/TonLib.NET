namespace TonLibDotNet.Cells
{
    public class BocSerializationTests
    {
        [Fact]
        public void SerializeWorks()
        {
            var cell = new CellBuilder()
                .StoreUInt(1)
                .StoreLong(12345)
                .StoreGrams(67890123)
                .Build();
            var boc = new Boc(cell);

            var base64 = Convert.ToBase64String(boc.Serialize(true).ToArray());
            Assert.Equal("te6cckEBAQEAEwAAIQAAAAEAAAAAAAAwOUBAvry4cwjMrQ==", base64);

            Assert.Equal(base64, boc.SerializeToBase64());
        }

        [Fact]
        public void SerializeToBase64Works()
        {
            var cell = new CellBuilder()
                .StoreUInt(1)
                .StoreLong(12345)
                .StoreGrams(67890123)
                .Build();
            var boc = new Boc(cell);

            var base64 = Convert.ToBase64String(boc.Serialize(true).ToArray());

            Assert.Equal(base64, boc.SerializeToBase64());
        }
    }
}
