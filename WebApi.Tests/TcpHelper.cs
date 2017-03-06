using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WebApi.Tests
{
    public class TcpHelper
    {
        public static string SendRequest(string request)
        {
            IPEndPoint hostEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 80);
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            
            socket.Connect(hostEP);
            socket.Send(Encoding.UTF8.GetBytes(request));
            byte[] buffer = new byte[10];

            StringBuilder page = new StringBuilder();

            int bytesReceivedCount = 0;
            do
            {
                bytesReceivedCount = socket.Receive(buffer, buffer.Length, 0);
                page.Append(Encoding.ASCII.GetString(buffer, 0, bytesReceivedCount));
            }
            while (socket.Available > 0);

            socket.Close();
            
            return page.ToString();
        }

    }
}
