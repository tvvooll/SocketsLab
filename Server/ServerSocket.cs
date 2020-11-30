using Protocol;
using System;
using System.Net;
using System.Net.Sockets;

namespace Server
{
    class ServerSocket
    {

        #region Cвойства

        /// <summary>
        /// Сокет, привязанный к текущей конечной точке сети, для принятия клиентов ожидающих подключения.
        /// </summary>
        public Socket Socket { get; set; }

        /// <summary>
        /// Буфер в который будут помещатся данные принятые от клиента.
        /// </summary>
        public byte[] Buffer { get; set; }

        /// <summary>
        /// Сокет, созданный на основе подлючения клиента, который является сущностью для комуникации с данным клиентом.
        /// </summary>
        public Socket Client { get; set; }

        /// <summary>
        /// Обработчик пакетов.
        /// </summary>
        public IPacketHandler Handler { get; set; }

        /// <summary>
        /// Показывает подключён ли сейчас клиент.
        /// </summary>
        public bool IsConnected => Client.Connected;

        #endregion

        #region Конструктор

        /// <summary>
        /// Стандартный конструктор, инициализирующий <see cref="ServerSocket"/> для работы по протоколу TCP, c адресами IP версии 4.
        /// </summary>
        public ServerSocket()
        {
            //Наш сокет будет работать с адресами IP версии 4.
            //Он будет типа Stream, что есть стандартом для протокола TCP, с которым мы и будем работать.
            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            Handler = new ServerSocketPacketHandler();
        }

        #endregion

        #region Призявывание

        /// <summary>
        /// Привязывает сокет к конечной точке сети, определяющейся IP адресом и портом.
        /// </summary>
        /// <param name="address">IP адрес.</param>
        /// <param name="port">Порт.</param>
        public void Bind(string address, int port)
        {
            //Создаём конечную точку сети к которой и будем осуществлять привязку.
            var endPoint = new IPEndPoint(IPAddress.Parse(address), port);

            //Привязываемся...
            Socket.Bind(endPoint);
        }

        #endregion

        #region Прослушивание

        /// <summary>
        /// Переводит сокет в режим прослушивания, то есть режим ожидания клиентов.
        /// </summary>
        /// <param name="backlog">Максимальная длинна очереди клиентов, ожидающих подключение.</param>
        public void Listen(int backlog)
        {
            //Переводим сокет в режим прослушивания.
            Socket.Listen(backlog);
        }

        #endregion

        #region Приём и отключение клиентов

        /// <summary>
        /// Принимает клиента.
        /// </summary>
        public void Accept()
        {
            //Принимаем клиента и сохраняем сокет для комуникации с ним с свойство Client
            Client = Socket.Accept();
        }

        /// <summary>
        /// Отключение текущего клиента.
        /// </summary>
        public void Disconnect()
        {
            //Закрываем соединение с клиентом.
            Client.Close();
        }

        #endregion

        #region Приём данных от клиента и ответ на них

        /// <summary>
        /// Получает и обрабатывает пакет.
        /// </summary>
        public void Receive()
        {
            //Если клиент не подключён, то даже не пытаемся получить данные от него.
            if (!IsConnected) return;

            //Создаём буфер достаточного размера чтобы принять все данные от клиента.
            int size = Client.ReceiveBufferSize;
            Buffer = new byte[size];
            SocketError e;
            //Получаем данные и копируем их из буфера в новый массив который и приводим к типу Packet.
            int received = Client.Receive(Buffer, SocketFlags.None, out e);

            if (e != SocketError.Success) return;

            var data = new byte[received];
            Array.Copy(Buffer, data, received);
            Packet packet = null;
            try
            {
                //Здесь может быть исключение сериализации если принятые данные имеют неправильный формат.
                packet = new Packet(data);
            }
            catch { }

            if (packet is { })
                //Обрабатываем пакет если в нём что то содержится. 
                Handler.HandlePacket(packet, Client, null);
        }

        #endregion

    }
}
