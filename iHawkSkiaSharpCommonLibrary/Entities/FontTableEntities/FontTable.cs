namespace iHawkSkiaSharpCommonLibrary.Entities.FontTableEntities
{
    public class FontTable
    {
        #region property
        public string[] Tags
        {
            get
            {
                return new[]
                {
                    /*Required Tables*/
                    "cmap", "glyf", "head", "hhea", "hmtx", "loca", "maxp", "name", "post", "OS/2",
                    /*Optional Tables*/
                    "cvt ", "EBDT", "EBLC", "EBSC", "fpgm", "gasp", "hdmx", "kern", "LTSH", "prep", "PCLT", "VDMX", "vhea", "vmtx"
                };
            }
        }

        public CmapTable CmapTable { get; set; }
        public GlyfTable GlyfTable { get; set; }
        public HeadTable? HeadTable { get; set; }
        public HheaTable? HheaTable { get; set; }
        public LocaTable LocaTable { get; set; }
        public MaxpTable MaxpTable { get; set; }
        public NameTable NameTable { get; set; }
        public Os2Table? Os2Table { get; set; }
        public PostTable PostTable { get; set; }
        #endregion

        #region method
        public new string ToString()
        {
            return (NameTable == null ? "" : NameTable.ToString()) +
                   (PostTable == null ? "" : PostTable.ToString()) +
                   (HheaTable == null ? "" : HheaTable.ToString()) +
                   (HeadTable == null ? "" : HeadTable.ToString()) +
                   (Os2Table == null ? "" : Os2Table.ToString()) +
                   "[BASELINE]\r\n" +
                   "baseline=0.20\r\n" +
                   "[CFF]\r\n" +
                   "IsCIDFont=1\r\n" +
                   "ROS_Registroy=Adobe\r\n" +
                   "ROS_Ordering=GB1\r\n" +
                   "ROS_Supplement=1\r\n";
        }

        public List<int> GetUnicodeList()
        {
            if (this.CmapTable == null || this.CmapTable.CmapSubTables == null || this.CmapTable.CmapSubTables.Count == 0) return null;

            var unicodes = new List<int>();
            foreach (CmapSubTable cmapSubTable in CmapTable.CmapSubTables)
            {
                if (cmapSubTable.format == 4)
                {
                    for (int i = 0; i < cmapSubTable.Format4.segCountX2 / 2; i++)
                    {
                        var start = cmapSubTable.Format4.startCount[i];
                        var end = cmapSubTable.Format4.endCount[i];
                        for (int j = start; j < end + 1; j++) if (!unicodes.Contains(j)) unicodes.Add(j);
                    }
                    break;
                }
            }
            return unicodes;
        }

        public GlyfTable GetGlyphTable(List<UInt16> unicodes)
        {
            if (this.CmapTable == null || this.CmapTable.CmapSubTables == null || this.CmapTable.CmapSubTables.Count == 0) return null;

            GlyfTable glyfTable = null;

            foreach (CmapSubTable cmapSubTable in this.CmapTable.CmapSubTables)
            {
                if (cmapSubTable.format == 4 && cmapSubTable.Format4 != null)
                {
                    var indexs = GetGlyphIdsByUnicodes(cmapSubTable.Format4, unicodes);
                    glyfTable = new GlyfTable();
                    glyfTable.AddRange(indexs.Select(index => this.GlyfTable[index]));
                    break;
                }
            }

            return glyfTable;
        }
        #endregion

        #region internal method
        /// <summary>
        /// var id = GetGlyphIdByUnicode(cmapSubTable.Format4, 20624);
        /// </summary>
        /// <param name="format4"></param>
        /// <param name="unicode"></param>
        /// <returns></returns>
        private UInt16 GetGlyphIdByUnicode(CmapSubTableFormat4 format4, UInt16 unicode)
        {
            UInt16 id = 0;
            int endIdx = -1, startIdx = -1;
            for (int i = 0; i < format4.segCountX2 / 2; i++)
            {
                var end = format4.endCount[i];
                if (end >= unicode)
                {
                    endIdx = i;
                    break;
                }
            }
            if (endIdx < 0) return 0;
            var start = format4.startCount[endIdx];
            if (start > unicode) return 0;
            if (format4.idRangeOffset[endIdx] == 0)
                id = (ushort)((format4.idDelta[endIdx] + unicode) % 65535);
            else
            {
                id = (ushort)((format4.idRangeOffset[endIdx] - format4.segCountX2) / 2 + endIdx + (unicode - start));
                id = format4.glyphIdArray[id];
            }
            return id;
        }

        private List<UInt16> GetGlyphIdsByUnicodes(CmapSubTableFormat4 format4, List<UInt16> unicodes, bool compress = false)
        {
            if (compress)
            {
                unicodes.Sort();
                for (int i = unicodes.Count - 1; i > 0; i--) if (unicodes[i] == unicodes[i - 1]) unicodes.RemoveAt(i);
            }

            return unicodes.Select(unicode => GetGlyphIdByUnicode(format4, unicode)).ToList();
        }
        #endregion
    }
}
