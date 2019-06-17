using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;

namespace OracleProcedureManager
{
    public static class OracleProcedureBuilder
    {
        public static OracleConnection OpenNewConnection(this string connectionString)
        {
            OracleConnection result = new OracleConnection(connectionString);
            try
            {
                result.Open();
            } catch (Exception exc)
            {
                return null;
            }
            return result;
        }

        public static OracleCommand CreateProcedure(string procedureName)
        {
            OracleCommand procedure = new OracleCommand(procedureName)
            {
                CommandType = CommandType.StoredProcedure
            };
            return procedure;
        }
        public static OracleCommand AddStringParameter(this OracleCommand procedure, string value)
        {
            OracleParameter param = procedure.CreateParameter();
            param.DbType = DbType.String;
            param.Value = value;
            procedure.Parameters.Add(param);
            return procedure;
        }
        public static OracleCommand AddDateTimeParameter(this OracleCommand procedure, DateTime value)
        {
            OracleParameter param = procedure.CreateParameter();
            param.DbType = DbType.DateTime;
            param.Value = value;
            procedure.Parameters.Add(param);
            return procedure;
        }

        public static OracleCommand ExtractProcedure(Procedure procedure)
        {
            var result = CreateProcedure(procedure.ProcedureName);
            procedure.ProcedureParams.ForEach(x => result.AddStringParameter(x));
            return result;
        }
    }
}
