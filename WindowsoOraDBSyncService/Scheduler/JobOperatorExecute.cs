using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OracleProcedureManager;
using Serilog;

namespace OraDBSyncService.Scheduler
{
    class JobOperatorExecute : IJobOperator
    {
        public MainScheduler Scheduler => MainScheduler.GetMainScheduler();

        public Task<bool> Operate(SynchronizationTask taskJob)
        {
            return TriggerTaskAsync(taskJob);
        }

        /// <summary>
        /// Triggers sync task without scheduling it
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        private async Task<bool> TriggerTaskAsync(SynchronizationTask task)
        {
            try
            {
                if (!await Scheduler.CheckTaskInSchedule(task.SyncTaskId))
                    await Scheduler.StartTaskAsync(task);
                await Scheduler.TriggerTaskAsync(task.SyncTaskId);
                return true;
            }
            catch (Exception e)
            {
                Log.Error($"Failed to trigger task: {task.SyncTaskId}");
                return false;
            }
        }
    }
}
