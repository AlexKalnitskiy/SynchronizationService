using log4net;
using SuperSocket.Common;
using SuperSocket.SocketEngine;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketBase.Logging;
using SuperSocket.SocketBase.Provider;
using SuperWebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Serilog;

namespace OraDBSyncService.WebServer
{
    public class SocketServer : WebSocketServer<SyncSession>
    {
        #region Singleton

        private static SocketServer _singleton;

        public static SocketServer GetDefaultServer()
        {
            if (_singleton == null)
            {
                try
                {
                    _singleton = new SocketServer();
                    _singleton.Initialize();
                    Log.Information("Web-Socket Server installed succesfully: (Port: {0}, MaxConnectionNumber: {1}, MaxRequestLength: {2}, SendingQueueSize: {3}, SendBufferSize: {4}, SendTimeOut: {5}, ReceiveBufferSize: {6})",
                                     _singleton.Config.Port,
                                     _singleton.Config.MaxConnectionNumber,
                                     _singleton.Config.MaxRequestLength,
                                     _singleton.Config.SendingQueueSize,
                                     _singleton.Config.SendBufferSize,
                                     _singleton.Config.SendTimeOut,
                                     _singleton.Config.ReceiveBufferSize);
                }
                catch (Exception e)
                {
                    Log.Error("Error while launching Web-Socket service: {0}", e);
                    throw;
                }
            }
            return _singleton;
        }

        public void Destroy()
        {
            if (_singleton != null)
            {
                _singleton.Stop();
                _singleton = null;
            }
        }

        #endregion Singleton

        #region Constructors
        private SocketServer()
        { }
        #endregion Constructors

        #region Public Fields

        #endregion Public Fields 

        #region Methods: private

        private ServerConfig GetWSServerConfig(ServiceSettings settings)
        {
            ServerConfig config = new ServerConfig()
            {
                Port = (settings.Port != String.Empty) ? Int32.Parse(settings.Port) : 2019,
                Ip = "Any",
                MaxConnectionNumber = 100,
                MaxRequestLength = 100000,
                SendingQueueSize = 5, //orig: 5 //1000
                SendBufferSize = 2048, //orig: 2048 //65536
                SendTimeOut = 5000, //orig: 5000
                ReceiveBufferSize = 4096 //orig: 4096 //131072
            };
            //if (isSecured) { }

            return config;
        }

        private void Initialize()
        {
            //TO DO: реализовать выборку настроек из конфига
            //ServiceSettings settings = ServiceSettings.FromJson("");
            ServiceSettings settings = ServiceSettings.Get();

            ServerConfig config = GetWSServerConfig(settings);
            _singleton.Setup(
                new RootConfig { DisablePerformanceDataCollector = true },
                config, null, null, null, null, null);
            _singleton.NewSessionConnected += new SessionHandler<SyncSession>(On_WebSocketServer_SessionConnected);
            _singleton.SessionClosed += new SessionHandler<SyncSession, CloseReason>(On_WebSockerServer_SessionClosed);
        }

        protected override void OnStopped()
        {
            base.OnStopped();
            Log.Information("Web-Socket Server stopped succesfully");
        }

        private void On_WebSockerServer_SessionClosed(SyncSession session, CloseReason value)
        {
            Log.Information($"Closed Web-Session with ID:{session.SessionID}");
        }

        private void On_WebSocketServer_SessionConnected(WebSocketSession<SyncSession> session)
        {
            Log.Information($"Opened Web-Session with ID:{session.SessionID}");
        }
        #endregion Methods: private
    }
    public class SyncSession : WebSocketSession<SyncSession>
    {

    }
}
