using System;
using System.Threading.Tasks;
using OracleProcedureManager;
using Serilog;

namespace OraDBSyncService.Scheduler
{
    class JobOperatorReplace : IJobOperator
    {
        public MainScheduler Scheduler => MainScheduler.GetMainScheduler();

        public Task<JobOperatorResponce> Operate(SynchronizationTask taskJob)
        {
            return ReplaceTaskAsync(taskJob);
        }

        /// <summary>
        /// Tries to schedule Job from json (SynchronizationTask)
        /// </summary>
        /// <param name="jsonTask"></param>
        /// <returns></returns>
        private async Task<JobOperatorResponce> ReplaceTaskAsync(SynchronizationTask task)
        {
            try
            {
                if (await Scheduler.CheckTaskInSchedule(task.SyncTaskId))
                    await Scheduler.DeleteTaskWithInterruptAsync(task.SyncTaskId);
                await Scheduler.StartTaskAsync(task);
                return new JobOperatorResponce(true, ResponceConstants.ReplaceSuccess + task.SyncTaskId);
            }
            catch (Exception e)
            {
                Log.Error($"Failed to replace task: {task.SyncTaskId}");
                return new JobOperatorResponce(false, $"{ResponceConstants.ReplaceFail}Задача: {task.SyncTaskId}.\r\nОшибка: {e.Message}");
            }
        }
    }
}
