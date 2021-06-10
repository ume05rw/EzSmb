using Newtonsoft.Json;

namespace EzSmbTest.Models
{
    public class Item
    {
        [JsonProperty("type")]
        public ItemType Type { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("existsPath")]
        public bool ExistsPath { get; set; }

        [JsonProperty("itemCount")]
        public int ItemCount { get; set; }

        [JsonProperty("filter")]
        public string Filter { get; set; }

        [JsonProperty("filteredItemCount")]
        public int FilteredItemCount { get; set; }

        [JsonProperty("relatedServer")]
        public string RelatedServer { get; set; }

        [JsonProperty("relatedShare")]
        public string RelatedShare { get; set; }

        [JsonProperty("relatedFolder")]
        public string RelatedFolder { get; set; }

        [JsonProperty("relatedFile")]
        public string RelatedFile { get; set; }
    }
}
