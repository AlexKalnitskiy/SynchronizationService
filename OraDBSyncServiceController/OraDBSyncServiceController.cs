using System;
using System.Threading;
using WebSocket4Net;

namespace OraDBSyncServiceController
{
    class OraDBSyncServiceController
    {
        static void Main(string[] args)
        {
            ManualMode();
            //JSONTest();
        }
        static void ManualMode()
        {
            Console.WriteLine("Started");
            WebSocket socket = new WebSocket("ws://127.0.0.1:2019/");
            socket.MessageReceived += Socket_MessageReceived;
            socket.Opened += Socket_Opened;
            socket.Open();
            while (socket.State == WebSocketState.Connecting)
            {
                //Console.WriteLine(socket.State.ToString());
                //System.Threading.Thread.Sleep(500);
            }
            Console.WriteLine(socket.State.ToString());
            while (socket.State == WebSocketState.Open)
            {
                Console.WriteLine("Ready to send");
                socket.Send(Console.ReadLine());
                Console.WriteLine(socket.State.ToString());
            }
            Console.WriteLine("Sorry " + socket.State.ToString());
            Console.ReadKey();
        }

        private static void Socket_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            Console.WriteLine(e.Message);
        }

        private static void Socket_Opened(object sender, EventArgs e)
        {
            Console.WriteLine("OPEN!");
        }
    }
}
