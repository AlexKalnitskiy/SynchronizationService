using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OracleProcedureManager;

namespace OraDBSyncService.Scheduler
{
    class JobOperatorCheck : IJobOperator
    {
        public MainScheduler Scheduler => MainScheduler.GetMainScheduler();

        public Task<bool> Operate(SynchronizationTask taskJob)
        {
            return CheckTaskAsync(taskJob.SyncTaskId);
        }

        /// <summary>
        /// Checks task in scheduler
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        private async Task<bool> CheckTaskAsync(string taskId)
        {
            return await Scheduler.CheckTaskInSchedule(taskId);
        }
    }
}
