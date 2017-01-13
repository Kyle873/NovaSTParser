using System;
using System.Collections;
using System.Dynamic;

namespace NovaSTParser
{
    public enum SearchType
    {
        Food,
        Survey,
        Collecting,
        Enemy
    }

    public enum SearchRisk
    {
        Safe,
        Normal,
        Danger,
        Fatal
    }

    public class SearchTeamReward
    {
        public const int ColumnTypeOffset = 1;
        public const int ColumnIndexOffset = 3;
        public const int ColumnQuantityOffset = 9;

        public uint Category { get; set; }
        public uint Type { get; set; }
        public uint Index { get; set; }
        public uint Quantity { get; set; }
    }

    public class SearchTeamEntry
    {
        public const int ColumnID = 0;
        public const int ColumnName = 1;
        public const int ColumnType = 2;
        public const int ColumnDescription = 3;
        public const int ColumnArea = 4;
        public const int ColumnCrew = 5;
        public const int ColumnRisk = 6;
        public const int ColumnSuccess = 7;
        public const int ColumnDays = 8;
        public const int ColumnThawNumber = 14;
        public const int ColumnRepopulate = 27;
        public const int ColumnReward1 = 28;
        public const int ColumnReward2 = 38;
        public const int ColumnReward3 = 48;

        public uint ID { get; set; }
        public uint Name { get; set; }
        public SearchType Type { get; set; }
        public uint Description { get; set; }
        public uint Area { get; set; }
        public uint Crew { get; set; }
        public SearchRisk Risk { get; set; }
        public uint Success { get; set; }
        public uint Days { get; set; }
        public uint ThawNumber { get; set; }
        public uint Repopulate { get; set; }

        public SearchTeamReward[] Rewards { get; set; } = new SearchTeamReward[3];
    }
}
