using Newtonsoft.Json;

namespace EzSmbTest.Models
{
    public class Setting
    {
        [JsonProperty("mame")]
        public string Name { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("hostName")]
        public string HostName { get; set; }

        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("domainName")]
        public string DomainName { get; set; }

        [JsonProperty("supportedSmb1")]
        public bool SupportedSmb1 { get; set; }

        [JsonProperty("supportedSmb2")]
        public bool SupportedSmb2 { get; set; }

        [JsonProperty("testPath")]
        public TestPath TestPath { get; set; }
    }

    public class TestPath
    {
        [JsonProperty("share")]
        public Item Share { get; set; }

        [JsonProperty("folder")]
        public Item Folder { get; set; }

        [JsonProperty("file")]
        public Item File { get; set; }


        [JsonProperty("failShare")]
        public Item FailShare { get; set; }

        [JsonProperty("failFolder")]
        public Item FailFolder { get; set; }

        [JsonProperty("failFile")]
        public Item FailFile { get; set; }


        [JsonProperty("relatedServer")]
        public Item RelatedServer { get; set; }

        [JsonProperty("relatedShare")]
        public Item RelatedShare { get; set; }

        [JsonProperty("relatedFolder")]
        public Item RelatedFolder { get; set; }

        [JsonProperty("relatedFile")]
        public Item RelatedFile { get; set; }
    }



    //public class Related
    //{
    //    [JsonProperty("path")]
    //    public string Path { get; set; }

    //    [JsonProperty("name")]
    //    public string Name { get; set; }
    //}
}
