using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;

namespace OracleProcedureManager
{
    public class OracleProcedurePack
    {
        public string SchemaName;

        public bool SuccessfullyCompleted;

        public bool WithNoIndex = false;

        public SortedDictionary<int, OracleCommand> Procedures = new SortedDictionary<int, OracleCommand>();

        public OracleProcedurePack(string schemaName)
        {
            SchemaName = schemaName;
        }
    }
}