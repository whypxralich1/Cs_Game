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

        private const int DefaultShieldDuration = 5000;

        private bool _isRunning = true;
        private string _exitMessage = "Конец";
        private Map _gameMap;
        private Player _player;
        private IEntity _activePlayer;
        private int _shieldTimer = 0;
        private Stopwatch _gameTimer = new Stopwatch();
        
        private CombatFacade _combat = new CombatFacade();
        private ConsoleRenderer _renderer = new ConsoleRenderer();
        private InputHandler _input = new InputHandler();

        private GameManager()
        {
            _gameMap = new Map(30, 10);
            _player = new Player { Name = "Hero", X = 15, Y = 7 };
            _activePlayer = _player;
            _gameMap.Entities.Add(_player);
        }

        public void Run()
        {
            InitializeConsole();
            
            _gameMap.Entities.Add(new Ork { X = 5, Y = 3 });
            _gameMap.Entities.Add(new Slime { X = 20, Y = 3 });

            _gameTimer.Start();
            long lastTime = _gameTimer.ElapsedMilliseconds;

            while (_isRunning)
            {
                long currentTime = _gameTimer.ElapsedMilliseconds;
                int deltaTime = (int)(currentTime - lastTime);
                lastTime = currentTime;

                HandleInput();
                UpdateEffects(deltaTime);
                CheckCollisions();

                if (_player.IsDead)
                {
                    _exitMessage = "ГЕРОЙ ПОГИБ!";
                    _isRunning = false;
                }

                _renderer.Render(_gameMap, _activePlayer, _player, _shieldTimer);
                System.Threading.Thread.Sleep(10);
            }

            _renderer.ShowGameOver(_exitMessage);
        }

        private void InitializeConsole()
        {
            Console.Clear();
            Console.CursorVisible = false;
        }

        private void HandleInput()
        {
            var (dx, dy, escape) = _input.GetMovement();
            
            if (escape)
            {
                _isRunning = false;
                return;
            }

            if (dx == 0 && dy == 0) return;

            int nextX = _player.X + dx;
            int nextY = _player.Y + dy;

            if (IsWithinMapBoundaries(nextX, nextY))
            {
                _player.X = nextX;
                _player.Y = nextY;
            }
        }

        private bool IsWithinMapBoundaries(int x, int y)
        {
            return x > 0 && x < _gameMap.Width - 1 && y > 0 && y < _gameMap.Height - 1;
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

            foreach (var enemy in _gameMap.Entities.OfType<Enemy>())
            {
                enemy.UpdateCooldown(deltaTime);
            }
        }

        private void CheckCollisions()
        {
            HandleItemPickups();
            HandleCombat();
        }

        private void HandleItemPickups()
        {
            if (_gameMap.IsShieldSpawned && IsPlayerOnItem(_gameMap.ShieldX, _gameMap.ShieldY))
            {
                _activePlayer = new ShieldDecorator(_player);
                _shieldTimer = DefaultShieldDuration;
                _gameMap.IsShieldSpawned = false;
            }

            if (_gameMap.IsSwordSpawned && IsPlayerOnItem(_gameMap.SwordX, _gameMap.SwordY))
            {
                _activePlayer = new SwordDecorator(_activePlayer);
                _gameMap.IsSwordSpawned = false;
            }
        }

        private bool IsPlayerOnItem(int itemX, int itemY)
        {
            return _player.X == itemX && _player.Y == itemY;
        }

        private void HandleCombat()
        {
            foreach (var enemy in _gameMap.Entities.OfType<Enemy>().ToList())
            {
                if (IsAdjacentToPlayer(enemy))
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

        private bool IsAdjacentToPlayer(Entity entity)
        {
            return Math.Abs(entity.X - _player.X) <= 1 && Math.Abs(entity.Y - _player.Y) <= 1;
        }
    }
}