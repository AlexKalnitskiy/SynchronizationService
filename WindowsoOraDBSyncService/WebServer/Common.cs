using OracleProcedureManager;
using Serilog;
using System;
using System.Threading.Tasks;
using SyncServiceClient;

namespace OraDBSyncService.Scheduler
{
    public static class RouterValidator
    {
        public static RouterCommands Validate(string command)
        {
            try
            {
                return (RouterCommands)Enum.Parse(typeof(RouterCommands), command);
            }
            catch (Exception ex)
            {
                Log.Error("Cron-expression validation failed");
                return RouterCommands.Unknown;
            }
        }
    }

    public static class Extensions
    {
        public static async Task<JobOperatorResponce> OperateTaskRequest(this SynchronizationTask it)
        {
            return await JobOperatorFactory.GetJobOperator(RouterValidator.Validate(it.RouterCommand)).Operate(it);
        }
    }
}
