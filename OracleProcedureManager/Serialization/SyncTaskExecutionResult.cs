using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace OracleProcedureManager
{
    public partial class SyncTaskExecutionResult
    {
        [JsonProperty("SyncTaskId")]
        public string SyncTaskId { get; set; }

        [JsonProperty("isExecuted")]
        public bool isExecutedCorrectly { get; set; }

        [JsonProperty("SyncObjectList")]
        public List<SyncObjExecutionResult> ObjectResultsList { get; set; }
    }

    public partial class SyncObjExecutionResult
    {
        [JsonProperty("SchemaName")]
        public string SchemaName { get; set; }

        [JsonProperty("isExecuted")]
        public bool isExecuted { get; set; }

        [JsonProperty("ExceptionMessage")]
        public string ExceptionMessage { get; set; }
    }

    //Not serializable
    public partial class SyncTaskExecutionResult
    {
        public List<System.Exception> ExceptionList;
    }

    //Not serializable
    public partial class SyncObjExecutionResult
    {
        public System.Exception Exception;
    }
}
