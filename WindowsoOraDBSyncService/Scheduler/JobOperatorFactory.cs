using OraDBSyncService.WebServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OraDBSyncService.Scheduler
{
    public static class JobOperatorFactory
    {
        public static IJobOperator GetJobOperator(RouterCommands command)
        {
            if (command == RouterCommands.Create)
                return new JobOperatorCreate();
            if (command == RouterCommands.Delete)
                return new JobOperatorDelete();
            if (command == RouterCommands.Check)
                return new JobOperatorCheck();
            if (command == RouterCommands.Replace)
                return new JobOperatorReplace();
            if (command == RouterCommands.Execute)
                return new JobOperatorExecute();
            if (command == RouterCommands.Unknown)
                return new JobOperatorCheck();

            return new JobOperatorCheck();
        }
    }
}
