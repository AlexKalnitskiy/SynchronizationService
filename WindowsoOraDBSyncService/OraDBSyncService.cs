using System;
using System.ServiceProcess;
using System.Threading.Tasks;
using OraDBSyncService.WebServer;
using Serilog;

namespace OraDBSyncService
{
    public partial class OraDBSyncService : ServiceBase
    {
        public CommonRequestRouter Router;

        public OraDBSyncService()
        {
            InitializeComponent();
            this.CanStop = true;
            this.CanPauseAndContinue = false;
            this.AutoLog = true;
        }

        protected override void OnStart(string[] args)
        {
            //Logger initialization
            Logging.SerilogInit.InitLog();
            try
            {
                SocketServer server = SocketServer.GetDefaultServer();
                Router = CommonRequestRouter.GetRouter();
                bool serverStartedFlag = Task.Factory.StartNew(server.Start, TaskCreationOptions.LongRunning).Result;
                Log.Information("Oracle sync service started succesfully");
            }
            catch (Exception exc)
            {
                Log.Fatal(exc, "Server initialization crashed!");
                Stop();
            }
        }
        protected override void OnStop()
        {
            Router.DiscardAllStates();
        }
    }
}

