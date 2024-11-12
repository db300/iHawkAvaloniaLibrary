namespace iHawkSkiaSharpCommonLibrary.Entities.FontTableEntities
{
    public class HheaTable
    {
        #region property
        /// <summary>
        /// distance from baseline of highest ascender
        /// </summary>
        public Int16 ascender { get; set; }
        /// <summary>
        /// distance from baseline of lowest descender
        /// </summary>
        public Int16 descender { get; set; }
        /// <summary>
        /// typographic line cap
        /// </summary>
        public Int16 lineGap { get; set; }
        #endregion

        #region method
        public new string ToString() => $"[HHEA_TABLE]\r\nAscender={ascender}\r\nDescender={descender}\r\nLineGap={lineGap}\r\n";
        #endregion
    }
}
