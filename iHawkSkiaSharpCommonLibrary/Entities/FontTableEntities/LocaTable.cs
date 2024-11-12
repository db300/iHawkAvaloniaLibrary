namespace iHawkSkiaSharpCommonLibrary.Entities.FontTableEntities
{
    public class LocaTable : List<LocaTableItem>
    {
    }

    public class LocaTableItem
    {
        public int offset { get; set; }
        public int length { get; set; }
    }
}
