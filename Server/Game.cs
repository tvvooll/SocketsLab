using System;
using System.Linq;

namespace Server
{
    class Game
    {
        #region Свойства

        /// <summary>
        /// Число которое нужно угадать.
        /// </summary>
        private readonly string Number;

        /// <summary>
        /// Верхняя граница числа.
        /// </summary>
        public int MaxNumber => 9999;

        /// <summary>
        /// Нижняя граница числа.
        /// </summary>
        public int MinNumber => 1000;

        /// <summary>
        /// Показывает закончена ли игра.
        /// </summary>
        public bool IsFinished { get; private set; }

        /// <summary>
        /// Отображает результат игры.
        /// </summary>
        public string GameResult { get; private set; }

        /// <summary>
        /// Количество использованых попыток.
        /// </summary>
        public int Attempts { get; private set; }

        #endregion

        #region Конструктор

        /// <summary>
        /// Стандартный конструктор, запускает игру с случайным 4-значным числом.
        /// </summary>
        public Game()
        {
            Random random = new Random();
            Number = random.Next(MinNumber, MaxNumber).ToString();
            
            IsFinished = false;
            GameResult = "Unknown";
            Attempts = 0;
        }

        #endregion

        #region Интерфейс игры

        /// <summary>
        /// Попытка игрока угадать число.
        /// </summary>
        /// <param name="number"></param>
        /// <returns>Возвращает два числа, одно - количество правильных чисел, второе - количество чисел на правильных позициях.</returns>
        public GameAttempt TryCheck(int number)
        {
            //Засчитываем пользователю попытку.
            ++Attempts;

            //Формируем ответ.
            var rightDigits = GetRight(number);
            var digitInPlaces = GetInPlaces(number);

            var result = new GameAttempt
            {
                RightDigits = rightDigits,
                DigitsInPlaces = digitInPlaces,
                IsSuccessful = false,
            };

            //Проверяем не выиграл ли игрок.
            if (rightDigits == 4 && digitInPlaces == 4)
            {
                //Игра заканчивается.
                IsFinished = true;
                //Попытка считается успешной.
                result.IsSuccessful = true;
                GameResult = $"The player won with {Attempts} attempts!";
            }

            //Возвращаем ответ.
            return result;
        }

        /// <summary>
        /// Игрок сдаётся.
        /// </summary>
        /// <returns>Возвращает искомое число.</returns>
        public int Concede()
        {
            //Игра заканчивается.
            IsFinished = true;
            GameResult = "The player lost!";

            //Показываем игроку искомое число.
            return Convert.ToInt32(Number);
        }

        #endregion

        #region Вспомогательные методы

        /// <summary>
        /// Вычисляет количество цифр которые совпали с цифрами искомого числа.
        /// </summary>
        /// <param name="number">Предполагаемое число.</param>
        /// <returns>Вовращает число от 0 до 4 или -1 если число не лежит в допустимых границах.</returns>
        private int GetRight(int number)
        {
            //Проверяем число на принадлежность нужным числовым границам.
            if (!CheckNumber(number)) return -1;

            //Приводим числа спискам, затем ищем их пересечение и возвращаем количество елементов в нём.
            var result = 0;
            var listNumber = number.ToString().ToList();
            var listRightNumber = Number.ToList();
            foreach(var c in listNumber)
            {
                foreach(var r in listRightNumber)
                {
                    if(r == c)
                    {
                        result++;
                        listRightNumber.Remove(c);
                        break;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Вычисляет сколько цифр из предполагаемого числа совпадают с цифрами искомого числа учитывая позицию.
        /// </summary>
        /// <param name="number">Предполагаемое число.</param>
        /// <returns>Вовращает число от 0 до 4 или -1 если число не лежит в допустимых границах.</returns>
        private int GetInPlaces(int number)
        {
            //Проверяем число на принадлежность нужным числовым границам.
            if (!CheckNumber(number)) return -1;

            //Приводим числа к строкам.
            var stringNumber = number.ToString();
            int result = 0;

            //Перебираем символы в полученых строках и сравниваем их, считая сколько совпало.
            for (int i = 0; i < Number.Length; ++i)
            {
                if (Number[i] == stringNumber[i])
                    ++result;
            }

            return result;
        }

        #endregion

        #region Проверка числа на корректность

        /// <summary>
        /// Проверяет находиться ли число в границах допустимого диапазона.
        /// </summary>
        /// <param name="number">Число.</param>
        /// <returns>Возвращает true если число имеет правильный формат. В ином случае false.</returns>
        private bool CheckNumber(int number)
        {
            return number >= 1000 && number <= 9999;
        }

        #endregion
    }
}
