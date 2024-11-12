namespace iHawkSkiaSharpCommonLibrary.Entities.FontTableEntities
{
    public class CmapTable
    {
        public CmapHeader CmapHeader { get; set; }
        public List<CmapSubTable> CmapSubTables { get; set; }
    }

    public class CmapHeader
    {
        public UInt16 NumTables { get; set; }
    }

    public class CmapSubTable
    {
        public UInt16 platformID { get; set; }
        public UInt16 encodingID { get; set; }
        public UInt32 offset { get; set; }
        public UInt16 format { get; set; }
        public UInt16 length { get; set; }
        public UInt16 version { get; set; }
        public CmapSubTableFormat4 Format4 { get; set; }
    }

    public class CmapSubTableFormat4
    {
        public UInt16 segCountX2 { get; set; }
        public UInt16 searchRange { get; set; }
        public UInt16 entrySelector { get; set; }
        public UInt16 rangeShift { get; set; }
        public UInt16[] endCount { get; set; }
        public UInt16 reservedPad { get; set; }
        public UInt16[] startCount { get; set; }
        public Int16[] idDelta { get; set; }
        public UInt16[] idRangeOffset { get; set; }
        public UInt16[] glyphIdArray { get; set; }
    }
}
