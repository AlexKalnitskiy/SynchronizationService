using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using OracleProcedureManager;
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;

namespace OraDBSyncService.Logging
{
    class OracleSink : ILogEventSink
    {
        private string _logFunctionName;
        private readonly LogEventLevel _minimumEventLevel;
        public OracleSink(LogEventLevel restrictedToMinimumLevel)
        {
            _minimumEventLevel = restrictedToMinimumLevel;
            _logFunctionName = ServiceSettings.Get().OracleMainLogFunction;
        }

        public void Emit(LogEvent logEvent)
        {
            if (logEvent.Level >= _minimumEventLevel)
            {
                DBLog(logEvent.Level.ToString(), logEvent.MessageTemplate.Render(logEvent.Properties));
            }
        }
        private OracleCommand GetWriteInDBCommand(string type, string data)
        {
            OracleCommand logRecord = OracleProcedureBuilder.CreateProcedure(_logFunctionName);
            logRecord = OracleProcedureBuilder.AddStringParameter(logRecord, type);
            logRecord = OracleProcedureBuilder.AddStringParameter(logRecord, data);
            return logRecord;
        }
        private void DBLog(string type, string data)
        {
            OracleCommand logRecord = GetWriteInDBCommand(type, data);
            logRecord.Connection = ServiceSettings.Get().ConnectionString.OpenNewConnection();
            OracleTransaction trasaction = logRecord.Connection.BeginTransaction();
            logRecord.ExecuteNonQuery();
            trasaction.Commit();
        }
    }

    public static class OracleSinkExtensions
    {
        public static LoggerConfiguration OracleSink(
                  this LoggerSinkConfiguration loggerConfiguration, LogEventLevel restrictedToMinimumLevel = LogEventLevel.Debug)
        {
            return loggerConfiguration.Sink(new OracleSink(restrictedToMinimumLevel));
        }
    }
}
