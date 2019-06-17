using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace OracleProcedureManager
{
    public partial class SynchronizationTask
    {
        [JsonProperty("SyncTaskId")]
        public string SyncTaskId { get; set; }

        [JsonProperty("RouterCommand")]
        public string RouterCommand { get; set; }

        [JsonProperty("CronExpression")]
        public string CronExpression { get; set; }

        [JsonProperty("SyncObjectList")]
        public List<SynchronizationObject> SyncObjectList { get; set; }
    }

    public partial class SynchronizationObject
    {
        [JsonProperty("ProceduresList")]
        public List<Procedure> ProceduresList { get; set; }

        [JsonProperty("SchemaName")]
        public string SchemaName { get; set; }

        [JsonProperty("Order")]
        public int Order { get; set; }

        [JsonProperty("WithNoIndex")]
        public bool WithNoIndex { get; set; }
    }

    public partial class Procedure
    {
        [JsonProperty("Order")]
        public int Order { get; set; }

        [JsonProperty("ProcedureName")]
        public string ProcedureName { get; set; }

        [JsonProperty("ProcedureParams")]
        public List<string> ProcedureParams { get; set; }
    }
}
