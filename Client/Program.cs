using Protocol;
using System;
using System.Linq;
using System.Threading;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            ClientSocket socket = new ClientSocket();
            socket.Connect("127.0.0.1", 1043);
            GetInfo();
            Command command = Command.None;

            while(true)
            {
                switch(command)
                {
                    case Command.None:           { HandleNone(ref command);                   } break;
                    case Command.NewGame:        { HandleNewGame(ref command, socket);        } break;
                    case Command.Quit:           { HandleQuit(ref command, socket);           } break;
                    case Command.ChooseMethod:   { HandleChooseMethod(ref command);           } break;
                    case Command.InputMethod:    { HandleInputMethod(ref command, socket);    } break;
                    case Command.GenerateMethod: { HandleGenerateMethod(ref command, socket); } break;
                    case Command.Concede:        { HandleConcede(ref command, socket);        } break;
                }
            }
        }

        private static void HandleConcede(ref Command command, ClientSocket socket)
        {
            var request = new Packet(PacketType.ClientGiveUp, null);
            socket.Send(request);

            Packet response = null;
            do
            {
                response = socket.Receive();
                if (response is { Type: PacketType.ServerShowsNumber })
                    break;
            } while (true);

            Console.WriteLine($"Вы проиграли, а число было {response.Content}.");
            command = Command.None;
        }

        public static void HandleGenerateMethod(ref Command command, ClientSocket socket)
        {
            Console.Write("Введите количество чисел которое хотите сгенерировать(от 1 до 100): ");

            bool isFail = false;
            string answer;

            do
            {
                if (isFail)
                    Console.Write("Напишите, пожалуйста число от 1 до 99: ");
                answer = Console.ReadLine();
                isFail = answer.Any(c => !char.IsDigit(c)) || answer.Length > 2;
            }
            while (isFail);

            for (int i = 0; i < Convert.ToInt32(answer); ++i)
            {
                Console.WriteLine($"\n****************** Попытка #{i + 1} ******************");
                var attempt = new Packet(PacketType.ClientSuggestion, GenerateNumber());
                Console.WriteLine($"Наше число: {attempt.Content}");
                socket.Send(attempt);

                Packet response = null;
                do
                {
                    response = socket.Receive();
                    if (response is { Type: PacketType.ServerResponse })
                        break;
                } while (true);

                var attemptResult = (ServerResponse)response.Content;

                if (attemptResult.IsSuccessful)
                {
                    Console.WriteLine("Да, вы выиграли!!!");
                    command = Command.None;
                    break;
                }
                else
                {
                    Console.WriteLine($"\nПравильных цифр: {attemptResult.RightDigits}");
                    Console.WriteLine($"Цифр на правильных позициях: {attemptResult.DigitsInPlaces}\n");
                    command = Command.ChooseMethod;
                }
                Thread.Sleep(1000);
            }
        }
        public static void GetInfo()
        {
            Console.WriteLine("Лабораторная робота No.2.\n");
            Console.WriteLine("Вариант: 18.");
            Console.WriteLine("Исполнитель: Волчаренко Тамара.\n");
            Console.WriteLine("Это игра в которой нужно угадать 4х значное число, в ответ на каждую попытку угадать игрок получает два числа:");
            Console.WriteLine("1) Количество угаданных цифр;");
            Console.WriteLine("2) Количество угаданных цифр с учётом их позиции в числе.\n");
            Console.WriteLine("Если хотите сдаться, нажмите клавишу \"с\".\n");
        }

        public static void PrintServerResponse(ServerResponse response)
        {
            if(response.IsSuccessful)
                Console.WriteLine("Да, вы выиграли!!!");
            else
            {
                Console.WriteLine($"\nПравильных цифр: {response.RightDigits}");
                Console.WriteLine($"Цифр на правильных позициях: {response.DigitsInPlaces}\n");
            }
        }

        public static int GenerateNumber()
        {
            Random random = new Random();
            return random.Next(1000, 9999);
        }

        public static void HandleNone(ref Command command)
        {
            Console.WriteLine("Выберите действие: ");
            Console.WriteLine("    1) Начать новую игру.");
            Console.WriteLine("    2) Завершить сеанс.");
            Console.Write("\nВаш ответ (1 или 2): ");
            bool isFail = false;
            string answer;

            do
            {
                if(isFail)
                    Console.Write("Ответьте, пожалуйста, \"1\" или \"2\": ");
                answer = Console.ReadLine();
                isFail = !(answer == "1" || answer == "2");
            }
            while (isFail);

            switch (answer)
            {
                case "1":
                    command = Command.NewGame;
                    break;
                case "2":
                    command = Command.Quit;
                    break;
            }
        }

        public static void HandleNewGame(ref Command command, ClientSocket socket)
        {
            Console.WriteLine("Начинаем новую игру!");
            var request = new Packet(PacketType.ClientStartsNewGame, null);
            socket.Send(request);
            Packet response = null;
            do
            {
                response = socket.Receive();
                if (response is { Type: PacketType.ServerStartedNewGame })
                    break;
            } while (true);

            Console.WriteLine("Игра началась!");
            command = Command.ChooseMethod;
        }

        public static void HandleQuit(ref Command command, ClientSocket socket)
        {
            var request = new Packet(PacketType.ClientTerminatesSession, null);
            socket.Send(request);
            Environment.Exit(0);
        }

        public static void HandleInputMethod(ref Command command, ClientSocket socket)
        {
            Console.Write("Введите 4х значное число: ");
            string number = "0";
            int n = 1001; 
            do
            {
                if (!(n >= 1000 && n <= 9999) || number.Any(c => !char.IsDigit(c)))
                    Console.Write("Это не 4х значное число, попробуйте снова: ");
                number = Console.ReadLine();
                if (number.All(c => char.IsDigit(c)))
                    n = Convert.ToInt32(number);
                else if(number == "с")
                {
                    command = Command.Concede;
                    return;
                }
                
            } while (!( n >= 1000 && n <= 9999) || number.Any(c => !char.IsDigit(c)));

            var suggestion = new Packet(PacketType.ClientSuggestion, n);
            socket.Send(suggestion);

            Packet response = null;
            do
            {
                response = socket.Receive();
                if (response is { Type: PacketType.ServerResponse })
                    break;
            } while (true);

            var attempt = (ServerResponse)response.Content;

            if (attempt.IsSuccessful)
            {
                Console.WriteLine("Да, вы выиграли!!!");
                command = Command.None;
            }
            else
            {
                Console.WriteLine($"\nПравильных цифр: {attempt.RightDigits}");
                Console.WriteLine($"Цифр на правильных позициях: {attempt.DigitsInPlaces}\n");
                command = Command.InputMethod;
            }
            
        }

        

        public static void HandleChooseMethod(ref Command command)
        {
            Console.WriteLine("Выберите способ игры:");
            Console.WriteLine("    1) Самому ввести число.");
            Console.WriteLine("    2) Сгенерировать n чисел автоматически (n Вы задаёте.)");
            Console.Write("\nВаш ответ (1 или 2): ");
            bool isFail = false;
            string answer;

            do
            {
                if (isFail)
                    Console.Write("Ответьте, пожалуйста, \"1\" или \"2\": ");
                answer = Console.ReadLine();
                if(answer == "с")
                {
                    command = Command.Concede;
                    return;
                }    
                isFail = !(answer == "1" || answer == "2");
            }
            while (isFail);

            switch (answer)
            {
                case "1":
                    command = Command.InputMethod;
                    break;
                case "2":
                    command = Command.GenerateMethod;
                    break;
            }
        }

        public enum Command
        {
            None,

            NewGame,
            Quit,


            ChooseMethod,

            InputMethod,
            GenerateMethod,
            Concede
        }
    }
}
