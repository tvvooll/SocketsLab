using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace Protocol
{
    /// <summary>
    /// Класс представляющий нашу структуру пакета.
    /// </summary>
    [Serializable]
    public class Packet
    {
        #region Свойства

        /// <summary>
        /// Тип пакета.
        /// </summary>
        public PacketType Type { get; set; }

        /// <summary>
        /// Содержимое пакета.
        /// </summary>
        public object Content { get; set; }

        #endregion

        #region Сериализатор

        /// <summary>
        /// <see cref="BinaryFormatter"/>, необходим для того чтобы (де)сериализировать обьекты в/из массивов байтов.
        /// </summary>
        private static BinaryFormatter formatter = new BinaryFormatter();

        #endregion

        #region Конструкторы

        /// <summary>
        /// Конструктор, создаёт пакет с определённым типом и содержимым.
        /// </summary>
        /// <param name="type">Тип пакета.</param>
        /// <param name="content">Содержимое пакета.</param>
        public Packet(PacketType type, object content)
        {
            Type = type;
            Content = content;
        }

        /// <summary>
        /// Создаёт пакет на основе массива байтов, десериализируя его.
        /// </summary>
        /// <param name="array">Массив байтов.</param>
        /// <exception cref="NullReferenceException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="SerializationException"/>
        public Packet(byte[] array)
        {
            //Превращаем массив байтов в поток, затем десериализируем этот поток в Packet.
            var stream = new MemoryStream(array);
            Packet p = formatter.Deserialize(stream) as Packet;

            Type = p.Type;
            Content = p.Content;
        }

        #endregion

        #region Приведение пакета к массиву байтов

        /// <summary>
        /// Приводит этот обьект к массиву байтов, сериализируя его.
        /// </summary>
        /// <returns>Возвращает массив байтов, представляющий данный пакет.</returns>
        public byte[] ToBytes()
        {
            //Создаём поток в который будет сериализироваться пакет.
            var stream = new MemoryStream();
            formatter.Serialize(stream, this);

            //Приводим поток к массиву байтов.
            byte[] packet = stream.ToArray();
            return packet;
        }

        #endregion
    }
}
