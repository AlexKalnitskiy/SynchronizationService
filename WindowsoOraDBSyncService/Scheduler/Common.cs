using OracleProcedureManager;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OraDBSyncService.Scheduler
{

    public enum RouterCommands { Create, Delete, Check, Replace, Execute, Unknown }

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
        public static async Task<bool> OperateTaskRequest(this SynchronizationTask it)
        {
            return await JobOperatorFactory.GetJobOperator(RouterValidator.Validate(it.RouterCommand)).Operate(it);
        }
    }
}
