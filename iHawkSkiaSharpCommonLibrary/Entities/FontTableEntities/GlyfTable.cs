namespace iHawkSkiaSharpCommonLibrary.Entities.FontTableEntities
{
    public class GlyfTable : List<GlyfTableItem>
    {
    }

    public class GlyfTableItem
    {
        public GlyfTableItemHead GlyfTableItemHead { get; set; }
        public GlyfTableItemBody GlyfTableItemBody { get; set; }
    }

    public class GlyfTableItemHead
    {
        public Int16 numberOfContours { get; set; }
        public Int16 xMin { get; set; }
        public Int16 yMin { get; set; }
        public Int16 xMax { get; set; }
        public Int16 yMax { get; set; }

        /// <summary>
        /// 宽度值(用于字库包装接口)
        /// </summary>
        public UInt16 AdvanceWidth { get; set; }

        /// <summary>
        /// Unicodes(用于字库包装接口)
        /// </summary>
        public UInt32[] Unicodes { get; set; }
    }

    public class GlyfTableItemBody
    {
        public List<UInt16> endPtsOfContours { get; set; }
        public UInt16 numberOfPoints { get; set; }
        public UInt16 instructionLength { get; set; }
        public byte[] instructions { get; set; }
        public byte[] flags { get; set; }
        public Int16[] xCoordinates { get; set; }
        public Int16[] yCoordinates { get; set; }
    }
}
