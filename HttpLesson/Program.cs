using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace HttpLesson
{
    /// <summary>
    /// Calling IIS with HTTP GET example. TCP/IP Level.
    /// 
    /// Based on example from here:
    /// http://stackoverflow.com/questions/11862890/c-how-to-execute-a-http-request-using-sockets
    /// </summary>
    class HttpLesson
    {
        public static void Main(String[] args)
        {
            string httpRequest = @"GET /test.html HTTP/1.1
Host: testvarvara
Connection: keep-alive
Cache-Control: max-age=0
Upgrade-Insecure-Requests: 1
User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.87 Safari/537.36
Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8
Accept-Encoding: gzip, deflate, sdch
Accept-Language: en-US,en;q=0.8

";
            //// 1. Initialize socket.
            IPEndPoint hostEP = new IPEndPoint(IPAddress.Parse("127.1.1.1"), 80);
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            //// 2. Send request.
            socket.Connect(hostEP);
            socket.Send(Encoding.UTF8.GetBytes(httpRequest));
            byte[] buffer = new byte[10];

            StringBuilder page = new StringBuilder();

            //// 3. Receive response.
            int bytesReceivedCount = 0;
            do
            {
                bytesReceivedCount = socket.Receive(buffer, buffer.Length, 0);

                //// 4. Get string from received byte array.
                page.Append(Encoding.ASCII.GetString(buffer, 0, bytesReceivedCount));
            }
            while (socket.Available > 0);

            socket.Close();

            Console.WriteLine(page.ToString());
            Console.Read();
        }
    }
}