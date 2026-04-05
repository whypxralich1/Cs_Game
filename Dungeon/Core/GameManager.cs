using System;
using System.Threading;
using Dungeon.World;

namespace Dungeon.Core
{
    public class GameManager
    {
        private static GameManager? _instance;

        public static GameManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameManager();
                }
                return _instance;
            }
        }

        private GameManager()
        {
            MapWidth = 25;
            MapHeight = 15;
            _gameMap = new Map(MapWidth, MapHeight);
        }

        public int MapWidth { get; private set; }
        public int MapHeight { get; private set; }

        private bool _isRunning = true;
        private Map _gameMap;

        public void Run()
        {
            Console.Clear();
            Console.WriteLine($"Размер карты: {MapWidth}x{MapHeight}");
            Console.WriteLine("Нажмите ESC длч выхода из игры");

            while (_isRunning)
            {
                if (Console.KeyAvailable)
                {
                    if (Console.ReadKey(true).Key == ConsoleKey.Escape) 
                        _isRunning = false;
                }
                
                _gameMap.Draw();
                Thread.Sleep(50);
            }
            Console.WriteLine("Игра окончена");
        }
    }
}