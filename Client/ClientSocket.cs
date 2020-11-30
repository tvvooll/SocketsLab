using Protocol;
using Server;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client
{
    class ClientSocket
    {

        #region Свойства

        /// <summary>
        /// Буфер для приёма данных от сервера.
        /// </summary>
        public byte[] Buffer { get; set; }

        /// <summary>
        /// Сокет для комуникации с сервером.
        /// </summary>
        public Socket Socket { get; set; }

        #endregion

        #region Конструктор

        /// <summary>
        /// Инициализирует клиентский сокет.
        /// </summary>
        public ClientSocket()
        {
            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        #endregion

        #region Подключение

        /// <summary>
        /// Подключается к указанному IP-адресу и порту.
        /// </summary>
        /// <param name="address">IP-адрес.</param>
        /// <param name="port">Порт.</param>
        public void Connect(string address, int port)
        {
            //Создаём конечную точку сети к которой хотим подключится.
            var endPoint = new IPEndPoint(IPAddress.Parse(address), port);

            //Подключаемся...
            Socket.Connect(endPoint);
        }

        #endregion

        #region Приём данных от сервера и ответ на них

        public Packet Receive()
        {
            int size = Socket.ReceiveBufferSize;
            Buffer = new byte[size];

            int received = Socket.Receive(Buffer);
            var data = new byte[received];
            Array.Copy(Buffer, data, received);
            Packet packet = null;
            try
            {
                packet = new Packet(data);
            }
            catch { }

            return packet;
        }

        public void Send(Packet packet)
        {
            Socket.Send(packet.ToBytes());
        }

        #endregion

    }
}
