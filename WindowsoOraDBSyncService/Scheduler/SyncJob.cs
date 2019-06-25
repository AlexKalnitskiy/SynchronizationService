using OracleProcedureManager;
using OraDBSyncService.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OraDBSyncService.Scheduler
{
    internal class BPMSyncJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            //return Task.Run(() => LaunchScenario(context));
            return LaunchScenarioAsync(context);
        }

        private async Task LaunchScenarioAsync(IJobExecutionContext context)
        {
            //Get info from context
            string connectionString = context.JobDetail.JobDataMap["connection"].ToString();
            SynchronizationTask syncTask = (SynchronizationTask)context.JobDetail.JobDataMap["task"];

            OracleScriptController oracleScriptController = new OracleScriptController(connectionString.OpenNewConnection());
            oracleScriptController.AddListener(new BPMListener(syncTask.SyncTaskId));

            //Start sync
            SyncTaskExecutionContext taskExecutionResult =
                await oracleScriptController.ExecuteSyncronizationTaskAsync(syncTask, context.CancellationToken);
            context.Result = taskExecutionResult;
        }
    }

    //TODO Здесь одинаково для разных процедур! Дублирование информации
    internal class BPMListener : ISyncListener
    {
        string _taskId;
        public BPMListener(string taskId)
        {
            _taskId = taskId;
        }
        public void ErrorEvent(string schemaName, Exception ex)
        {
            BPMTaskSpecialLog.Log(_taskId, $"Ошибка синхронизации: {schemaName}", Serilog.Events.LogEventLevel.Error);
        }

        public void ExecutionCancelledEvent(string objectName, string procedureName)
        {
            BPMTaskSpecialLog.Log(_taskId, $"Отмена синхронизации: {objectName}: {procedureName}");
        }

        public void ExecutionFinishedEvent(string objectName, string procedureName)
        {
            BPMTaskSpecialLog.Log(_taskId, $"Завершение синхронизации: {objectName}: {procedureName}");
        }

        public void ExecutionStartedEvent(string objectName, string procedureName)
        {
            BPMTaskSpecialLog.Log(_taskId, $"Начало синхронизации: {objectName}: {procedureName}");
        }
    }
}
