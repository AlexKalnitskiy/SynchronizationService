using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OracleProcedureManager;
using Serilog;

namespace OraDBSyncService.Scheduler
{
    class JobOperatorReplace : IJobOperator
    {
        public MainScheduler Scheduler => MainScheduler.GetMainScheduler();

        public Task<bool> Operate(SynchronizationTask taskJob)
        {
            return ReplaceTaskAsync(taskJob);
        }

        /// <summary>
        /// Tries to schedule Job from json (SynchronizationTask)
        /// </summary>
        /// <param name="jsonTask"></param>
        /// <returns></returns>
        private async Task<bool> ReplaceTaskAsync(SynchronizationTask task)
        {
            try
            {
                if (await Scheduler.CheckTaskInSchedule(task.SyncTaskId))
                    await Scheduler.DeleteTaskAsync(task.SyncTaskId);
                await Scheduler.StartTaskAsync(task);
                return true;
            }
            catch (Exception e)
            {
                Log.Error($"Failed to replace task: {task.SyncTaskId}");
                return false;
            }
        }
    }
}
