using Serilog;

namespace OraDBSyncService.Logging
{
    public static class SerilogInit
    {
        public static void InitLog()
        {
            //Logger initialization
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File(
                    "logMain-.txt",
                    restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Warning,
                    rollingInterval: RollingInterval.Day)
                .WriteTo.File(
                    "logDebug-.txt",
                    restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Debug,
                    rollingInterval: RollingInterval.Day
                )
                .WriteTo.File(
                    "logError-.txt",
                    restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error,
                    rollingInterval: RollingInterval.Day
                )
                .WriteTo.OracleSink()
                .CreateLogger();
        }
    }
}
