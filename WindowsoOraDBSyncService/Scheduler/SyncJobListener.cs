using OracleProcedureManager;
using OraDBSyncService.Logging;
using Quartz;
using Serilog;
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
            BPMTaskSpecialLog.Log(syncTask.SyncTaskId, $"Начало выполнения задачи синхронизации");
            BPMTaskSpecialLog.TaskStateLog(syncTask.SyncTaskId, TaskStateConstants.Executing);
            return Task.Run(() => this.GetHashCode());
        }

        public Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException, CancellationToken cancellationToken = default(CancellationToken))
        {
            SyncTaskExecutionContext result = (SyncTaskExecutionContext)context.Result;
            if (result.TaskCurrentState == TaskStateConstants.Ready)
            {
                Log.Warning($"Executed SyncTask with ID: {result.SyncTaskId}");
                BPMTaskSpecialLog.Log(result.SyncTaskId, $"Выполнение задачи успешно завершено");
                BPMTaskSpecialLog.TaskStateLog(result.SyncTaskId, TaskStateConstants.Ready);
            }
            else if (result.TaskCurrentState == TaskStateConstants.Failed)
            {
                result.ExceptionList.ForEach(x =>
                {
                    Log.Error(x, $"Error at Job with ID: {result.SyncTaskId}");
                    BPMTaskSpecialLog.Log(result.SyncTaskId, $"Ошибка синхронизации: {x.Message}");
                });
                BPMTaskSpecialLog.TaskStateLog(result.SyncTaskId, TaskStateConstants.Failed);
            }
            WebServer.CommonRequestRouter.GetRouter().Broadcast(WebServer.CommonRequestRouter.Refresh);
            return Task.Run(() => this.GetHashCode());
        }
    }
}
