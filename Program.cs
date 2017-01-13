using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;

using Newtonsoft.Json;
using System.Linq;

namespace NovaSTParser
{
    internal static class Program
    {
        const string SearchCSVFile = "Data\\SearchCorps.csv";
        const string MaterialsCSVFile = "Data\\item_general.csv";
        const string GranCSVFile = "Data\\item_gran.csv";
        const string MaterialsJSONFile = "Data\\Materials.json";
        const string DescriptionsJSONFile = "Data\\Search Team Quest Descriptions.json";
        const string MaterialsJSONDownload = "https://raw.githubusercontent.com/Arks-Layer/PSNovaTranslations/master/rmd/Materials.json";

        const char MultiplierChar = '×';

        static List<SearchTeamEntry> searchEntries = new List<SearchTeamEntry>();
        static List<ItemEntry> materialEntries = new List<ItemEntry>();
        static Dictionary<string, StringEntry> nameEntries = new Dictionary<string, StringEntry>();
        static Dictionary<string, StringEntry> descriptionEntries = new Dictionary<string, StringEntry>();

        static Stopwatch watch = new Stopwatch();

        static void WriteError(Exception e, string message)
        {
            string text = $"{message} - {e}\n{e.InnerException}";

            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(text);
        }

        static uint ParseCSVValue(string text)
        {
            uint result = 0;

            if (text.Length == 0)
                return 0;
            else
                uint.TryParse(text, out result);

            return result;
        }

        static void ParseSearchTeamData()
        {
            Console.WriteLine("Parsing search team CSV...");

            watch.Start();

            string data = File.ReadAllText(SearchCSVFile);
            string[] rows = data.Split(new string[] { "\r\n" }, StringSplitOptions.None);

            foreach (string row in rows)
            {
                string[] split = row.Split(',');

                SearchTeamEntry entry = new SearchTeamEntry
                {
                    ID = ParseCSVValue(split[SearchTeamEntry.ColumnID]),
                    Name = ParseCSVValue(split[SearchTeamEntry.ColumnName]),
                    Type = (SearchType)Enum.Parse(typeof(SearchType), Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(split[SearchTeamEntry.ColumnType])),
                    Description = ParseCSVValue(split[SearchTeamEntry.ColumnDescription]),
                    Area = ParseCSVValue(split[SearchTeamEntry.ColumnArea]),
                    Crew = ParseCSVValue(split[SearchTeamEntry.ColumnCrew]),
                    Risk = (SearchRisk)Enum.Parse(typeof(SearchRisk), Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(split[SearchTeamEntry.ColumnRisk])),
                    Success = ParseCSVValue(split[SearchTeamEntry.ColumnSuccess]),
                    Days = ParseCSVValue(split[SearchTeamEntry.ColumnDays]),
                    ThawNumber = ParseCSVValue(split[SearchTeamEntry.ColumnThawNumber]),
                    Repopulate = ParseCSVValue(split[SearchTeamEntry.ColumnRepopulate]),
                };

                entry.Rewards[0] = new SearchTeamReward
                {
                    Category = ParseCSVValue(split[SearchTeamEntry.ColumnReward1]),
                    Type = ParseCSVValue(split[SearchTeamEntry.ColumnReward1 + SearchTeamReward.ColumnTypeOffset]),
                    Index = ParseCSVValue(split[SearchTeamEntry.ColumnReward1 + SearchTeamReward.ColumnIndexOffset]),
                    Quantity = ParseCSVValue(split[SearchTeamEntry.ColumnReward1 + SearchTeamReward.ColumnQuantityOffset])
                };
                entry.Rewards[1] = new SearchTeamReward
                {
                    Category = ParseCSVValue(split[SearchTeamEntry.ColumnReward2]),
                    Type = ParseCSVValue(split[SearchTeamEntry.ColumnReward2 + SearchTeamReward.ColumnTypeOffset]),
                    Index = ParseCSVValue(split[SearchTeamEntry.ColumnReward2 + SearchTeamReward.ColumnIndexOffset]),
                    Quantity = ParseCSVValue(split[SearchTeamEntry.ColumnReward2 + SearchTeamReward.ColumnQuantityOffset])
                };
                entry.Rewards[2] = new SearchTeamReward
                {
                    Category = ParseCSVValue(split[SearchTeamEntry.ColumnReward3]),
                    Type = ParseCSVValue(split[SearchTeamEntry.ColumnReward3 + SearchTeamReward.ColumnTypeOffset]),
                    Index = ParseCSVValue(split[SearchTeamEntry.ColumnReward3 + SearchTeamReward.ColumnIndexOffset]),
                    Quantity = ParseCSVValue(split[SearchTeamEntry.ColumnReward3 + SearchTeamReward.ColumnQuantityOffset])
                };

                searchEntries.Add(entry);
            }

            watch.Stop();

            Console.WriteLine($"Done! Parsing {searchEntries.Count} entries took {watch.ElapsedMilliseconds} ms");

            watch.Reset();
        }

        static void ParseItemData()
        {
            Console.WriteLine("Parsing item CSV...");

            watch.Start();

            string data = File.ReadAllText(MaterialsCSVFile) + File.ReadAllText(GranCSVFile);
            string[] rows = data.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            const int MaxSplit = 15;

            foreach (string row in rows)
            {
                List<string> split = row.Split(',').ToList();

                // Stupid as fuck hack because whoever made the DLC entries in this CSV were a bunch of assholes and didn't bother to include the end columns
                while (split.Count < MaxSplit)
                    split.Add("0");

                ItemEntry entry = new ItemEntry
                {
                    Key = split[ItemEntry.ColumnKey],
                    Category = ParseCSVValue(split[ItemEntry.ColumnCategory]),
                    Type = ParseCSVValue(split[ItemEntry.ColumnType]),
                    Index = ParseCSVValue(split[ItemEntry.ColumnIndex]),
                    Icon = split[ItemEntry.ColumnIcon],
                    Name = ParseCSVValue(split[ItemEntry.ColumnName]),
                    Description = ParseCSVValue(split[ItemEntry.ColumnDescription]),
                    Quantity = ParseCSVValue(split[ItemEntry.ColumnQuantity])
                };

                materialEntries.Add(entry);
            }

            watch.Stop();

            Console.WriteLine($"Done! Parsing {materialEntries.Count} entries took {watch.ElapsedMilliseconds} ms");

            watch.Reset();
        }

        static void ParseNameData()
        {
            Console.WriteLine("Parsing materials JSON...");

            using (WebClient client = new WebClient())
                client.DownloadFile(MaterialsJSONDownload, MaterialsJSONFile);

            watch.Start();

            nameEntries = JsonConvert.DeserializeObject<Dictionary<string, StringEntry>>(File.ReadAllText(MaterialsJSONFile));

            watch.Stop();

            Console.WriteLine($"Done! Parsing {nameEntries.Count} entries took {watch.ElapsedMilliseconds} ms");

            watch.Reset();
        }

        static void ParseDescriptionData()
        {
            Console.WriteLine("Parsing description JSON...");

            watch.Start();

            descriptionEntries = JsonConvert.DeserializeObject<Dictionary<string, StringEntry>>(File.ReadAllText(DescriptionsJSONFile));

            watch.Stop();

            Console.WriteLine($"Done! Parsing {descriptionEntries.Count} entries took {watch.ElapsedMilliseconds} ms");

            watch.Reset();
        }

        static void PopulateDescriptions()
        {
            Console.WriteLine("Populating description data...");

            watch.Start();

            foreach (KeyValuePair<string, StringEntry> descriptionKvp in descriptionEntries)
            {
                string description = string.Empty;
                List<Item> items = new List<Item>();

                foreach (SearchTeamEntry searchEntry in searchEntries)
                    if (searchEntry.Description == uint.Parse(descriptionKvp.Key))
                        foreach (SearchTeamReward reward in searchEntry.Rewards)
                            foreach (ItemEntry itemEntry in materialEntries)
                                if (reward.Category == itemEntry.Category && reward.Type == itemEntry.Type && reward.Index == itemEntry.Index)
                                    foreach (KeyValuePair<string, StringEntry> nameKvp in nameEntries)
                                        if (itemEntry.Name.ToString() == nameKvp.Key)
                                        {
                                            Item item = new Item();

                                            if (nameKvp.Value.Text == string.Empty)
                                                item.Name = nameKvp.Value.OriginalText;
                                            else
                                                item.Name = nameKvp.Value.Text;

                                            item.Quantity = reward.Quantity;

                                            items.Add(item);
                                        }

                foreach (Item item in items)
                    description += $"{item.Name} {MultiplierChar} {item.Quantity}\n";

                if (description.Length > 0)
                {
                    description = description.Remove(description.Length - 1);

                    descriptionEntries[descriptionKvp.Key].Text = description;
                    descriptionEntries[descriptionKvp.Key].Enabled = true;
                }
                else
                {
                    descriptionEntries[descriptionKvp.Key].Text = "\t";
                    descriptionEntries[descriptionKvp.Key].Enabled = true;
                }
            }

            File.WriteAllText(DescriptionsJSONFile + ".out", JsonConvert.SerializeObject(descriptionEntries, Formatting.Indented));

            watch.Stop();

            Console.WriteLine($"Done! Took {watch.ElapsedMilliseconds} ms");

            watch.Reset();
        }

        static void Main()
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Green;

                ParseSearchTeamData();
                ParseItemData();
                ParseNameData();
                ParseDescriptionData();
                PopulateDescriptions();

                Console.ReadLine();
            }
            catch (Exception e)
            {
                WriteError(e, "Error");

                Console.ReadLine();
            }
        }
    }
}
