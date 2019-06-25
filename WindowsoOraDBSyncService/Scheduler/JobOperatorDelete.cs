using System;
using System.Threading.Tasks;
using OracleProcedureManager;
using OraDBSyncService.Logging;
using Serilog;

namespace OraDBSyncService.Scheduler
{
    class JobOperatorDelete : IJobOperator
    {
        public MainScheduler Scheduler => MainScheduler.GetMainScheduler();

        public Task<JobOperatorResponce> Operate(SynchronizationTask taskJob)
        {
            return DeleteTaskAsync(taskJob.SyncTaskId);
        }

        /// <summary>
        /// Tries to delete job from scheduler
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        private async Task<JobOperatorResponce> DeleteTaskAsync(string taskId)
        {
            try
            {
                if (await Scheduler.CheckTaskInSchedule(taskId))
                {

                    bool check = await Scheduler.DeleteTaskWithInterruptAsync(taskId);
                    Log.Information($"Task deleted: {taskId}");
                    BPMTaskSpecialLog.TaskStateLog(taskId, TaskStateConstants.NotInSchedule);
                    return new JobOperatorResponce(check, ResponceConstants.DeleteSuccess + taskId);
                }
                else
                {
                    return new JobOperatorResponce(false, ResponceConstants.DeleteFail + taskId);
                }
            }
            catch (Exception e)
            {
                Log.Error($"Failed to delete task: {taskId}");
                return new JobOperatorResponce(false, $"{ResponceConstants.DeleteFail}Задача: {taskId}.\r\nОшибка: {e.Message}");
            }
        }
    }
}
