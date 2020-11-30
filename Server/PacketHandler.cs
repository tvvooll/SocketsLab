using Protocol;
using System;
using System.Net.Sockets;

namespace Server
{
    class ServerSocketPacketHandler : IPacketHandler
    {
        public GameManager GameManager { get; set; }
        public bool IsInGame => GameManager.Current is { };

        public ServerSocketPacketHandler()
        {
            GameManager = new GameManager();
        }

        public void HandlePacket(Packet packet, Socket socket, object parameter)
        {
            if (packet is null || socket is null) return;

            switch(packet.Type)
            {
                case PacketType.ClientStartsNewGame:
                    HandleClientStartsNewGame(socket);
                    break;

                case PacketType.ClientSuggestion:
                    HandleClientSuggestion(socket, packet.Content);
                    break;

                case PacketType.ClientGiveUp:
                    HandleClientGiveUp(socket);
                    break;

                case PacketType.ClientTerminatesSession:
                    HandleClientTerminatesSession(socket);
                    break;
            }
        }

        /// <summary>
        /// Обработчик запроса клиента на начало.
        /// </summary>
        /// <param name="socket"></param>
        private void HandleClientStartsNewGame(Socket socket)
        {
            GameManager.StartGame();
            var answer = new Packet(PacketType.ServerStartedNewGame, null);

            socket.Send(answer.ToBytes());
        }

        
        private void HandleClientSuggestion(Socket socket, object suggestion)
        {
            var number = Convert.ToInt32(suggestion);
            var attempt = GameManager.TryCheck(number);
            var response = new ServerResponse
            {
                RightDigits = attempt.RightDigits,
                DigitsInPlaces = attempt.DigitsInPlaces
            };

            var answer = new Packet(PacketType.ServerResponse, response);

            socket.Send(answer.ToBytes());
        }

        private void HandleClientGiveUp(Socket socket)
        {
            var result = GameManager.Concede();

            var answer = new Packet(PacketType.ServerShowsNumber, result);
            socket.Send(answer.ToBytes());
        }

        private void HandleClientTerminatesSession(Socket socket)
        {
            GameManager.TerminateSession();
            socket.Close();
        }
    }
}
