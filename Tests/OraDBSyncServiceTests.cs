using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OracleProcedureManager;
using OraDBSyncService;
using OraDBSyncService.WebServer;
using Quartz;
using System.Threading.Tasks;
using System.Threading;
using Quartz.Impl.Matchers;
using System.Diagnostics;
using OraDBSyncService.Scheduler;
using WebSocket4Net;

namespace Tests
{
    /*TODO
     Теперь операция синхронизации возвращает сериализуемый объект. А на джобы можно повесить лисенер. Поэтому требуется сделать адекватный
     роутер с добавлением, отслеживанием задач, проверить прерывания в синхронизации и т.д.
         
         */

    class AllJobListener : IJobListener
    {
        OraDBSyncServiceTests Test = new OraDBSyncServiceTests();

        public string Name => "Default";

        public AllJobListener(OraDBSyncServiceTests testMethod)
        {
            Test = testMethod;
        }

        public Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Run(() => this.GetHashCode());
        }

        public Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException, CancellationToken cancellationToken = default(CancellationToken))
        {
            Test.Results.Add((SyncTaskExecutionResult)context.Result);
            return Task.Run(() => this.GetHashCode());
        }
    }

    class ClientUnit
    {
        public static int ResponseCount = 0;

        string Uri = "ws://127.0.0.1:2019/";

        public ClientUnit()
        {
        }

        public ClientUnit(string Uri)
        {
            this.Uri = Uri;
        }
        public void Send(string text)
        {
            WebSocket socket = new WebSocket(Uri);
            socket.MessageReceived += Socket_MessageReceived;
            socket.Open();
            while (socket.State == WebSocketState.Connecting)
            {
            }
            socket.Send(text);
        }

        private void Socket_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            ResponseCount++;
        }
    }


    [TestClass]
    public class OraDBSyncServiceTests
    {
        int tasksCount = 4;
        public List<SyncTaskExecutionResult> Results = new List<SyncTaskExecutionResult>();

        [TestMethod]
        public void SchedulerTaskLifeCycleTest()
        {
            //Arrange
            string[] expectedId = { "9B2EC227-F96D-4859-872A-701EF8A1C65E", "12CaseCategory", "12ITAreaType", "12ITCaseKind" };
            string[] expectedSchemaName = { "CaseOrigin", "CaseCategory", "ITAreaType", "ITCaseKind" };
            string[] expectedProcedureName = { "SYNC_LOOKUP_CASEORIGIN", "SYNC_LOOKUP_CASECATEGORY", "SYNC_LOOKUP_ITAREATYPE", "SYNC_LOOKUP_ITCASEKIND" };
            string expectedConnection = ServiceSettings.Get().ConnectionString;
            SynchronizationTask[] tasks = new SynchronizationTask[tasksCount];

            //Arrange Test Tasks (with no params)
            for (int i = 0; i < tasksCount; i++)
            {
                tasks[i] = new SynchronizationTask()
                {
                    SyncTaskId = expectedId[i],
                    CronExpression = "0 */1 0-23 */1 * ? *",
                    SyncObjectList = new List<SynchronizationObject>()
                {
                    new SynchronizationObject()
                    {
                        Order = 1,
                        SchemaName = expectedSchemaName[i],
                        WithNoIndex = false,
                        ProceduresList = new List<Procedure>()
                        {
                            new Procedure()
                            {
                                ProcedureName = expectedProcedureName[i],
                                ProcedureParams = new List<string>(),
                                Order = 1
                            }
                        }
                    }
                }
                };
            }


            //Initialiae services like OnStart() service method
            OraDBSyncService.OraDBSyncService actualServ = new OraDBSyncService.OraDBSyncService();
            actualServ.InitLog();
            SocketServer server = SocketServer.GetDefaultServer();
            actualServ.Router = new CommonRequestRouter();
            bool serverStartedFlag = Task.Factory.StartNew(server.Start, TaskCreationOptions.LongRunning).Result;
            IScheduler scheduler = MainScheduler.GetMainScheduler().Scheduler;



            AddTasksByClientMockUnit();
            void AddTasksByClientMockUnit()
            {

                for (int i = 0; i < tasksCount; i++)
                {
                    tasks[i].RouterCommand = RouterCommands.Create.ToString();
                    ClientUnit mock = new ClientUnit();
                    mock.Send(tasks[i].ToJson());

                    //bool startResult = actualServ.Router.StartTaskAsync(tasks[i]).Result;

                    //Assert Scheduling
                    //Assert.IsTrue(startResult);
                }

                //Waiting all results
                var resp = Stopwatch.StartNew();
                while (ClientUnit.ResponseCount < tasksCount && resp.ElapsedMilliseconds < 10000)
                {

                }
                resp.Stop();
            }



            //Cheking scheduled tasks
            for (int i = 0; i < tasksCount; i++)
            {
                var actualDetails = scheduler.GetJobDetail(new JobKey(expectedId[i])).Result;
                string actualConnectionString = actualDetails.JobDataMap.Get("connection").ToString();
                SynchronizationTask actualTask = (SynchronizationTask)actualDetails.JobDataMap.Get("task");

                //Assert Scheduling2
                Assert.AreEqual(expectedId[i], actualDetails.Key.Name);
                Assert.AreEqual(expectedConnection, actualConnectionString);
                Assert.AreEqual(tasks[i].CronExpression, actualTask.CronExpression);
                Assert.AreEqual(tasks[i].SyncTaskId, actualTask.SyncTaskId);
                Assert.AreEqual(tasks[i].SyncObjectList[0].SchemaName, actualTask.SyncObjectList[0].SchemaName);
            }

            //Adding a job listener
            scheduler.ListenerManager.AddJobListener(new AllJobListener(this), EverythingMatcher<JobKey>.AllJobs());

            //Triggering tasks
            Task[] jobs = new Task[tasksCount];
            for (int i = 0; i < tasksCount; i++)
            {
                jobs[i] = scheduler.TriggerJob(new JobKey(expectedId[i]));
            }
            Task.WaitAll(jobs);

            //Waiting all results
            var timeout = Stopwatch.StartNew();
            while (Results.Count != tasksCount && timeout.ElapsedMilliseconds < 10000)
            {

            }

            //Elapsed time
            timeout.Stop();
            long elapsed = timeout.ElapsedMilliseconds;

            //Asserting results
            for (int i = 0; i < tasksCount; i++)
            {
                Assert.IsTrue(Results[i].isExecutedCorrectly);
            }


            //Deleting
            DeleteTasksByClientMockUnit();

            void DeleteTasksByClientMockUnit()
            {
                for (int i = 0; i < tasksCount; i++)
                {
                    tasks[i].RouterCommand = RouterCommands.Delete.ToString();
                    ClientUnit mock = new ClientUnit();
                    mock.Send(tasks[i].ToJson());
                }

                ClientUnit.ResponseCount = 0;
                //Waiting all results
                var resp = Stopwatch.StartNew();
                while (ClientUnit.ResponseCount < tasksCount && resp.ElapsedMilliseconds < 10000)
                {

                }
                resp.Stop();
            }

            //Deleting tasks
            for (int i = 0; i < tasksCount; i++)
            {
                bool checkTaskFalse = MainScheduler.GetMainScheduler().CheckTaskInSchedule(expectedId[i]).Result;
                //Delete Assert
                Assert.IsFalse(checkTaskFalse);
            }
        }
    }

    [TestClass]
    public class OraDBRouterTests
    {
        [TestMethod]
        public void ValidateRouterCommandTestSuccess()
        {
            //Arrange
            RouterCommands expected = RouterCommands.Check;
            string command = expected.ToString();


            //Act
            RouterCommands actual = RouterValidator.Validate(command);


            //Assert
            Assert.AreEqual(expected, actual);
        }
        public void ValidateRouterCommandTestFail()
        {
            //Arrange
            RouterCommands expected = RouterCommands.Unknown;
            string command = "Failture Command";


            //Act
            RouterCommands actual = RouterValidator.Validate(command);


            //Assert
            Assert.AreEqual(expected, actual);
        }
    }

    [TestClass]
    public class LogTests
    {
        [TestMethod]
        public void LoggerTest()
        {
            Assert.IsTrue(true);
        }
    }
}
