using System;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            ServerSocket server = new ServerSocket();

            server.Bind("127.0.0.1", 1043);
            server.Listen(1);
            server.Accept();
            while (server.IsConnected)
            {
                server.Receive();
            }    
        }
    }
}
