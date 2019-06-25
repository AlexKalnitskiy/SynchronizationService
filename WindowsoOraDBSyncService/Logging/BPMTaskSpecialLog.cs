using Oracle.ManagedDataAccess.Client;
using OracleProcedureManager;
using System;

namespace OraDBSyncService.Logging
{
    public static class BPMTaskSpecialLog
    {
        static string _logFunctionName = ServiceSettings.Get().OracleLogFunction;
        static string _stateLogFunctionName = ServiceSettings.Get().OracleStateLogFunction;
        private static OracleCommand GetWriteInDBCommand(string taskId, string type, string data)
        {
            OracleCommand logRecord = OracleProcedureBuilder.CreateProcedure(_logFunctionName);
            logRecord = OracleProcedureBuilder.AddStringParameter(logRecord, "{" + taskId.ToUpper() + "}");
            logRecord = OracleProcedureBuilder.AddStringParameter(logRecord, type);
            logRecord = OracleProcedureBuilder.AddStringParameter(logRecord, data);
            return logRecord;
        }
        public static void Log(string taskId, string data, Serilog.Events.LogEventLevel level = Serilog.Events.LogEventLevel.Information)
        {
            try
            {
                OracleCommand logRecord = GetWriteInDBCommand(taskId, level.ToString(), data);
                logRecord.Connection = ServiceSettings.Get().ConnectionString.OpenNewConnection();
                OracleTransaction trasaction = logRecord.Connection.BeginTransaction();
                logRecord.ExecuteNonQuery();
                trasaction.Commit();
            }
            catch (Exception ex)
            {
                Serilog.Log.Error($"BPMLog error: {ex.Message}");
            }
        }

        public static void TaskStateLog(string taskId, int state)
        {
            try
            {
                OracleCommand logRecord = OracleProcedureBuilder.CreateProcedure(_stateLogFunctionName);
                logRecord = OracleProcedureBuilder.AddStringParameter(logRecord, "{" + taskId.ToUpper() + "}");
                logRecord = OracleProcedureBuilder.AddStringParameter(logRecord, state.ToString());
                logRecord.Connection = ServiceSettings.Get().ConnectionString.OpenNewConnection();
                OracleTransaction trasaction = logRecord.Connection.BeginTransaction();
                logRecord.ExecuteNonQuery();
                trasaction.Commit();
            }
            catch (Exception ex)
            {
                Serilog.Log.Error($"BPMLog error: {ex.Message}");
            }
        }
    }
}
