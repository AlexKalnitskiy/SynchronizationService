using OracleProcedureManager;
using OraDBSyncService.Scheduler;
using Quartz;
using Quartz.Impl.Matchers;
using Serilog;
using SuperSocket.SocketBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OraDBSyncService.WebServer
{
    public class CommonRequestRouter
    {
        MainScheduler Scheduler;
        SocketServer WebServer;
        SyncJobListener JobListener;

        public CommonRequestRouter()
        {
            //Init
            Scheduler = MainScheduler.GetMainScheduler();
            JobListener = new SyncJobListener();
            WebServer = SocketServer.GetDefaultServer();
            Log.Debug("Router Initialized");

            //Adding listeners
            Scheduler.Scheduler.ListenerManager.AddJobListener(JobListener, EverythingMatcher<JobKey>.AllJobs());
            WebServer.NewMessageReceived += OnMessageRecieved;
            Log.Debug("Router listeners Initialized");
        }

        #region Methods: private

        //Взаимодействие с клиентом через этот ивент
        private async void OnMessageRecieved(SyncSession session, string value)
        {
            try
            {
                Log.Debug($"Session: {session.SessionID}, requested command: {value}");
                bool success = await SynchronizationTask.FromJson(value).OperateTaskRequest();

                //TODO: make a response object
                session.Send("Response");
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
