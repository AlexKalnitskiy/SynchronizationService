using OracleProcedureManager;
using OraDBSyncService.Logging;
using OraDBSyncService.Scheduler;
using Quartz;
using Quartz.Impl.Matchers;
using Serilog;
using System;
using System.Threading.Tasks;

namespace OraDBSyncService.WebServer
{
    public class CommonRequestRouter
    {
        public const string Refresh = "Refresh";

        #region Singleton

        private static CommonRequestRouter _singleton;

        public static CommonRequestRouter GetRouter()
        {
            if (_singleton == null)
            {
                try
                {
                    _singleton = new CommonRequestRouter();
                    //Init
                    _singleton.Scheduler = MainScheduler.GetMainScheduler();
                    _singleton.JobListener = new SyncJobListener();
                    _singleton.WebServer = SocketServer.GetDefaultServer();
                    Log.Debug("Router Initialized");

                    //Adding listeners
                    _singleton.Scheduler.Scheduler.ListenerManager.AddJobListener(_singleton.JobListener, EverythingMatcher<JobKey>.AllJobs());
                    _singleton.WebServer.NewMessageReceived += _singleton.OnMessageRecieved;
                    Log.Debug("Router listeners Initialized");
                }
                catch (Exception e)
                {
                    Log.Error("Error while launching Web-Socket service: {0}", e);
                    throw;
                }
            }
            return _singleton;
        }
        #endregion Singleton

        MainScheduler Scheduler;
        SocketServer WebServer;
        SyncJobListener JobListener;

        private CommonRequestRouter()
        {
            /*//Init
            Scheduler = MainScheduler.GetMainScheduler();
            JobListener = new SyncJobListener();
            WebServer = SocketServer.GetDefaultServer();
            Log.Debug("Router Initialized");

            //Adding listeners
            Scheduler.Scheduler.ListenerManager.AddJobListener(JobListener, EverythingMatcher<JobKey>.AllJobs());
            WebServer.NewMessageReceived += OnMessageRecieved;
            Log.Debug("Router listeners Initialized");*/
        }

        public void DiscardAllStates()
        {
            foreach (var item in Scheduler.Scheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup()).Result)
            {
                BPMTaskSpecialLog.TaskStateLog(item.Name, TaskStateConstants.NotInSchedule);
            }
            Broadcast(Refresh);
        }
        public void Broadcast(string message)
        {
            foreach (var item in WebServer.GetAllSessions())
            {
                item.Send(message);
            }
        }
        #region Methods: private

        //Взаимодействие с клиентом через этот ивент
        private void OnMessageRecieved(SyncSession session, string value)
        {
            Task.Run(() => OperateRequestAsync(session, value));
        }      

        private async void OperateRequestAsync(SyncSession session, string value)
        {
            try
            {
                Log.Debug($"Session: {session.SessionID}, requested command: {value}");
                JobOperatorResponce responce = await SynchronizationTask.FromJson(value).OperateTaskRequest();
                //TODO: make a response object
                session.Send(responce.Description);
            }
            catch (Exception exc)
            {
                Log.Debug($"Session: {session.SessionID}, incorrect request: {value}");
                session.Send($"Incorrect command: {value}");
            }
        }
        #endregion
    }
}
