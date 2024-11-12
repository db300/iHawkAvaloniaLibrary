namespace iHawkSkiaSharpCommonLibrary.Entities.FontTableEntities
{
    public class PostTable
    {
        #region property

        public PostHeader PostHeader { get; set; }
        public List<PostName> PostNames { get; set; }

        #endregion

        #region method

        public new string ToString()
        {
            return "[POST_TABLE]\r\n" +
                   string.Format("ItalicAngle={0}.{1}\r\n", PostHeader.italicAngle.high, PostHeader.italicAngle.low) +
                   string.Format("UnderlinePosition={0}\r\n", PostHeader.underlinePosition) +
                   string.Format("UnderlineThickness={0}\r\n", PostHeader.underlineThickness) +
                   string.Format("IsFixedPitch={0}\r\n", PostHeader.isFixedPitch);
        }

        #endregion
    }

    public class PostHeader
    {
        public Fixed formatType { get; set; }
        public Fixed italicAngle { get; set; }
        public Int16 underlinePosition { get; set; }
        public Int16 underlineThickness { get; set; }
        public UInt32 isFixedPitch { get; set; }
    }

    public class PostName
    {
    }
}
