namespace iHawkSkiaSharpCommonLibrary.Entities.FontTableEntities
{
    public class Os2Table
    {
        #region property

        public Int16 xAvgCharWidth { get; set; }
        public UInt16 usWeightClass { get; set; }
        public UInt16 usWidthClass { get; set; }
        public Int16 fsType { get; set; }
        public Int16 ySubscriptXSize { get; set; }
        public Int16 ySubscriptYSize { get; set; }
        public Int16 ySubscriptXOffset { get; set; }
        public Int16 ySubscriptYOffset { get; set; }
        public Int16 ySuperscriptXSize { get; set; }
        public Int16 ySuperscriptYSize { get; set; }
        public Int16 ySuperscriptXOffset { get; set; }
        public Int16 ySuperscriptYOffset { get; set; }
        public Int16 yStrikeoutSize { get; set; }
        public Int16 yStrikeoutPosition { get; set; }
        public Int16 sFamilyClass { get; set; }
        public Panose panose { get; set; }
        public UInt32 ulUnicodeRange1 { get; set; }
        public UInt32 ulUnicodeRange2 { get; set; }
        public UInt32 ulUnicodeRange3 { get; set; }
        public UInt32 ulUnicodeRange4 { get; set; }
        public string achVendID { get; set; }
        public UInt16 fsSelection { get; set; }
        public UInt16 usFirstCharIndex { get; set; }
        public UInt16 usLastCharIndex { get; set; }
        public Int16 sTypoAscender { get; set; }
        public Int16 sTypoDescender { get; set; }
        public Int16 sTypoLineGap { get; set; }
        public UInt16 usWinAscent { get; set; }
        public UInt16 usWinDescent { get; set; }
        public UInt32 ulCodePageRange1 { get; set; }
        public UInt32 ulCodePageRange2 { get; set; }
        public Int16 sxHeight { get; set; }
        public Int16 sCapHeight { get; set; }
        public UInt16 usDefaultChar { get; set; }
        public UInt16 usBreakChar { get; set; }

        public UInt16 usMaxContext { get; set; }
        //public UInt16 usLowerOpticalPointSize { get; set; }
        //public UInt16 usUpperOpticalPointSize { get; set; }

        #endregion

        #region method

        public new string ToString()
        {
            return "[OS/2_TABLE]\r\n" +
                   string.Format("WeightClass={0}\r\n", usWeightClass) +
                   string.Format("WidthClass={0}\r\n", usWidthClass) +
                   string.Format("FsType={0}\r\n", fsType) +
                   string.Format("Panose_FamilyType={0}\r\n", panose.bFamilyType) +
                   string.Format("Panose_SerifStyle={0}\r\n", panose.bSerifStyle) +
                   string.Format("Panose_Weight={0}\r\n", panose.bWeight) +
                   string.Format("Panose_Proportion={0}\r\n", panose.bProportion) +
                   string.Format("Panose_Contrast={0}\r\n", panose.bContrast) +
                   string.Format("Panose_StrokeVariation={0}\r\n", panose.bStrokeVariation) +
                   string.Format("Panose_ArmStyle={0}\r\n", panose.bArmStyle) +
                   string.Format("Panose_Letterform={0}\r\n", panose.bLetterform) +
                   string.Format("Panose_Midline={0}\r\n", panose.bMidline) +
                   string.Format("Panose_XHeight={0}\r\n", panose.bXHeight) +
                   string.Format("UnicodeRange1={0}\r\n", "0x" + ulUnicodeRange1.ToString("X").ToLower().PadLeft(8, '0')) +
                   string.Format("UnicodeRange2={0}\r\n", "0x" + ulUnicodeRange2.ToString("X").ToLower().PadLeft(8, '0')) +
                   string.Format("UnicodeRange3={0}\r\n", "0x" + ulUnicodeRange3.ToString("X")) +
                   string.Format("UnicodeRange4={0}\r\n", "0x" + ulUnicodeRange4.ToString("X")) +
                   string.Format("achVendID={0}\r\n", achVendID) +
                   string.Format("fsSelection={0}\r\n", "0x" + fsSelection.ToString("X")) +
                   string.Format("TypoAscender={0}\r\n", sTypoAscender) +
                   string.Format("TypoDscender={0}\r\n", sTypoDescender) +
                   string.Format("TypoLineGap={0}\r\n", sTypoLineGap) +
                   string.Format("WinAscent={0}\r\n", usWinAscent) +
                   string.Format("WinDescent={0}\r\n", usWinDescent) +
                   string.Format("CodePageRange1={0}\r\n",
                       "0x" + ulCodePageRange1.ToString("X").ToLower().PadLeft(8, '0')) +
                   string.Format("CodePageRange2={0}\r\n",
                       "0x" + ulCodePageRange2.ToString("X").ToLower().PadLeft(8, '0')) +
                   string.Format("SubscriptXSize={0}\r\n", ySubscriptXSize) +
                   string.Format("SubscriptYSize={0}\r\n", ySubscriptYSize) +
                   string.Format("SubscriptXOffset={0}\r\n", ySubscriptXOffset) +
                   string.Format("SubscriptYOffset={0}\r\n", ySubscriptYOffset) +
                   string.Format("SuperscriptXSize={0}\r\n", ySuperscriptXSize) +
                   string.Format("SuperscriptYSize={0}\r\n", ySuperscriptYSize) +
                   string.Format("SuperscriptXOffset={0}\r\n", ySuperscriptXOffset) +
                   string.Format("SuperscriptYOffset={0}\r\n", ySuperscriptYOffset) +
                   string.Format("StrikeoutSize={0}\r\n", yStrikeoutSize) +
                   string.Format("StrikeoutPosition={0}\r\n", yStrikeoutPosition) +
                   string.Format("xHeight={0}\r\n", sxHeight) +
                   string.Format("CapHeight={0}\r\n", sCapHeight);
        }

        #endregion
    }

    public class Panose
    {
        public byte bFamilyType { get; set; }
        public byte bSerifStyle { get; set; }
        public byte bWeight { get; set; }
        public byte bProportion { get; set; }
        public byte bContrast { get; set; }
        public byte bStrokeVariation { get; set; }
        public byte bArmStyle { get; set; }
        public byte bLetterform { get; set; }
        public byte bMidline { get; set; }
        public byte bXHeight { get; set; }
    }
}
