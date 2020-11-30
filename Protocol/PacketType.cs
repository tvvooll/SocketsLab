namespace Protocol
{
    /// <summary>
    /// Все возможные типы пакетов, которые распознают сервер и клиент
    /// </summary>
    public enum PacketType
    {

        #region Пакеты, которые принимает сервер

        /// <summary>
        /// Попытка клиента угадать число.
        /// </summary>
        /// <remarks>В <see cref="Packet.Content"/> хранится тип <see cref="int"/>.</remarks>
        ClientSuggestion,

        /// <summary>
        /// Клиент сдаётся.
        /// </summary>
        /// <remarks>В <see cref="Packet.Content"/> хранится <see cref="null"/>.</remarks>
        ClientGiveUp,

        /// <summary>
        /// Клиент начинает новую игру.
        /// </summary>
        /// <remarks>В <see cref="Packet.Content"/> хранится <see cref="null"/>.</remarks>
        ClientStartsNewGame,

        /// <summary>
        /// Клиент завершает сеанс.
        /// </summary>
        /// <remarks>В <see cref="Packet.Content"/> хранится <see cref="null"/>.</remarks>
        ClientTerminatesSession,

        #endregion

        #region Пакеты, которые принимает клиент

        /// <summary>
        /// Ответ сервера на попытку клиента угадать число.
        /// </summary>
        /// <remarks>В <see cref="Packet.Content"/> хранится тип <see cref="Protocol.ServerResponse"/>.</remarks>
        ServerResponse,

        /// <summary>
        /// Сервер показывает игроку число в случае его поражения.
        /// </summary>
        /// <remarks>В <see cref="Packet.Content"/> хранится тип <see cref="int"/>.</remarks>
        ServerShowsNumber,

        /// <summary>
        /// Сервер начал новую игру.
        /// </summary>
        /// <remarks>В <see cref="Packet.Content"/> хранится <see cref="null"/>.</remarks>
        ServerStartedNewGame

        #endregion

    }
}