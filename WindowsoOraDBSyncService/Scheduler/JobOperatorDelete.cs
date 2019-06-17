using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OracleProcedureManager;
using Serilog;

namespace OraDBSyncService.Scheduler
{
    class JobOperatorDelete : IJobOperator
    {
        public MainScheduler Scheduler => MainScheduler.GetMainScheduler();

        public Task<bool> Operate(SynchronizationTask taskJob)
        {
            return DeleteTaskAsync(taskJob.SyncTaskId);
        }

        /// <summary>
        /// Tries to delete job from scheduler
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        private async Task<bool> DeleteTaskAsync(string taskId)
        {
            try
            {
                if (await Scheduler.CheckTaskInSchedule(taskId))
                {

                    return await Scheduler.DeleteTaskAsync(taskId);
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Log.Error($"Failed to delete task: {taskId}");
                return false;
            }
        }
    }
}
