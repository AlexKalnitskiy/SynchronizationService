using OracleProcedureManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OraDBSyncService.Scheduler
{
    public interface IJobOperator
    {
        MainScheduler Scheduler { get; }

        Task<bool> Operate(SynchronizationTask taskJob);
    }
}
