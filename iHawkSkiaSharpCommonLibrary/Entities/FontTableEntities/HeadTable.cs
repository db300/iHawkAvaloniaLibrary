namespace iHawkSkiaSharpCommonLibrary.Entities.FontTableEntities
{
    public class HeadTable
    {
        #region property
        public UInt32 checkSumAdjustment { get; set; }
        public UInt32 magicNumber { get; set; }
        public UInt16 flags { get; set; }
        public UInt16 unitsPerEm { get; set; }
        public Int16 xMin { get; set; }
        public Int16 yMin { get; set; }
        public Int16 xMax { get; set; }
        public Int16 yMax { get; set; }
        public UInt16 macStyle { get; set; }
        public UInt16 lowestRecPPEM { get; set; }
        public Int16 fontDirectionHint { get; set; }
        public Int16 indexToLocFormat { get; set; }
        public Int16 glyphDataFormt { get; set; }
        #endregion

        #region method

        public new string ToString()
        {
            return "[HEAD_TABLE]\r\n" +
                   string.Format("Flag={0}\r\n", flags) +
                   string.Format("UnitsPerEM={0}\r\n", unitsPerEm) +
                   string.Format("Macstyle={0}\r\n", macStyle) +
                   string.Format("lowestRecPPEM={0}\r\n", lowestRecPPEM) +
                   string.Format("FontDirectionHint={0}\r\n", fontDirectionHint);
        }

        #endregion
    }
}
