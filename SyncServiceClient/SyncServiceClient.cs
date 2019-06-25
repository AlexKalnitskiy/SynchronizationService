using OracleProcedureManager;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using WebSocket4Net;
using XTensionProject;

namespace SyncServiceClient
{
    public class ServiceClient
    {
        #region Events
        public delegate void MessageRecievedEventHandler(string message);
        public event MessageRecievedEventHandler OnMessageReceived;
        #endregion
        #region Fields
        private WebSocket _socket;
        #endregion
        #region Constructors
        public ServiceClient(string uri = "ws://127.0.0.1:2019/") //TODO: Move to Constants WS Connection
        {
            _socket = new WebSocket(uri);
            _socket.MessageReceived += Client_MessageReceived;
            _socket.Open();
            TimeoutExt.WaitForCondition(() => _socket.State == WebSocketState.Open);
            if (_socket.State != WebSocketState.Open) throw new Exception("ConnectionTimeout");
        }
        #endregion
        #region Methods: public
        public void ServiceRequest(SynchronizationTask task, RouterCommands routerCommand, int timeout = 2500)
        {
            task.RouterCommand = routerCommand.ToString();
            _socket.Send(task.ToJson());
        }
        #endregion
        #region Methods: private
        private void Client_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            OnMessageReceived(e.Message);
        }
        #endregion
    }
}
