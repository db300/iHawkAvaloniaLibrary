using iHawkSkiaSharpCommonLibrary.Entities.FontTableEntities;
using SkiaSharp;
using System.Text;

namespace iHawkSkiaSharpCommonLibrary.Helpers
{
    public class SkiaSharpHelper
    {
        #region constructor
        public SkiaSharpHelper(string filename)
        {
            _typeface = SKTypeface.FromFile(filename);
        }
        #endregion

        #region property
        private readonly SKTypeface? _typeface;
        private List<string>? _tableTags;
        #endregion

        #region method
        public FontTable? GetFontTable(List<string> fontTableTags)
        {
            _tableTags = GetTableTags();
            if (!(_tableTags?.Count > 0)) return null;

            var fontTable = new FontTable();
            foreach (var fontTableTag in fontTableTags)
            {
                if (!_tableTags.Contains(fontTableTag)) continue;
                switch (fontTableTag)
                {
                    case "cmap":
                        break;
                    case "glyf":
                        break;
                    case "head":
                        if (fontTable.HeadTable != null) break;
                        fontTable.HeadTable = GetHeadTable();
                        break;
                    case "hhea":
                        if (fontTable.HheaTable != null) break;
                        fontTable.HheaTable = GetHheaTable();
                        break;
                    case "hmtx":
                        break;
                    case "loca":
                        break;
                    case "maxp":
                        break;
                    case "name":
                        break;
                    case "post":
                        break;
                    case "OS/2":
                        if (fontTable.Os2Table != null) break;
                        fontTable.Os2Table = GetOs2Table();
                        break;
                }
            }
            return fontTable;
        }

        public string GetTypeSetting()
        {
            var table = GetFontTable(new List<string> { "head", "hhea", "OS/2" });
            var sb = new StringBuilder();
            sb.AppendLine($"yMin: {table?.HeadTable?.yMin}");
            sb.AppendLine($"yMax: {table?.HeadTable?.yMax}");
            sb.AppendLine($"ascender: {table?.HheaTable?.ascender}");
            sb.AppendLine($"descender: {table?.HheaTable?.descender}");
            sb.AppendLine($"usWinAscent: {table?.Os2Table?.usWinAscent}");
            sb.AppendLine($"usWinDescent: {table?.Os2Table?.usWinDescent}");
            sb.AppendLine($"sTypoAscender: {table?.Os2Table?.sTypoAscender}");
            sb.AppendLine($"sTypoDescender: {table?.Os2Table?.sTypoDescender}");
            return sb.ToString();
        }

        private List<string>? GetTableTags()
        {
            var tableTags = _typeface?.GetTableTags();
            return tableTags?.Select(UIntToString).ToList();
        }

        private static string UIntToString(uint value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
            return Encoding.ASCII.GetString(bytes);
        }

        private HeadTable? GetHeadTable()
        {
            var headTable = new HeadTable();
            var tableData = _typeface?.GetTableData(0x68656164);//head
            if (tableData is null) return null;
            var offset = 8;//TODO:暂时跳过version和fontRevision
            headTable.checkSumAdjustment = BitConverter.ToUInt32(tableData.Skip(offset).Take(4).Reverse().ToArray());
            offset += 4;
            headTable.magicNumber = BitConverter.ToUInt32(tableData.Skip(offset).Take(4).Reverse().ToArray());
            offset += 4;
            headTable.flags = BitConverter.ToUInt16(tableData.Skip(offset).Take(2).Reverse().ToArray());
            offset += 2;
            headTable.unitsPerEm = BitConverter.ToUInt16(tableData.Skip(offset).Take(2).Reverse().ToArray());
            offset += 2;
            offset += 16;//TODO:暂时跳过created和modified
            headTable.xMin = BitConverter.ToInt16(tableData.Skip(offset).Take(2).Reverse().ToArray());
            offset += 2;
            headTable.yMin = BitConverter.ToInt16(tableData.Skip(offset).Take(2).Reverse().ToArray());
            offset += 2;
            headTable.xMax = BitConverter.ToInt16(tableData.Skip(offset).Take(2).Reverse().ToArray());
            offset += 2;
            headTable.yMax = BitConverter.ToInt16(tableData.Skip(offset).Take(2).Reverse().ToArray());
            offset += 2;
            headTable.macStyle = BitConverter.ToUInt16(tableData.Skip(offset).Take(2).Reverse().ToArray());
            offset += 2;
            headTable.lowestRecPPEM = BitConverter.ToUInt16(tableData.Skip(offset).Take(2).Reverse().ToArray());
            offset += 2;
            headTable.fontDirectionHint = BitConverter.ToInt16(tableData.Skip(offset).Take(2).Reverse().ToArray());
            offset += 2;
            headTable.indexToLocFormat = BitConverter.ToInt16(tableData.Skip(offset).Take(2).Reverse().ToArray());
            offset += 2;
            headTable.glyphDataFormt = BitConverter.ToInt16(tableData.Skip(offset).Take(2).Reverse().ToArray());
            return headTable;
        }

        private HheaTable? GetHheaTable()
        {
            var hheaTable = new HheaTable();
            var tableData = _typeface?.GetTableData(0x68686561);//hhea
            if (tableData is null) return null;
            var offset = 4;//TODO:暂时跳过version
            hheaTable.ascender = BitConverter.ToInt16(tableData.Skip(offset).Take(2).Reverse().ToArray());
            offset += 2;
            hheaTable.descender = BitConverter.ToInt16(tableData.Skip(offset).Take(2).Reverse().ToArray());
            offset += 2;
            hheaTable.lineGap = BitConverter.ToInt16(tableData.Skip(offset).Take(2).Reverse().ToArray());
            return hheaTable;
        }

        private Os2Table? GetOs2Table()
        {
            var os2Table = new Os2Table();
            var tableData = _typeface?.GetTableData(0x4F532F32);//OS/2
            if (tableData is null) return null;
            var offset = 2;//TODO:暂时跳过version
            os2Table.xAvgCharWidth = BitConverter.ToInt16(tableData.Skip(offset).Take(2).Reverse().ToArray());
            offset += 2;
            os2Table.usWeightClass = BitConverter.ToUInt16(tableData.Skip(offset).Take(2).Reverse().ToArray());
            offset += 2;
            os2Table.usWidthClass = BitConverter.ToUInt16(tableData.Skip(offset).Take(2).Reverse().ToArray());
            offset += 2;
            os2Table.fsType = BitConverter.ToInt16(tableData.Skip(offset).Take(2).Reverse().ToArray());
            offset += 2;
            os2Table.ySubscriptXSize = BitConverter.ToInt16(tableData.Skip(offset).Take(2).Reverse().ToArray());
            offset += 2;
            os2Table.ySubscriptYSize = BitConverter.ToInt16(tableData.Skip(offset).Take(2).Reverse().ToArray());
            offset += 2;
            os2Table.ySubscriptXOffset = BitConverter.ToInt16(tableData.Skip(offset).Take(2).Reverse().ToArray());
            offset += 2;
            os2Table.ySubscriptYOffset = BitConverter.ToInt16(tableData.Skip(offset).Take(2).Reverse().ToArray());
            offset += 2;
            os2Table.ySuperscriptXSize = BitConverter.ToInt16(tableData.Skip(offset).Take(2).Reverse().ToArray());
            offset += 2;
            os2Table.ySuperscriptYSize = BitConverter.ToInt16(tableData.Skip(offset).Take(2).Reverse().ToArray());
            offset += 2;
            os2Table.ySuperscriptXOffset = BitConverter.ToInt16(tableData.Skip(offset).Take(2).Reverse().ToArray());
            offset += 2;
            os2Table.ySuperscriptYOffset = BitConverter.ToInt16(tableData.Skip(offset).Take(2).Reverse().ToArray());
            offset += 2;
            os2Table.yStrikeoutSize = BitConverter.ToInt16(tableData.Skip(offset).Take(2).Reverse().ToArray());
            offset += 2;
            os2Table.yStrikeoutPosition = BitConverter.ToInt16(tableData.Skip(offset).Take(2).Reverse().ToArray());
            offset += 2;
            os2Table.sFamilyClass = BitConverter.ToInt16(tableData.Skip(offset).Take(2).Reverse().ToArray());
            offset += 2;
            os2Table.panose = GetOs2TablePanose(tableData.Skip(offset).Take(10).ToArray());
            offset += 10;
            os2Table.ulUnicodeRange1 = BitConverter.ToUInt32(tableData.Skip(offset).Take(4).Reverse().ToArray());
            offset += 4;
            os2Table.ulUnicodeRange2 = BitConverter.ToUInt32(tableData.Skip(offset).Take(4).Reverse().ToArray());
            offset += 4;
            os2Table.ulUnicodeRange3 = BitConverter.ToUInt32(tableData.Skip(offset).Take(4).Reverse().ToArray());
            offset += 4;
            os2Table.ulUnicodeRange4 = BitConverter.ToUInt32(tableData.Skip(offset).Take(4).Reverse().ToArray());
            offset += 4;
            os2Table.achVendID = Encoding.ASCII.GetString(tableData.Skip(offset).Take(4).ToArray());
            offset += 4;
            os2Table.fsSelection = BitConverter.ToUInt16(tableData.Skip(offset).Take(2).Reverse().ToArray());
            offset += 2;
            os2Table.usFirstCharIndex = BitConverter.ToUInt16(tableData.Skip(offset).Take(2).Reverse().ToArray());
            offset += 2;
            os2Table.usLastCharIndex = BitConverter.ToUInt16(tableData.Skip(offset).Take(2).Reverse().ToArray());
            offset += 2;
            os2Table.sTypoAscender = BitConverter.ToInt16(tableData.Skip(offset).Take(2).Reverse().ToArray());
            offset += 2;
            os2Table.sTypoDescender = BitConverter.ToInt16(tableData.Skip(offset).Take(2).Reverse().ToArray());
            offset += 2;
            os2Table.sTypoLineGap = BitConverter.ToInt16(tableData.Skip(offset).Take(2).Reverse().ToArray());
            offset += 2;
            os2Table.usWinAscent = BitConverter.ToUInt16(tableData.Skip(offset).Take(2).Reverse().ToArray());
            offset += 2;
            os2Table.usWinDescent = BitConverter.ToUInt16(tableData.Skip(offset).Take(2).Reverse().ToArray());
            offset += 2;
            os2Table.ulCodePageRange1 = BitConverter.ToUInt32(tableData.Skip(offset).Take(4).Reverse().ToArray());
            offset += 4;
            os2Table.ulCodePageRange2 = BitConverter.ToUInt32(tableData.Skip(offset).Take(4).Reverse().ToArray());
            offset += 4;
            os2Table.sxHeight = BitConverter.ToInt16(tableData.Skip(offset).Take(2).Reverse().ToArray());
            offset += 2;
            os2Table.sCapHeight = BitConverter.ToInt16(tableData.Skip(offset).Take(2).Reverse().ToArray());
            offset += 2;
            os2Table.usDefaultChar = BitConverter.ToUInt16(tableData.Skip(offset).Take(2).Reverse().ToArray());
            offset += 2;
            os2Table.usBreakChar = BitConverter.ToUInt16(tableData.Skip(offset).Take(2).Reverse().ToArray());
            offset += 2;
            os2Table.usMaxContext = BitConverter.ToUInt16(tableData.Skip(offset).Take(2).Reverse().ToArray());
            return os2Table;
        }

        private static Panose GetOs2TablePanose(byte[] buf)
        {
            var panose = new Panose
            {
                bFamilyType = buf[0],
                bSerifStyle = buf[1],
                bWeight = buf[2],
                bProportion = buf[3],
                bContrast = buf[4],
                bStrokeVariation = buf[5],
                bArmStyle = buf[6],
                bLetterform = buf[7],
                bMidline = buf[8],
                bXHeight = buf[9]
            };
            return panose;
        }
        #endregion
    }
}
