using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace ClientServerTest {
    class Client {

        public Client (TcpClient client) {
            string responseHtml = "<html><body><h1>It works!</h1></body></html>";
            string responseString = "HTTP/1.1 200 OK\nContent-type: text/html\nContent-Length:" + responseHtml.Length.ToString() + "\n\n" + responseHtml;
            byte[] buffer = Encoding.ASCII.GetBytes(responseString);
            client.GetStream().Write(buffer, 0, buffer.Length);
            client.Close();
        }

    }
}
