using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace ClientServerTest {

    class Server : TcpListener, IDisposable {

        Thread workerThread;
        ServerWorker worker;
        public delegate void OnServerNeedToLogEvent (string logText);
        public static event OnServerNeedToLogEvent OnServerNeedToLog;

        public Server (IPAddress localAddrr, int port) : base (localAddrr, port) { }

        public void Dispose () {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose (bool disposing) {
            if (disposing) {
                if (this.Active) {
                    this.Stop();
                }
            }
        }

        public void ServerStart () {
            logServer("Starting server...");
            this.Start();
            worker = new ServerWorker(this);
            workerThread = new Thread(worker.DoWork);
            workerThread.Start();
            logServer("Server started at " + Server.LocalEndPoint.ToString().Split(':')[1] + " port!");
        }

        public void ServerStop () {
            if (workerThread.IsAlive) {
                this.Stop();
                worker.RequestStop();
                workerThread.Join();
            }
        }

        internal static void logServer (string text) {
            OnServerNeedToLog?.Invoke(text);
        }

        class ServerWorker {

            private volatile bool shouldStop;
            private Server server;

            public ServerWorker (Server _server) {
                server = _server;
            }

            public void DoWork () {
                while (!shouldStop) {
                    try {
                        TcpClient client =  server.Active ? server.AcceptTcpClient() : null;
                        if (client != null) {
                            logServer("New client: " + client.Client.RemoteEndPoint.ToString());
                            new Client(client);
                        }
                    } catch (Exception) { }
                }
                logServer("Server stoped");
            }

            public void RequestStop () {
                shouldStop = true;
                logServer("Stoping server...");
            }

        }

    }

}
