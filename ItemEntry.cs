namespace NovaSTParser
{
    public class ItemEntry
    {
        public const int ColumnKey = 0;
        public const int ColumnCategory = 1;
        public const int ColumnType = 2;
        public const int ColumnIndex = 3;
        public const int ColumnIcon = 5;
        public const int ColumnName = 8;
        public const int ColumnDescription = 9;
        public const int ColumnQuantity = 13;

        public string Key { get; set; }
        public uint Category { get; set; }
        public uint Type { get; set; }
        public uint Index { get; set; }
        public uint Unknown1 { get; set; }
        public string Icon { get; set; }
        public uint Unknown2 { get; set; }
        public uint Unknown3 { get; set; }
        public uint Name { get; set; }
        public uint Description { get; set; }
        public uint Unknown4 { get; set; }
        public uint Unknown5 { get; set; }
        public uint Unknown6 { get; set; }
        public uint Quantity { get; set; }
        public uint Unknown7 { get; set; }
    }
}
