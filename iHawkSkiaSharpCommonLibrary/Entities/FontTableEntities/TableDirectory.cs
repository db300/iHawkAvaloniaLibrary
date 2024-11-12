namespace iHawkSkiaSharpCommonLibrary.Entities.FontTableEntities
{
    public class TableDirectory : Dictionary<string, TableDirectoryItem>
    {
    }

    public class TableDirectoryItem
    {
        public string tag { get; set; }
        public UInt32 checkSum { get; set; }
        public UInt32 offset { get; set; }
        public UInt32 length { get; set; }
    }
}
