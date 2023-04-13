namespace TonLibDotNet.Cells
{
    public class BocFewCellsTests
    {
        /*
         * Data from https://tonscan.org/address/EQAiIsvar4OYBn8BGBf9flfin6tl5poBx4MgJe4CQJYasy51#source
         *   (Contract Type: Domain, domain: toncenter.ton)
         *   Data from "Raw Data" property
         *
         * The data is current at the time of writing the test, 09 April 2023
         *
         * Same data but as cells (decoded by tonscan.org):
            x{AFBE954CF1C028D02B3CEDF915D55249F2FCAD54808DB4FB91C7509677373CCF8016EE9B2BD640A87E30D80D66E357115AE09EFC4AC26195F2C3126D14FA196D99F00287E19A32F7884E4C38D5C1C4BE2D77673017355B5192C1EAFCFBC6B8C144367800000000C67349F5_}
             x{00C_}
              x{A01F75C083605883DA1FB149DF607378F01BAD5E943E18841E84AC35CE0BBB487FD_}
               x{AD01CB47963568ED1BFE385CDBACB22682ECC10C55D668741606008EAD83979551A100}
             x{746F6E63656E746572}
         */
        const string DataBase64 = @"
te6cckEBBQEAyQAC1a++lUzxwCjQKzzt+RXVUkny/K1UgI20+5HHUJZ3NzzPgBbumyvWQKh+MNgNZuNXEVrgnvxKwmGV8sMSbRT
6GW2Z8AKH4Zoy94hOTDjVwcS+LXdnMBc1W1GSwer8+8a4wUQ2eAAAAADGc0n1AQIBAwDAAwASdG9uY2VudGVyAUOgH3XAg2BYg9
ofsUnfYHN48ButXpQ+GIQehKw1zgu7SH/QBABGrQHLR5Y1aO0b/jhc26yyJoLswQxV1mh0FgYAjq2Dl5VRoQBD2Qwk";

        [Fact]
        public void ParseTest()
        {
            Assert.True(Boc.TryParseFromBase64(DataBase64, out var boc));
            Assert.NotNull(boc);
            Assert.Single(boc.RootCells);

            var root = boc.RootCells[0];

            var cell = root; // 02d5 afbe95..7349f5 0102
            Assert.Equal(
                Convert.FromHexString("AFBE954CF1C028D02B3CEDF915D55249F2FCAD54808DB4FB91C7509677373CCF8016EE9B2BD640A87E30D80D66E357115AE09EFC4AC26195F2C3126D14FA196D99F00287E19A32F7884E4C38D5C1C4BE2D77673017355B5192C1EAFCFBC6B8C144367800000000C67349F5"),
                cell.Content);
            Assert.True(cell.IsAugmented);
            Assert.Equal(214 * 4 - 1, cell.BitsCount); // length 0xD5=213 is odd so it's augmented, last byte 0xF5=0b1111_0101, so last 1 bit is not used
            Assert.Equal(2, cell.Refs.Count);

            cell = root.Refs[0]; // 0103 00c0 03
            Assert.Equal(
                Convert.FromHexString("00C0"), // replaced trailing _ with 0
                cell.Content);
            Assert.True(cell.IsAugmented);
            Assert.Equal(4 * 4 - 7, cell.BitsCount); // length 0x03=3 is odd so it's augmented, last byte 0xC0=0b1100_0000, so last 7 bits are not used
            Assert.Single(cell.Refs);

            cell = root.Refs[1]; // 0012 746f6e63656e746572
            Assert.Equal(
                Convert.FromHexString("746F6E63656E746572"),
                cell.Content);
            Assert.False(cell.IsAugmented);
            Assert.Equal(18 * 4, cell.BitsCount); // length 0x12=18 is even - not augmented
            Assert.Empty(cell.Refs);

            cell = root.Refs[0].Refs[0]; // 0143 a01f75c083605883da1fb149df607378f01bad5e943e18841e84ac35ce0bbb487fd0 04
            Assert.Equal(
                Convert.FromHexString("A01F75C083605883DA1FB149DF607378F01BAD5E943E18841E84AC35CE0BBB487FD0"), // replaced trailing _ with 0
                cell.Content);
            Assert.True(cell.IsAugmented);
            Assert.Equal(68 * 4 - 5, cell.BitsCount); // lenth 0x43=67 is odd so it's augmented, last byte 0xD0=0b1101_0000, so last 5 bits are not used
            Assert.Single(cell.Refs);

            cell = root.Refs[0].Refs[0].Refs[0]; // 0046 ad01cb47963568ed1bfe385cdbacb22682ecc10c55d668741606008ead83979551a100
            Assert.Equal(
                Convert.FromHexString("AD01CB47963568ED1BFE385CDBACB22682ECC10C55D668741606008EAD83979551A100"),
                cell.Content);
            Assert.False(cell.IsAugmented);
            Assert.Equal(70 * 4, cell.BitsCount); // length 0x46=70 is even - not augmented
            Assert.Empty(cell.Refs);
        }

        [Fact]
        public void SerializeTest()
        {
            Assert.True(Boc.TryParseFromBase64(DataBase64, out var boc));

            var ms = boc.Serialize(true);
            Assert.NotNull(ms);
            Assert.Equal(Convert.ToHexString(Convert.FromBase64String(DataBase64)), Convert.ToHexString(ms.ToArray()));
        }
    }
}
