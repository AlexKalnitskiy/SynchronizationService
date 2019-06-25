using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OracleProcedureManager
{
    public interface ISyncListener
    {
        void ErrorEvent(string schemaName, Exception ex);
        void ExecutionCancelledEvent(string objectName, string procedureName);
        void ExecutionFinishedEvent(string objectName, string procedureName);
        void ExecutionStartedEvent(string objectName, string procedureName);
    }
}
