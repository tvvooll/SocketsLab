using Protocol;
using System.Net.Sockets;

namespace Server
{
    /// <summary>
    /// Представляет интерфейс, общий для всех обработчиков пакетов.
    /// </summary>
    public interface IPacketHandler
    {
        /// <summary>
        /// Обрабатывает пакет.
        /// </summary>
        /// <param name="packet">Пакет.</param>
        /// <param name="socket">Отправитель/получатель.</param>
        /// <param name="parameter">Дополнительный параметр, для построения специфических обработчиков.</param>
        void HandlePacket(Packet packet, Socket socket, object parameter);
    }
}