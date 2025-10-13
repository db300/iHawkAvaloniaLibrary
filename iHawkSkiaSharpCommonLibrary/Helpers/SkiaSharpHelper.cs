using iHawkSkiaSharpCommonLibrary.Entities;
using iHawkSkiaSharpCommonLibrary.Entities.FontTableEntities;
using SkiaSharp;
using System.Buffers.Binary;
using System.Linq;
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
        private readonly FontTable _fontTable = new FontTable();
        private List<string>? _tableTags;
        #endregion

        #region method
        public FontTable? GetFontTable(List<string> fontTableTags)
        {
            _tableTags = GetTableTags();
            if (!(_tableTags?.Count > 0)) return null;

            foreach (var fontTableTag in fontTableTags)
            {
                if (!_tableTags.Contains(fontTableTag)) continue;
                switch (fontTableTag)
                {
                    case "cmap":
                        if (_fontTable.CmapTable != null) break;
                        _fontTable.CmapTable = GetCmapTable();
                        break;
                    case "glyf":
                        break;
                    case "head":
                        if (_fontTable.HeadTable != null) break;
                        _fontTable.HeadTable = GetHeadTable();
                        break;
                    case "hhea":
                        if (_fontTable.HheaTable != null) break;
                        _fontTable.HheaTable = GetHheaTable();
                        break;
                    case "hmtx":
                        break;
                    case "loca":
                        break;
                    case "maxp":
                        break;
                    case "name":
                        if (_fontTable.NameTable != null) break;
                        _fontTable.NameTable = GetNameTable();
                        break;
                    case "post":
                        break;
                    case "OS/2":
                        if (_fontTable.Os2Table != null) break;
                        _fontTable.Os2Table = GetOs2Table();
                        break;
                }
            }
            return _fontTable;
        }

        public string GetFullFontName()
        {
            var table = GetFontTable(["name"]);
            return table?.NameTable?.NameRecords?.Find(x => x.platformID == 3 && x.encodingID == 1 && x.languageID == 2052 && x.nameID == 4)?.nameString ?? "";
        }

        public string GetTypeSetting()
        {
            var table = GetFontTable(["head", "hhea", "OS/2"]);
            var sb = new StringBuilder();
            sb.AppendLine($"yMin: {table?.HeadTable?.yMin}");
            sb.AppendLine($"yMax: {table?.HeadTable?.yMax}");
            sb.AppendLine($"ascender: {table?.HheaTable?.ascender}");
            sb.AppendLine($"descender: {table?.HheaTable?.descender}");
            sb.AppendLine($"lineGap: {table?.HheaTable?.lineGap}");
            sb.AppendLine($"usWinAscent: {table?.Os2Table?.usWinAscent}");
            sb.AppendLine($"usWinDescent: {table?.Os2Table?.usWinDescent}");
            sb.AppendLine($"sTypoAscender: {table?.Os2Table?.sTypoAscender}");
            sb.AppendLine($"sTypoDescender: {table?.Os2Table?.sTypoDescender}");
            //sb.AppendLine($"fsSelection: {table?.Os2Table?.fsSelection}");
            var fsSelection = table?.Os2Table?.fsSelection ?? 0;
            var fsSelectionBinary = Convert.ToString(fsSelection, 2).PadLeft(16, '0');
            var bit7Value = (fsSelection & (1 << 7)) != 0 ? 1 : 0;
            sb.AppendLine($"fsSelection: {fsSelectionBinary}");
            sb.AppendLine($"Bit 7: Use Typo Metrics: {bit7Value}");
            sb.AppendLine($"sTypoLineGap: {table?.Os2Table?.sTypoLineGap}");
            return sb.ToString();
        }

        public List<int>? GetAllUnicode()
        {
            var table = GetFontTable(["cmap"]);
            if (!(table?.CmapTable?.CmapSubTables?.Count > 0)) return null;
            var format4 = table.CmapTable.CmapSubTables.FirstOrDefault(x => x.format == 4)?.Format4;
            if (format4 == null) return null;
            var segCount = format4.segCountX2 / 2;
            var unicodeList = new List<int>();
            for (var i = 0; i < segCount; i++)
            {
                var start = format4.startCode[i];
                var end = format4.endCode[i];
                for (int code = start; code <= end; code++)
                {
                    unicodeList.Add(code);
                }
            }
            return unicodeList;
        }

        public List<string>? GetAllUnicodeHex()
        {
            var unicodeList = GetAllUnicode();
            if (unicodeList == null || unicodeList.Count == 0) return null;
            return unicodeList.Select(x => $"{x:X4}").ToList();
        }

        public Dictionary<int, int>? GetUnicodeGidMap()
        {
            var table = GetFontTable(["cmap"]);
            if (!(table?.CmapTable?.CmapSubTables?.Count > 0)) return null;
            var format4 = table.CmapTable.CmapSubTables.FirstOrDefault(x => x.format == 4)?.Format4;
            if (format4 == null) return null;

            var result = new Dictionary<int, int>();
            int segCount = format4.segCountX2 / 2;
            for (int i = 0; i < segCount; i++)
            {
                int start = format4.startCode[i];
                int end = format4.endCode[i];
                int idDelta = format4.idDelta[i];
                int idRangeOffset = format4.idRangeOffset[i];
                for (int code = start; code <= end; code++)
                {
                    if (code == 0xFFFF) // end-of-range marker
                        continue;

                    int glyphId;
                    if (idRangeOffset == 0)
                    {
                        glyphId = (code + idDelta) & 0xFFFF;
                    }
                    else
                    {
                        // 计算glyphIdArray的索引
                        int idx = (idRangeOffset / 2) + (code - start) - (segCount - i);
                        if (format4.glyphIdArray != null && idx >= 0 && idx < format4.glyphIdArray.Length)
                        {
                            int glyphIndex = format4.glyphIdArray[idx];
                            if (glyphIndex == 0)
                            {
                                glyphId = 0;
                            }
                            else
                            {
                                glyphId = (glyphIndex + idDelta) & 0xFFFF;
                            }
                        }
                        else
                        {
                            glyphId = 0;
                        }
                    }
                    result[code] = glyphId;
                }
            }
            return result;
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

        private static void DrawReferenceLine(SKCanvas canvas, SKPaint paint, int value, int width, float baseline, int textSize, int unitsPerEm)
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

        private CmapTable? GetCmapTable()
        {
            var cmapTable = new CmapTable();
            var tableData = _typeface?.GetTableData(0x636D6170);//cmap
            if (tableData is null) return null;
            //读取'cmap' header
            cmapTable.CmapHeader = new CmapHeader();
            var offset = 0;
            cmapTable.CmapHeader.Version = BitConverter.ToUInt16(tableData.Skip(offset).Take(2).Reverse().ToArray());
            offset += 2;
            cmapTable.CmapHeader.NumTables = BitConverter.ToUInt16(tableData.Skip(offset).Take(2).Reverse().ToArray());
            //读取'cmap' subtables
            cmapTable.CmapSubTables = new List<CmapSubTable>();
            offset += 2;
            for (var i = 0; i < cmapTable.CmapHeader.NumTables; i++)
            {
                var subTable = new CmapSubTable();
                subTable.platformID = BitConverter.ToUInt16(tableData.Skip(offset).Take(2).Reverse().ToArray());
                offset += 2;
                subTable.encodingID = BitConverter.ToUInt16(tableData.Skip(offset).Take(2).Reverse().ToArray());
                offset += 2;
                subTable.offset = BitConverter.ToUInt32(tableData.Skip(offset).Take(4).Reverse().ToArray());
                offset += 4;
                cmapTable.CmapSubTables.Add(subTable);

                ParseCmapSubTable(subTable, tableData);
            }
            return cmapTable;
        }

        private static void ParseCmapSubTable(CmapSubTable subTable, byte[] tableData)
        {
            var offset = (int)subTable.offset;
            subTable.format = BitConverter.ToUInt16(tableData.Skip(offset).Take(2).Reverse().ToArray());
            switch (subTable.format)
            {
                case 0:
                    // Format 0: Byte encoding table
                    // No further processing needed for format 0
                    break;
                case 4:
                    // Format 4: Segment mapping to delta values
                    ParseCmapSubTableFormat4(subTable, tableData);
                    break;
                default:
                    // Other formats can be handled here if needed
                    break;
            }
        }

        private static void ParseCmapSubTableFormat4(CmapSubTable subTable, byte[] tableData)
        {
            var offset = (int)subTable.offset;
            var segCountX2 = BitConverter.ToUInt16(tableData.Skip(offset + 6).Take(2).Reverse().ToArray());
            var searchRange = BitConverter.ToUInt16(tableData.Skip(offset + 8).Take(2).Reverse().ToArray());
            var entrySelector = BitConverter.ToUInt16(tableData.Skip(offset + 10).Take(2).Reverse().ToArray());
            var rangeShift = BitConverter.ToUInt16(tableData.Skip(offset + 12).Take(2).Reverse().ToArray());
            var segCount = segCountX2 / 2;
            var endCode = new ushort[segCount];
            var startCode = new ushort[segCount];
            var idDelta = new short[segCount];
            var idRangeOffset = new ushort[segCount];

            subTable.Format4 = new CmapSubTableFormat4
            {
                segCountX2 = segCountX2,
                searchRange = searchRange,
                entrySelector = entrySelector,
                rangeShift = rangeShift,
                endCode = endCode,
                startCode = startCode,
                idDelta = idDelta,
                idRangeOffset = idRangeOffset
            };

            var endCodeOffset = offset + 14;
            var startCodeOffset = endCodeOffset + segCount * 2 + 2;
            var idDeltaOffset = startCodeOffset + segCount * 2;
            var idRangeOffsetOffset = idDeltaOffset + segCount * 2;
            for (int i = 0; i < segCount; i++)
            {
                endCode[i] = BitConverter.ToUInt16(tableData.Skip(endCodeOffset + i * 2).Take(2).Reverse().ToArray());
                startCode[i] = BitConverter.ToUInt16(tableData.Skip(startCodeOffset + i * 2).Take(2).Reverse().ToArray());
                idDelta[i] = BitConverter.ToInt16(tableData.Skip(idDeltaOffset + i * 2).Take(2).Reverse().ToArray());
                idRangeOffset[i] = BitConverter.ToUInt16(tableData.Skip(idRangeOffsetOffset + i * 2).Take(2).Reverse().ToArray());
            }

            // 计算 glyphIdArray 的起始偏移
            int glyphIdArrayOffset = idRangeOffsetOffset + segCount * 2;

            // 计算 glyphIdArray 的长度
            // 其长度无法直接从表头获得，需遍历 idRangeOffset，找到最大可能访问的 glyphIdArray 索引
            int maxGlyphIdIndex = 0;
            for (int i = 0; i < segCount; i++)
            {
                if (idRangeOffset[i] != 0)
                {
                    int start = startCode[i];
                    int end = endCode[i];
                    int rangeOffset = idRangeOffset[i];
                    int count = end - start + 1;
                    int firstIndex = (rangeOffset / 2) + (i - segCount);
                    int lastIndex = firstIndex + count - 1;
                    if (lastIndex > maxGlyphIdIndex)
                        maxGlyphIdIndex = lastIndex;
                }
            }
            int glyphIdArrayLength = maxGlyphIdIndex + 1;
            if (glyphIdArrayLength < 0) glyphIdArrayLength = 0;

            // 解析 glyphIdArray
            var glyphIdArray = new ushort[glyphIdArrayLength];
            for (int i = 0; i < glyphIdArrayLength; i++)
            {
                int pos = glyphIdArrayOffset + i * 2;
                if (pos + 2 <= tableData.Length)
                    glyphIdArray[i] = BitConverter.ToUInt16(tableData.Skip(pos).Take(2).Reverse().ToArray());
                else
                    glyphIdArray[i] = 0;
            }
            subTable.Format4.glyphIdArray = glyphIdArray;
        }

        private HeadTable? GetHeadTable()
        {
            var tableData = _typeface?.GetTableData(0x68656164);//head
            if (tableData is null) return null;

            var span = new ReadOnlySpan<byte>(tableData);
            int offset = 8; // 跳过 version 和 fontRevision

            var headTable = new HeadTable
            {
                checkSumAdjustment = Common.ReadUInt32(span, ref offset),
                magicNumber = Common.ReadUInt32(span, ref offset),
                flags = Common.ReadUInt16(span, ref offset),
                unitsPerEm = Common.ReadUInt16(span, ref offset),
                created = Common.ReadInt64(span, ref offset),
                modified = Common.ReadInt64(span, ref offset),
                xMin = Common.ReadInt16(span, ref offset),
                yMin = Common.ReadInt16(span, ref offset),
                xMax = Common.ReadInt16(span, ref offset),
                yMax = Common.ReadInt16(span, ref offset),
                macStyle = Common.ReadUInt16(span, ref offset),
                lowestRecPPEM = Common.ReadUInt16(span, ref offset),
                fontDirectionHint = Common.ReadInt16(span, ref offset),
                indexToLocFormat = Common.ReadInt16(span, ref offset),
                glyphDataFormat = Common.ReadInt16(span, ref offset)
            };

            return headTable;
        }

        private HheaTable? GetHheaTable()
        {
            var tableData = _typeface?.GetTableData(0x68686561);//hhea
            if (tableData is null) return null;

            var span = new ReadOnlySpan<byte>(tableData);
            int offset = 4; // 跳过 majorVersion 和 minorVersion

            var hheaTable = new HheaTable
            {
                ascender = Common.ReadInt16(span, ref offset),
                descender = Common.ReadInt16(span, ref offset),
                lineGap = Common.ReadInt16(span, ref offset)
            };

            return hheaTable;
        }

        private NameTable? GetNameTable()
        {
            var nameTable = new NameTable();
            var tableData = _typeface?.GetTableData(0x6E616D65);//name
            if (tableData is null) return null;
            //读取'name' header
            var nameHeader = new NameHeader();
            var offset = 0;
            nameHeader.format = BitConverter.ToUInt16(tableData.Skip(offset).Take(2).Reverse().ToArray());
            offset += 2;
            nameHeader.numberOfRecords = BitConverter.ToUInt16(tableData.Skip(offset).Take(2).Reverse().ToArray());
            offset += 2;
            nameHeader.storageOffset = BitConverter.ToUInt16(tableData.Skip(offset).Take(2).Reverse().ToArray());
            nameTable.NameHeader = nameHeader;
            //读取'name' records
            if (nameHeader.numberOfRecords > 0) nameTable.NameRecords = new List<NameRecord>();
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            for (var i = 0; i < nameHeader.numberOfRecords; i++)
            {
                var nameRecord = new NameRecord();
                offset += 2;
                nameRecord.platformID = BitConverter.ToUInt16(tableData.Skip(offset).Take(2).Reverse().ToArray());
                offset += 2;
                nameRecord.encodingID = BitConverter.ToUInt16(tableData.Skip(offset).Take(2).Reverse().ToArray());
                offset += 2;
                nameRecord.languageID = BitConverter.ToUInt16(tableData.Skip(offset).Take(2).Reverse().ToArray());
                offset += 2;
                nameRecord.nameID = BitConverter.ToUInt16(tableData.Skip(offset).Take(2).Reverse().ToArray());
                offset += 2;
                nameRecord.stringLength = BitConverter.ToUInt16(tableData.Skip(offset).Take(2).Reverse().ToArray());
                offset += 2;
                nameRecord.stringOffset = BitConverter.ToUInt16(tableData.Skip(offset).Take(2).Reverse().ToArray());

                var recbuf = tableData.Skip(nameHeader.storageOffset + nameRecord.stringOffset).Take(nameRecord.stringLength).ToArray();
                switch (nameRecord.platformID)
                {
                    case 0:
                        nameRecord.nameString = Encoding.BigEndianUnicode.GetString(recbuf);
                        break;
                    case 1:
                        nameRecord.nameString = nameRecord.languageID switch
                        {
                            33 => Encoding.GetEncoding("gb18030").GetString(recbuf),
                            _ => Encoding.Default.GetString(recbuf),
                        };
                        break;
                    case 3:
                        nameRecord.nameString = Encoding.BigEndianUnicode.GetString(recbuf);
                        break;
                }

                nameTable.NameRecords.Add(nameRecord);
            }
            return nameTable;
        }

        private Os2Table? GetOs2Table()
        {
            var tableData = _typeface?.GetTableData(0x4F532F32);//OS/2
            if (tableData is null) return null;

            var span = new ReadOnlySpan<byte>(tableData);
            int offset = 2; // 跳过 version

            var os2Table = new Os2Table
            {
                xAvgCharWidth = Common.ReadInt16(span, ref offset),
                usWeightClass = Common.ReadUInt16(span, ref offset),
                usWidthClass = Common.ReadUInt16(span, ref offset),
                fsType = Common.ReadInt16(span, ref offset),
                ySubscriptXSize = Common.ReadInt16(span, ref offset),
                ySubscriptYSize = Common.ReadInt16(span, ref offset),
                ySubscriptXOffset = Common.ReadInt16(span, ref offset),
                ySubscriptYOffset = Common.ReadInt16(span, ref offset),
                ySuperscriptXSize = Common.ReadInt16(span, ref offset),
                ySuperscriptYSize = Common.ReadInt16(span, ref offset),
                ySuperscriptXOffset = Common.ReadInt16(span, ref offset),
                ySuperscriptYOffset = Common.ReadInt16(span, ref offset),
                yStrikeoutSize = Common.ReadInt16(span, ref offset),
                yStrikeoutPosition = Common.ReadInt16(span, ref offset),
                sFamilyClass = Common.ReadInt16(span, ref offset),
                panose = GetOs2TablePanose(span.Slice(offset, 10).ToArray())
            };
            offset += 10;
            os2Table.ulUnicodeRange1 = Common.ReadUInt32(span, ref offset);
            os2Table.ulUnicodeRange2 = Common.ReadUInt32(span, ref offset);
            os2Table.ulUnicodeRange3 = Common.ReadUInt32(span, ref offset);
            os2Table.ulUnicodeRange4 = Common.ReadUInt32(span, ref offset);
            os2Table.achVendID = Encoding.ASCII.GetString(span.Slice(offset, 4).ToArray());
            offset += 4;
            os2Table.fsSelection = Common.ReadUInt16(span, ref offset);
            os2Table.usFirstCharIndex = Common.ReadUInt16(span, ref offset);
            os2Table.usLastCharIndex = Common.ReadUInt16(span, ref offset);
            os2Table.sTypoAscender = Common.ReadInt16(span, ref offset);
            os2Table.sTypoDescender = Common.ReadInt16(span, ref offset);
            os2Table.sTypoLineGap = Common.ReadInt16(span, ref offset);
            os2Table.usWinAscent = Common.ReadUInt16(span, ref offset);
            os2Table.usWinDescent = Common.ReadUInt16(span, ref offset);
            os2Table.ulCodePageRange1 = Common.ReadUInt32(span, ref offset);
            os2Table.ulCodePageRange2 = Common.ReadUInt32(span, ref offset);
            os2Table.sxHeight = Common.ReadInt16(span, ref offset);
            os2Table.sCapHeight = Common.ReadInt16(span, ref offset);
            os2Table.usDefaultChar = Common.ReadUInt16(span, ref offset);
            os2Table.usBreakChar = Common.ReadUInt16(span, ref offset);
            os2Table.usMaxContext = Common.ReadUInt16(span, ref offset);

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
