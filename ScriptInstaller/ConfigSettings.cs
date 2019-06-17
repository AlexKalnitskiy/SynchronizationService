using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Globalization;
namespace ScriptInstaller
{
    public partial class ConfigSettings
    {
        [JsonProperty("ConnectionString")]
        public string[] ConnectionString { get; set; }

        [JsonProperty("InstallFolder")]
        public string InstallFolder { get; set; }

        [JsonProperty("Ignore")]
        public string[] Ignore { get; set; }
    }

    public partial class ConfigSettings
    {
        public static ConfigSettings FromJson(string json) => JsonConvert.DeserializeObject<ConfigSettings>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this ConfigSettings self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Default,
            DateParseHandling = DateParseHandling.None,
            Converters = {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}