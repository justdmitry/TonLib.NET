namespace TonLibDotNet.Data
{
    public class BocManyCellsTests
    {
        /*
         * Data from https://tonscan.org/address/EQAiIsvar4OYBn8BGBf9flfin6tl5poBx4MgJe4CQJYasy51#source
         *   (Contract Type: Domain, domain: toncenter.ton)
         *   Data from "Bytecode" property
         *
         * The data is current at the time of writing the test, 09 April 2023
         *
         * Same data but as cells (decoded by tonscan.org), first 10 cells of 44:
            x{FF00F4A413F4BCF2C80B}
             x{62_}
              x{CC}
               x{2_}
                x{2_}
                 x{2_}
                  x{3E09DBC400B434C0C05C6C2497C1383E903E900C7E800C5C75C87E800C7E800C3C0289ECE39497C15B088D148CB1C17CB865407E90350C1B5C3232C1FD00327E08E08418B93CC428608209E3402A4108308324CC200337A082024EA02082024B1C162A20032A41287E08C0683C00911DFC02440D7E08FC02F814D66_}
                   x{C70518B08E34109B5F0BFA40307020F8256D8040708010C8CB055007CF165005FA0215CB6A12CB1FCB3F226EB39458CF17019132E201C901FB00E029C70091709509D31F50AAE221F008F82321BC24C0008E9E343A3A3B8E1636363737375135C705F2E196102510241023F823F00BE30EE0310DD33F256EB31FB0E30F}
                    x{302680698064A98452B0BEF2E19782103B9ACA0052A0A15270BC993682103B9ACA0019A193390805E220C2008E328210557CEA20F82510396D71708010C8CB055007CF165005FA0215CB6A12CB1FCB3F226EB39458CF17019132E201C901FB00923036E2810E1023F823A1A120C2009313A0029130E24474F0091024F823}
                     x{F00B}
         */
        const string DataBase64 = @"
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
fQmDwAwumZSOw==";

        [Fact]
        public void ParseTest()
        {
            Assert.True(Boc.TryParseFromBase64(DataBase64, out var boc));
            Assert.NotNull(boc);
            Assert.Single(boc.RootCells);

            var root = boc.RootCells[0];

            var cell = root;
            Assert.Equal(
                Convert.FromHexString("FF00F4A413F4BCF2C80B"),
                cell.Content);
            Assert.False(cell.IsAugmented);
            Assert.Equal(80, cell.BitsCount);
            Assert.Single(cell.Refs);

            cell = root.Refs[0];
            Assert.Equal(
                Convert.FromHexString("62"),
                cell.Content);
            Assert.True(cell.IsAugmented);
            Assert.Equal(6, cell.BitsCount);
            Assert.Equal(2, cell.Refs.Count);

            cell = root.Refs[0].Refs[0];
            Assert.Equal(
                Convert.FromHexString("CC"),
                cell.Content);
            Assert.False(cell.IsAugmented);
            Assert.Equal(8, cell.BitsCount);
            Assert.Equal(2, cell.Refs.Count);

            cell = root.Refs[0].Refs[0].Refs[0];
            Assert.Equal(
                Convert.FromHexString("20"),
                cell.Content);
            Assert.True(cell.IsAugmented);
            Assert.Equal(2, cell.BitsCount);
            Assert.Equal(2, cell.Refs.Count);

            cell = root.Refs[0].Refs[0].Refs[0].Refs[0];
            Assert.Equal(
                Convert.FromHexString("20"),
                cell.Content);
            Assert.True(cell.IsAugmented);
            Assert.Equal(2, cell.BitsCount);

            cell = root.Refs[0].Refs[0].Refs[0].Refs[0].Refs[0];
            Assert.Equal(
                Convert.FromHexString("20"),
                cell.Content);
            Assert.True(cell.IsAugmented);
            Assert.Equal(2, cell.BitsCount);

            cell = root.Refs[0].Refs[0].Refs[0].Refs[0].Refs[0].Refs[0];
            Assert.Equal(
                Convert.FromHexString("3E09DBC400B434C0C05C6C2497C1383E903E900C7E800C5C75C87E800C7E800C3C0289ECE39497C15B088D148CB1C17CB865407E90350C1B5C3232C1FD00327E08E08418B93CC428608209E3402A4108308324CC200337A082024EA02082024B1C162A20032A41287E08C0683C00911DFC02440D7E08FC02F814D660"),
                cell.Content);
            Assert.True(cell.IsAugmented);
            Assert.Equal(246 * 4 + 2, cell.BitsCount);

            cell = root.Refs[0].Refs[0].Refs[0].Refs[0].Refs[0].Refs[0].Refs[0];
            Assert.Equal(
                Convert.FromHexString("C70518B08E34109B5F0BFA40307020F8256D8040708010C8CB055007CF165005FA0215CB6A12CB1FCB3F226EB39458CF17019132E201C901FB00E029C70091709509D31F50AAE221F008F82321BC24C0008E9E343A3A3B8E1636363737375135C705F2E196102510241023F823F00BE30EE0310DD33F256EB31FB0E30F"),
                cell.Content);
            Assert.False(cell.IsAugmented);
            Assert.Equal(250 * 4, cell.BitsCount);

            cell = root.Refs[0].Refs[0].Refs[0].Refs[0].Refs[0].Refs[0].Refs[0].Refs[0];
            Assert.Equal(
                Convert.FromHexString("302680698064A98452B0BEF2E19782103B9ACA0052A0A15270BC993682103B9ACA0019A193390805E220C2008E328210557CEA20F82510396D71708010C8CB055007CF165005FA0215CB6A12CB1FCB3F226EB39458CF17019132E201C901FB00923036E2810E1023F823A1A120C2009313A0029130E24474F0091024F823"),
                cell.Content);
            Assert.False(cell.IsAugmented);
            Assert.Equal(252 * 4, cell.BitsCount);

            cell = root.Refs[0].Refs[0].Refs[0].Refs[0].Refs[0].Refs[0].Refs[0].Refs[0].Refs[0];
            Assert.Equal(
                Convert.FromHexString("F00B"),
                cell.Content);
            Assert.False(cell.IsAugmented);
            Assert.Equal(4 * 4, cell.BitsCount);
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
