namespace iHawkSkiaSharpCommonLibrary.Entities.FontTableEntities
{
    public class NameTable
    {
        #region property

        public NameHeader NameHeader { get; set; }
        public List<NameRecord> NameRecords { get; set; }

        #endregion

        #region method

        public new string ToString()
        {
            var s = "[NAME]\r\n" +
                    string.Format("Segment={0}\r\n", this.NameHeader.numberOfRecords);
            for (int i = 0; i < this.NameHeader.numberOfRecords; i++)
                s += string.Format("NAME_RECORD_{0}={1}\r\n", i, this.NameRecords[i].ToString());
            return s;
        }

        #endregion
    }

    public class NameHeader
    {
        public UInt16 format { get; set; }
        public UInt16 numberOfRecords { get; set; }
        public UInt16 storageOffset { get; set; }
    }

    public class NameRecord
    {
        #region property

        public UInt16 platformID { get; set; }
        public UInt16 encodingID { get; set; }
        public UInt16 languageID { get; set; }
        public UInt16 nameID { get; set; }
        public UInt16 stringLength { get; set; }
        public UInt16 stringOffset { get; set; }
        public string nameString { get; set; }

        #endregion

        #region method

        public new string ToString()
        {
            return string.Format("{0},{1},{2},{3},{4}",
                this.platformID, this.encodingID, this.languageID, this.nameID, this.nameString);
        }

        #endregion
    }
}
