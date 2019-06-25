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

        public Task<JobOperatorResponce> Operate(SynchronizationTask taskJob)
        {
            return CheckTaskAsync(taskJob.SyncTaskId);
        }

        /// <summary>
        /// Checks task in scheduler
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        private async Task<JobOperatorResponce> CheckTaskAsync(string taskId)
        {
            bool check = await Scheduler.CheckTaskInSchedule(taskId);
            if (!check)
                return new JobOperatorResponce(check, ResponceConstants.CheckFail + taskId);
            else
            {
                Quartz.ITrigger infoTrigger = await Scheduler.GetTriggetInfo(taskId);
                var context = Scheduler.Scheduler.GetJobDetail(new Quartz.JobKey(taskId));
                
                string template = $"{ResponceConstants.CheckSuccess}Задача: {taskId}\r\n";
                template += $"Текущий статус: {context.Status.ToString()}\r\n";
                if (infoTrigger.GetPreviousFireTimeUtc().HasValue)
                    template += $"Последний запуск: {infoTrigger.GetPreviousFireTimeUtc().Value.ToLocalTime().ToString()}\r\n";
                if (infoTrigger.GetNextFireTimeUtc().HasValue)
                    template += $"Следующий запуск: {infoTrigger.GetNextFireTimeUtc().Value.ToLocalTime().ToString()}";
                return new JobOperatorResponce(check, template);
            }
        }
    }
}
