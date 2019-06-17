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
        void ExecutionCancelledEvent(string schemaName);
        void ExecutionFinishedEvent(string schemaName);
        void ExecutionStartedEvent(string schemaName);
    }
}
