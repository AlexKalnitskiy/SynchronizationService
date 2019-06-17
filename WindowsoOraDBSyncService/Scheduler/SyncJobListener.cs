using OracleProcedureManager;
using Quartz;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OraDBSyncService.Scheduler
{
    class SyncJobListener : IJobListener
    {
        public string Name => "DefaultJobListener";

        public Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Run(() => this.GetHashCode());
        }

        public Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            SynchronizationTask syncTask = (SynchronizationTask)context.JobDetail.JobDataMap["task"];
            Log.Warning($"Executing SyncTask with ID: {syncTask.SyncTaskId}");
            BPMTaskSpecialLog.Log(syncTask.SyncTaskId, $"Выполнение задачи синхронизации");
            //filler
            return Task.Run(() => this.GetHashCode());
        }

        public Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException, CancellationToken cancellationToken = default(CancellationToken))
        {
            SyncTaskExecutionResult result = (SyncTaskExecutionResult)context.Result;
            if (result.isExecutedCorrectly)
            {
                Log.Warning($"Executed SyncTask with ID: {result.SyncTaskId}");
                BPMTaskSpecialLog.Log(result.SyncTaskId, $"Выполнение задачи успешно завершено");
            }
            else
                result.ExceptionList.ForEach(x => {
                    Log.Error(x, $"Error at Job with ID: {result.SyncTaskId}");
                    BPMTaskSpecialLog.Log(result.SyncTaskId, $"Ошибка синхронизации: {x.Message}");
                });
            //filler
            return Task.Run(() => this.GetHashCode());
        }
    }
}
