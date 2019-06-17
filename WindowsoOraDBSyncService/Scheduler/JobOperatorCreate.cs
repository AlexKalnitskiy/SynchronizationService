using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OracleProcedureManager;
using Serilog;

namespace OraDBSyncService.Scheduler
{
    public class JobOperatorCreate : IJobOperator
    {
        public MainScheduler Scheduler => MainScheduler.GetMainScheduler();

        Task<bool> IJobOperator.Operate(SynchronizationTask taskJob)
        {
            return StartTaskAsync(taskJob);
        }

        /// <summary>
        /// Tries to schedule Job from json (SynchronizationTask)
        /// </summary>
        /// <param name="jsonTask"></param>
        /// <returns></returns>
        private async Task<bool> StartTaskAsync(SynchronizationTask task)
        {
            try
            {
                if (await Scheduler.CheckTaskInSchedule(task.SyncTaskId))
                {
                    return false;
                }
                else
                {
                    await Scheduler.StartTaskAsync(task);
                    return true;
                }
            }
            catch (Exception e)
            {
                Log.Error($"Failed to schedule task: {task.SyncTaskId}");
                return false;
            }
        }
    }
}
