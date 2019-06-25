using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OracleProcedureManager;
using OraDBSyncService.Logging;
using Serilog;

namespace OraDBSyncService.Scheduler
{
    class JobOperatorInterrupt : IJobOperator
    {
        public MainScheduler Scheduler => MainScheduler.GetMainScheduler();

        public Task<JobOperatorResponce> Operate(SynchronizationTask taskJob)
        {
            return InterruptTaskAsync(taskJob.SyncTaskId);
        }

        /// <summary>
        /// Checks task in scheduler
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        private async Task<JobOperatorResponce> InterruptTaskAsync(string taskId)
        {
            bool check = await Scheduler.InterruptTask(taskId);
            if (check)
            {
                Log.Information($"Task interrupted: {taskId}");
                BPMTaskSpecialLog.TaskStateLog(taskId, TaskStateConstants.Ready);
                return new JobOperatorResponce(true, ResponceConstants.InterruptSuccess + taskId);
            }
            return new JobOperatorResponce(false, ResponceConstants.InterruptFail + taskId);
        }
    }
}
