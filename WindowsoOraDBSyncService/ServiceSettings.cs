using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Globalization;

namespace OraDBSyncService
{
    partial class ServiceSettings
    {
        #region Constructors
        private ServiceSettings() { }
        #endregion Constructors

        private static ServiceSettings _singleton;

        public static ServiceSettings Get()
        {
            if (_singleton != null)
                return _singleton;
            else
                try
                {
                    _singleton = FromJson(System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "\\config.json"));
                    return _singleton;
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to initialize config", ex);
                }
        }
    }

    partial class ServiceSettings
    {
        [JsonProperty("Port")]
        public string Port { get; set; }

        [JsonProperty("ConnectionString")]
        public string ConnectionString { get; set; }

        [JsonProperty("OracleLogFunction")]
        public string OracleLogFunction { get; set; }

        [JsonProperty("OracleMainLogFunction")]
        public string OracleMainLogFunction { get; set; }
    }

    public static partial class Serialize
    {
        public static string ToJson(this ServiceSettings self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    public partial class ServiceSettings
    {
        public static ServiceSettings FromJson(string json) => JsonConvert.DeserializeObject<ServiceSettings>(json, Converter.Settings);
    }

    internal static partial class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters = {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

}
