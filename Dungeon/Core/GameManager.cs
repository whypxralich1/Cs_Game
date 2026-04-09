using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using Dungeon.World;
using Dungeon.Entities;
using Dungeon.Decorators;

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
        
        private IEntity _activePlayer; 
        private int _shieldTimer = 0;
        private Stopwatch _gameTimer = new Stopwatch();

        private CombatFacade _combat = new CombatFacade();

        private GameManager()
        {
            _gameMap = new Map(30, 10);
            _player = new Player { Name = "Hero", X = 15, Y = 7 };
            _activePlayer = _player;
            _gameMap.Entities.Add(_player);
        }

        public void Run()
        {
            Console.Clear();
            Console.CursorVisible = false;

            _gameMap.Entities.Add(new Ork { X = 5, Y = 3 });
            _gameMap.Entities.Add(new Slime { X = 20, Y = 3 });

            _gameTimer.Start();
            long lastTime = _gameTimer.ElapsedMilliseconds;

            while (_isRunning)
            {
                long currentTime = _gameTimer.ElapsedMilliseconds;
                int deltaTime = (int)(currentTime - lastTime);
                lastTime = currentTime;

                while (Console.KeyAvailable)
                {
                    HandleInput();
                }

                UpdateEffects(deltaTime);
                CheckCollisions();

                if (_player.IsDead)
                {
                    _exitMessage = "ГЕРОЙ ПОГИБ!";
                    _isRunning = false;
                }

                Render();
                System.Threading.Thread.Sleep(10); 
            }

            ShowGameOver();
        }

        private void UpdateEffects(int deltaTime)
        {
            if (_shieldTimer > 0)
            {
                _shieldTimer -= deltaTime;
                if (_shieldTimer <= 0)
                {
                    _shieldTimer = 0;
                    _activePlayer = _player; 
                }
            }

            foreach (var entity in _gameMap.Entities)
            {
                if (entity is Enemy enemy)
                {
                    enemy.UpdateCooldown(deltaTime);
                }
            }
        }

        private void CheckCollisions()
        {
            if (_gameMap.IsShieldSpawned && _player.X == _gameMap.ShieldX && _player.Y == _gameMap.ShieldY)
            {
                _activePlayer = new ShieldDecorator(_player);
                _shieldTimer = 5000; 
                _gameMap.IsShieldSpawned = false;
            }

            if (_gameMap.IsSwordSpawned && _player.X == _gameMap.SwordX && _player.Y == _gameMap.SwordY)
            {
                _activePlayer = new SwordDecorator(_activePlayer);
                _gameMap.IsSwordSpawned = false;
            }

            foreach (var entity in _gameMap.Entities.ToList())
            {
                if (entity is Enemy enemy)
                {
                    if (Math.Abs(enemy.X - _player.X) <= 1 && Math.Abs(enemy.Y - _player.Y) <= 1)
                    {
                        if (enemy.CanAttack)
                        {
                            _combat.ResolveCombat(_activePlayer, _player, enemy, () => {
                                _activePlayer = (_shieldTimer > 0) ? new ShieldDecorator(_player) : _player;
                            });

                            enemy.ResetCooldown();
                        }

                        if (enemy.HealthPoints.IsDead)
                        {
                            _gameMap.Entities.Remove(enemy);
                        }
                    }
                }
            }
        }

        private void Render()
        {
            Console.SetCursorPosition(0, 0);
            Console.Write(_gameMap.GetView());

            string statusLine = $"[ СТАТУС ]: {_activePlayer.Name} | HP: {_player.Health}";
            Console.WriteLine(statusLine.PadRight(60));
            
            if (_shieldTimer > 0) 
                Console.WriteLine($"[ ЩИТ ]: {(_shieldTimer / 1000.0):F1} сек.".PadRight(60));
            else 
                Console.WriteLine("".PadRight(60)); 
        }

        private void HandleInput()
        {
            var key = Console.ReadKey(true).Key;
            int nextX = _player.X, nextY = _player.Y;

            if (key == ConsoleKey.W) nextY--;
            else if (key == ConsoleKey.S) nextY++;
            else if (key == ConsoleKey.A) nextX--;
            else if (key == ConsoleKey.D) nextX++;
            else if (key == ConsoleKey.Escape) _isRunning = false;

            if (nextX > 0 && nextX < _gameMap.Width - 1 && nextY > 0 && nextY < _gameMap.Height - 1)
            {
                _player.X = nextX;
                _player.Y = nextY;
            }
        }

        private void ShowGameOver()
        {
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
    }
}