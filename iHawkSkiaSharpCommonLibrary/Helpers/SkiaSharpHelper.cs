using iHawkSkiaSharpCommonLibrary.Entities;
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

        public SKBitmap DrawTextToImage(string text, int width, int height, SKColor backgroundColor, SKColor textColor, int textSize, GlyphPreviewParam param)
        {
            // 创建一个新的位图
            var bitmap = new SKBitmap(width, height);

            // 创建一个画布
            using (var canvas = new SKCanvas(bitmap))
            {
                // 填充背景颜色
                canvas.Clear(backgroundColor);

                // 创建画笔
                using var paint = new SKPaint
                {
                    Color = textColor,
                    IsAntialias = true,
                    TextSize = textSize,
                    Typeface = _typeface
                };

                // 计算文本的绘制位置
                var textBounds = new SKRect();
                paint.MeasureText(text, ref textBounds);

                // 获取字体度量信息
                var fontMetrics = paint.FontMetrics;
                var baseline = (height - textBounds.Height) / 2 - fontMetrics.Ascent;

                // 绘制文本
                var x = (width - textBounds.Width) / 2;
                canvas.DrawText(text, x, baseline, paint);

                // 绘制基线
                using var paint4Baseline = new SKPaint
                {
                    Color = SKColors.Black,
                    IsAntialias = true,
                    PathEffect = SKPathEffect.CreateDash([3, 3], 0),
                    StrokeWidth = 1
                };
                canvas.DrawLine(0, baseline, width, baseline, paint4Baseline);

                // 获取字体表信息
                var table = GetFontTable(new List<string> { "head", "hhea", "OS/2" });

                paint.Color = SKColors.Red;
                paint.StrokeWidth = 2;
                paint.IsAntialias = true;

                // 绘制参考线
                var em = table!.HeadTable!.unitsPerEm;
                if (param.YMinMaxVisible && table?.HeadTable != null)
                {
                    using var paint4YMinMax = new SKPaint
                    {
                        Color = SKColors.Black,
                        IsAntialias = true,
                        StrokeWidth = 2
                    };
                    DrawReferenceLine(canvas, paint4YMinMax, table.HeadTable.yMin, width, baseline, textSize, em);
                    DrawReferenceLine(canvas, paint4YMinMax, table.HeadTable.yMax, width, baseline, textSize, em);
                }
                if (param.HheaAscDescVisible && table?.HheaTable != null)
                {
                    using var paint4Hhea = new SKPaint
                    {
                        Color = SKColors.Red,
                        IsAntialias = true,
                        StrokeWidth = 1
                    };
                    DrawReferenceLine(canvas, paint4Hhea, table.HheaTable.ascender, width, baseline, textSize, em);
                    DrawReferenceLine(canvas, paint4Hhea, table.HheaTable.descender, width, baseline, textSize, em);
                }
                if (param.TypoAscDescVisible && table?.Os2Table != null)
                {
                    using var paint4Typo = new SKPaint
                    {
                        Color = SKColors.Lime,
                        IsAntialias = true,
                        StrokeWidth = 1
                    };
                    DrawReferenceLine(canvas, paint4Typo, table.Os2Table.sTypoAscender, width, baseline, textSize, em);
                    DrawReferenceLine(canvas, paint4Typo, table.Os2Table.sTypoDescender, width, baseline, textSize, em);
                }
                if (param.WinAscDescVisible && table?.Os2Table != null)
                {
                    using var paint4Win = new SKPaint
                    {
                        Color = SKColors.Blue,
                        IsAntialias = true,
                        StrokeWidth = 1
                    };
                    DrawReferenceLine(canvas, paint4Win, table.Os2Table.usWinAscent, width, baseline, textSize, em);
                    DrawReferenceLine(canvas, paint4Win, -table.Os2Table.usWinDescent, width, baseline, textSize, em);
                }
            }
            return bitmap;
        }

        public SKBitmap DrawTextToImage(List<string> lines, int width, int height, SKColor backgroundColor, SKColor textColor, int textSize, GlyphPreviewParam param)
        {
            // 创建一个新的位图
            var bitmap = new SKBitmap(width, height);

            // 创建一个画布
            using (var canvas = new SKCanvas(bitmap))
            {
                // 填充背景颜色
                canvas.Clear(backgroundColor);

                // 创建画笔
                using var paint = new SKPaint
                {
                    Color = textColor,
                    IsAntialias = true,
                    TextSize = textSize,
                    Typeface = _typeface
                };

                // 获取字体度量信息
                var fontMetrics = paint.FontMetrics;
                var baselineOffet = fontMetrics.Ascent;
                var lineheight = Math.Abs(fontMetrics.Ascent) + Math.Abs(fontMetrics.Descent);
                switch (param.LineGapTag)
                {
                    case "YMinMax":
                        baselineOffet = fontMetrics.Top;
                        lineheight = Math.Abs(fontMetrics.Top) + Math.Abs(fontMetrics.Bottom);
                        break;
                    case "AscDesc":
                        break;
                }

                // 获取字体表信息
                var table = GetFontTable(new List<string> { "head", "hhea", "OS/2" });

                // 计算每行文本的绘制位置
                var y = Math.Abs(fontMetrics.Ascent - fontMetrics.Top);// 0f;
                var linenum = 0;
                foreach (var line in lines)
                {
                    var textBounds = new SKRect();
                    paint.MeasureText(line, ref textBounds);

                    // 确认基线位置
                    var baseline = y - baselineOffet;

                    // 绘制文本
                    var x = (width - textBounds.Width) / 2;
                    canvas.DrawText(line, x, baseline, paint);

                    // 绘制基线
                    using var paint4Baseline = new SKPaint
                    {
                        Color = SKColors.Black,
                        IsAntialias = true,
                        PathEffect = SKPathEffect.CreateDash([3, 3], 0),
                        StrokeWidth = 1
                    };
                    canvas.DrawLine(0, baseline, width, baseline, paint4Baseline);

                    // 绘制参考线
                    var em = table!.HeadTable!.unitsPerEm;
                    if (param.YMinMaxVisible && table?.HeadTable != null)
                    {
                        using var paint4YMinMax = new SKPaint
                        {
                            Color = SKColors.Black,
                            IsAntialias = true,
                            StrokeWidth = 2
                        };
                        DrawReferenceLine(canvas, paint4YMinMax, table.HeadTable.yMin, width, baseline, textSize, em);
                        DrawReferenceLine(canvas, paint4YMinMax, table.HeadTable.yMax, width, baseline, textSize, em);
                    }
                    if (param.HheaAscDescVisible && table?.HheaTable != null)
                    {
                        using var paint4Hhea = new SKPaint
                        {
                            Color = SKColors.Red,
                            IsAntialias = true,
                            StrokeWidth = 1
                        };
                        DrawReferenceLine(canvas, paint4Hhea, table.HheaTable.ascender, width, baseline, textSize, em);
                        DrawReferenceLine(canvas, paint4Hhea, table.HheaTable.descender, width, baseline, textSize, em);
                    }
                    if (param.TypoAscDescVisible && table?.Os2Table != null)
                    {
                        using var paint4Typo = new SKPaint
                        {
                            Color = SKColors.Lime,
                            IsAntialias = true,
                            StrokeWidth = 1
                        };
                        DrawReferenceLine(canvas, paint4Typo, table.Os2Table.sTypoAscender, width, baseline, textSize, em);
                        DrawReferenceLine(canvas, paint4Typo, table.Os2Table.sTypoDescender, width, baseline, textSize, em);
                    }
                    if (param.WinAscDescVisible && table?.Os2Table != null)
                    {
                        using var paint4Win = new SKPaint
                        {
                            Color = SKColors.Blue,
                            IsAntialias = true,
                            StrokeWidth = 1
                        };
                        DrawReferenceLine(canvas, paint4Win, table.Os2Table.usWinAscent, width, baseline, textSize, em);
                        DrawReferenceLine(canvas, paint4Win, -table.Os2Table.usWinDescent, width, baseline, textSize, em);
                    }

                    // 更新y坐标，准备绘制下一行
                    y += lineheight;
                    //y += textBounds.Height + fontMetrics.Leading;
                    //y += Math.Abs(fontMetrics.Ascent) + Math.Abs(fontMetrics.Descent);
                    //y += Math.Abs(fontMetrics.Top) + Math.Abs(fontMetrics.Bottom);
                }
            }
            return bitmap;
        }

        private void DrawReferenceLine(SKCanvas canvas, SKPaint paint, int value, int width, float baseline, int textSize, int unitsPerEm)
        {
            float y = baseline - (value * textSize / unitsPerEm); // 将参考线绘制在基线位置
            canvas.DrawLine(0, y, width, y, paint);
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
