using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    class GameManager
    {
        public List<Game> Games { get; set; }
        public Game Current { get; set; }

        public GameManager()
        {
            Games = new List<Game>();
            Current = null;
        }

        public void TerminateSession()
        {
            FinishGame();
        }

        public void StartGame()
        {
            if (Current is { })
                FinishGame();

            Current = new Game();
        }

        public void FinishGame()
        {
            Games.Add(Current);
            Current = null;
        }

        #region Интерфейс игры

        /// <inheritdoc cref="Game.TryCheck(int)"/>
        public GameAttempt TryCheck(int number)
        {
            var result = Current.TryCheck(number);

            if (result.IsSuccessful)
                FinishGame();

            return result;
        }

        /// <inheritdoc cref="Game.Concede"/>
        public int Concede()
        {
            var result = Current.Concede();
            FinishGame();

            return result;
        }

        #endregion
    }
}
