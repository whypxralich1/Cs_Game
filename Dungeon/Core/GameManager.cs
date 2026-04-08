using System;
using System.Threading;
using System.Collections.Generic;
using Dungeon.World;
using Dungeon.Entities;

namespace Dungeon.Core
{
    public class GameManager
    {
        private static GameManager? _instance;
        public static GameManager Instance => _instance ??= new GameManager();

        private bool _isRunning = true;
        private string _exitMessage = "Конец";
        private Map _gameMap;
        private Player _player;

        private GameManager()
        {
            _gameMap = new Map(30, 10);
            _player = new Player { Name = "Hero", X = 15, Y = 7 }; 
            _gameMap.Entities.Add(_player);
        }

        public void Run()
        {
            Console.Clear();
            Console.CursorVisible = false;

            Ork protoOrk = new Ork();
            Slime protoSlime = new Slime();

            Ork ork1 = (Ork)protoOrk.Clone();
            ork1.X = 5; 
            ork1.Y = 3;
            _gameMap.Entities.Add(ork1);

            Slime slime1 = (Slime)protoSlime.Clone();
            slime1.X = 20; 
            slime1.Y = 3;
            _gameMap.Entities.Add(slime1);

            while (_isRunning)
            {
                if (Console.KeyAvailable)
                {
                    HandleInput();
                }

                CheckCollisions();

                if (_player.IsDead)
                {
                    _exitMessage = "ГЕРОЙ ПОГИБ!";
                    _isRunning = false;
                }

                Render();
                Thread.Sleep(50);
            }

            Console.Clear();
            Console.SetCursorPosition(0, 0);
            Console.WriteLine("######################################");
            Console.WriteLine($"# {Padding(_exitMessage, 34)} #");
            Console.WriteLine("######################################");
            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey();
        }

        private string Padding(string text, int length)
        {
            if (text.Length >= length) return text;
            int left = (length - text.Length) / 2;
            return text.PadLeft(text.Length + left).PadRight(length);
        }

        private void HandleInput()
        {
            var key = Console.ReadKey(true).Key;
            int nextX = _player.X;
            int nextY = _player.Y;

            if (key == ConsoleKey.W) nextY--;
            else if (key == ConsoleKey.S) nextY++;
            else if (key == ConsoleKey.A) nextX--;
            else if (key == ConsoleKey.D) nextX++;
            else if (key == ConsoleKey.Escape) 
            {
                _exitMessage = "Конец";
                _isRunning = false;
            }

            if (nextX > 0 && nextX < _gameMap.Width - 1 && 
                nextY > 0 && nextY < _gameMap.Height - 1)
            {
                _player.X = nextX;
                _player.Y = nextY;
            }
        }

        private void CheckCollisions()
        {
            foreach (var entity in _gameMap.Entities)
            {
                if (entity is Enemy enemy)
                {
                    int deltaX = Math.Abs(enemy.X - _player.X);
                    int deltaY = Math.Abs(enemy.Y - _player.Y);

                    if (deltaX <= 1 && deltaY <= 1)
                    {
                        _player.TakeDamage(enemy.Damage);
                    }
                }
            }
        }

        private void Render()
        {
            Console.SetCursorPosition(0, 0);
            Console.Write(_gameMap.GetView());
            Console.WriteLine($"[ СТАТУС ]: X:{_player.X} Y:{_player.Y}");
        }
    }
}