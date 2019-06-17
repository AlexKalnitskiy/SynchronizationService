using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Globalization;
using System.Linq;

namespace OracleProcedureManager
{ 
    public static partial class Serialize
    {
        public static string ToJson(this SynchronizationTask self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    public partial class SynchronizationTask
    {
        public static SynchronizationTask FromJson(string json) => JsonConvert.DeserializeObject<SynchronizationTask>(json, Converter.Settings);
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
    
    public static class SyncExtensions
    {
        /// <summary>
        /// Loop through all SyncObjectsResults, collecting their Exceptions and marking task result as false, if any of Exceptions found
        /// </summary>
        /// <param name="it"></param>
        /// <returns></returns>
        public static SyncTaskExecutionResult CollectResults(this SyncTaskExecutionResult it)
        {
            if (it.ObjectResultsList.Any(x =>
            {
                if (x.isExecuted)
                    return false;
                else
                {
                    it.ExceptionList.Add(x.Exception);
                    return true;
                }
            }))
            {
                it.isExecutedCorrectly = false;
            }
            return it;
        }
    }
}
