using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace OracleProcedureManager
{
    /// <summary>
    /// Class to manipulate oracle procedures execution
    /// </summary>
    public sealed class OracleScriptController
    {
        #region Properties

        public OracleConnection Connection;

        #endregion Properties

        #region Events

        private delegate void ExecutionStateHandler(string schemaName);
        private event ExecutionStateHandler ExecutionStartedEvent;
        private event ExecutionStateHandler ExecutionFinishedEvent;
        private event ExecutionStateHandler ExecutionCancelledEvent;
        private delegate void ExecutionExceptionHandler(string schemaName, Exception ex);
        private event ExecutionExceptionHandler ErrorEvent;

        #endregion

        #region Constructors
        public OracleScriptController(OracleConnection connection)
        {
            Connection = connection;
        }
        #endregion Constructors

        #region Methods:Public

        public void AddListener(ISyncListener listener)
        {
            ExecutionStartedEvent += listener.ExecutionStartedEvent;
            ExecutionFinishedEvent += listener.ExecutionFinishedEvent;
            ExecutionCancelledEvent += listener.ExecutionCancelledEvent;
            ErrorEvent += listener.ErrorEvent;
        }

        public Task InterruptAll()
        {
            return Task.Run(() => Connection.Close());
        }

        public void PerformOperation(OracleCommand command)
        {
            command.Connection = Connection;
            OracleTransaction transaction = Connection.BeginTransaction();
            command.ExecuteNonQuery();
            transaction.Commit();
        }

        public Task PerformOperationAsync(OracleCommand command)
        {
            return Task.Run(() => PerformOperation(command));
        }

        public async Task<SyncObjExecutionResult> ExecuteProcedurePackAsync(OracleProcedurePack SyncObjectPack, CancellationToken token)
        {
            SyncObjExecutionResult objResult = new SyncObjExecutionResult()
            {
                SchemaName = SyncObjectPack.SchemaName,
                isExecuted = true
            };

            foreach (OracleCommand procedure in SyncObjectPack.Procedures.Values)
            {
                try
                {
                    if (!token.IsCancellationRequested)
                    {
                        ExecutionStartedEvent(SyncObjectPack.SchemaName);
                        await PerformOperationAsync(procedure);
                        ExecutionFinishedEvent(SyncObjectPack.SchemaName);
                    }
                    else
                    {
                        await InterruptAll();
                        objResult.ExceptionMessage = Settings.isInterruptRequestedMessage;
                        objResult.isExecuted = false;
                        ExecutionCancelledEvent(SyncObjectPack.SchemaName);
                        break;
                    }
                }
                catch (Exception ex)
                {
                    objResult.Exception = ex;
                    objResult.ExceptionMessage = ex.Message;
                    objResult.isExecuted = false;
                    await InterruptAll();
                    ErrorEvent(SyncObjectPack.SchemaName, ex);
                    break;
                }
            }
            return objResult;
        }

        public void ExecuteProcedurePack(OracleProcedurePack SyncObjectPack)
        {
            if (Connection.State != System.Data.ConnectionState.Open)
                Connection.Open();
            foreach (OracleCommand procedure in SyncObjectPack.Procedures.Values)
            {
                PerformOperationAsync(procedure);
                SyncObjectPack.SuccessfullyCompleted = true;
            }  
        }

        public async Task<SyncTaskExecutionResult> ExecuteSyncronizationTaskAsync(SynchronizationTask task, CancellationToken token)
        {
            //Result object init
            SyncTaskExecutionResult result = new SyncTaskExecutionResult()
            {
                SyncTaskId = task.SyncTaskId,
                isExecutedCorrectly = true,
                ObjectResultsList = new List<SyncObjExecutionResult>(),
                ExceptionList = new List<Exception>()
            };

            //Synchronization loop
            foreach (var x in task.SyncObjectList.ParseToProcedurePack())
            {
                /*if (x.WithNoIndex)
                     NoIndexExecute(x.SchemaName, () => ExecuteProcedurePack(x));
                 else*/

                SyncObjExecutionResult executionResult = await ExecuteProcedurePackAsync(x, token);

                //Collects all SyncObjectsResults
                result.ObjectResultsList.Add(executionResult);
            }
            return result.CollectResults();
        }
        //TODO: Refactor for async
        public void NoIndexExecute(string schemaName, Action executeMethod)
        {
            OracleProcedurePack disablePack = new OracleProcedurePack($"disable all on {schemaName}");
            disablePack.Procedures.Add(1, OracleProcedureBuilder.CreateProcedure(Settings.ConstraintDisableProcName).AddStringParameter(schemaName));
            disablePack.Procedures.Add(2, OracleProcedureBuilder.CreateProcedure(Settings.IndexUnusableProcName).AddStringParameter(schemaName));
            OracleProcedurePack enablePack = new OracleProcedurePack($"enable all on {schemaName}");
            enablePack.Procedures.Add(1, OracleProcedureBuilder.CreateProcedure(Settings.IndexRebuildProcName).AddStringParameter(schemaName));
            enablePack.Procedures.Add(2, OracleProcedureBuilder.CreateProcedure(Settings.ConstraintEnableProcName).AddStringParameter(schemaName));

            ExecuteProcedurePack(disablePack);
            executeMethod.Invoke();
            ExecuteProcedurePack(enablePack);

        }
        #endregion Methods:Public
    }
}

