using System;
using System.Threading;
using Dungeon.World;

namespace Dungeon.Core
{
    public class Game
    {
        private bool _isRunning = true;
        private Map _gameMap;

        public Game()
        {
            _gameMap = new Map(20, 10);
        }

        public void Run()
        {
            Console.Clear();
            while (_isRunning)
            {
                HandleInput();
                Update();
                Render();
                Thread.Sleep(50);
            }
        }

        private void HandleInput()
        {
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.Escape) _isRunning = false;
            }
        }

        private void Update()
        {
            
        }

        private void Render()
        {
            _gameMap.Draw();
        }
    }
}