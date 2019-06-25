using System;
using System.Threading.Tasks;
using OracleProcedureManager;
using OraDBSyncService.Logging;
using Serilog;

namespace OraDBSyncService.Scheduler
{
    public class JobOperatorCreate : IJobOperator
    {
        public MainScheduler Scheduler => MainScheduler.GetMainScheduler();

        Task<JobOperatorResponce> IJobOperator.Operate(SynchronizationTask taskJob)
        {
            return StartTaskAsync(taskJob);
        }

        /// <summary>
        /// Tries to schedule Job from json (SynchronizationTask)
        /// </summary>
        /// <param name="jsonTask"></param>
        /// <returns></returns>
        private async Task<JobOperatorResponce> StartTaskAsync(SynchronizationTask task)
        {
            try
            {
                if (await Scheduler.CheckTaskInSchedule(task.SyncTaskId))
                {
                    return new JobOperatorResponce(false, ResponceConstants.CreateFailExists + task.SyncTaskId);
                }
                else
                {
                    await Scheduler.StartTaskAsync(task);
                    Log.Information($"Task created: {task.SyncTaskId}");
                    BPMTaskSpecialLog.TaskStateLog(task.SyncTaskId, TaskStateConstants.Ready);
                    return new JobOperatorResponce(true, ResponceConstants.CreateSuccess + task.SyncTaskId);
                }
            }
            catch (Exception e)
            {
                Log.Error($"Failed to schedule task: {task.SyncTaskId}");
                return new JobOperatorResponce(false, $"{ResponceConstants.CreateFail}Задача: {task.SyncTaskId}.\r\nОшибка: {e.Message}");
            }
        }
    }
}
