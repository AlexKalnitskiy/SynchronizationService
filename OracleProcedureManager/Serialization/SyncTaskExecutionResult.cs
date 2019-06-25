using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace OracleProcedureManager
{
    public partial class SyncTaskExecutionContext
    {
        [JsonProperty("SyncTaskId")]
        public string SyncTaskId { get; set; }

        [JsonProperty("TaskCurrentState")]
        public int TaskCurrentState { get; set; }

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
    public partial class SyncTaskExecutionContext
    {
        public List<System.Exception> ExceptionList;
    }

    //Not serializable
    public partial class SyncObjExecutionResult
    {
        public System.Exception Exception;
    }
}
