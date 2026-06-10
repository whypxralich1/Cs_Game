using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Collections.Generic;
using Dungeon.Core;
using Dungeon.Entities;
using Dungeon.Decorators;
using Dungeon.Data;

namespace Dungeon.States
{
    public class GameplayState : IGameState
    {
        private readonly GameManager _gameManager;
        private readonly string _saveFilePath;
        private string _statusMessage = string.Empty;
        private int _messageTimer = 0;

        public GameplayState(GameManager gameManager)
        {
            _gameManager = gameManager;
            _saveFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "save.json");
        }

        public void HandleInput()
        {
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                char keyChar = char.ToUpper(keyInfo.KeyChar);
                bool turnMade = false;

                if (keyInfo.Key == ConsoleKey.Z || keyChar == 'Z' || keyChar == 'Я')
                {
                    _gameManager.UndoLastCommand();
                    turnMade = true;
                }
                else if (keyInfo.Key == ConsoleKey.Escape)
                {
                    _gameManager.ChangeState(new PauseState(_gameManager));
                    return;
                }
                else if (keyInfo.Key == ConsoleKey.K || keyChar == 'K' || keyChar == 'Л')
                {
                    SaveGame();
                    turnMade = true;
                }
                else if (keyInfo.Key == ConsoleKey.L || keyChar == 'L' || keyChar == 'Д')
                {
                    LoadGame();
                    turnMade = true;
                }
                else
                {
                    int dx = 0;
                    int dy = 0;

                    if (keyInfo.Key == ConsoleKey.UpArrow || keyInfo.Key == ConsoleKey.W || keyChar == 'W' || keyChar == 'Ц') dy = -1;
                    else if (keyInfo.Key == ConsoleKey.DownArrow || keyInfo.Key == ConsoleKey.S || keyChar == 'S' || keyChar == 'Ы') dy = 1;
                    else if (keyInfo.Key == ConsoleKey.LeftArrow || keyInfo.Key == ConsoleKey.A || keyChar == 'A' || keyChar == 'Ф') dx = -1;
                    else if (keyInfo.Key == ConsoleKey.RightArrow || keyInfo.Key == ConsoleKey.D || keyChar == 'D' || keyChar == 'В') dx = 1;

                    if (dx != 0 || dy != 0)
                    {
                        int nextX = _gameManager.Player.X + dx;
                        int nextY = _gameManager.Player.Y + dy;

                        if (!_gameManager.GameMap.IsWall(nextX, nextY))
                        {
                            _gameManager.ExecuteGameCommand(new Commands.MoveCommand(_gameManager.Player, dx, dy));
                            turnMade = true;
                        }
                    }
                }

                if (turnMade)
                {
                    _gameManager.HandleItemPickups();
                    _gameManager.HandleCombat();
                }
            }
        }

        public void Update(int deltaTime)
        {
            if (_messageTimer > 0)
            {
                _messageTimer -= deltaTime;
                if (_messageTimer <= 0) _statusMessage = string.Empty;
            }

            if (_gameManager.GameMap.IsHoleSpawned && _gameManager.Player.X == _gameManager.GameMap.TrapX && _gameManager.Player.Y == _gameManager.GameMap.TrapY)
            {
                _gameManager.NextLevel();
                return;
            }

            if (_gameManager.Player.IsDead)
            {
                _gameManager.ChangeState(new GameOverState(_gameManager, "ГЕРОЙ ПОГИБ!"));
            }
        }

        public void Render()
        {
            _gameManager.Renderer.Render(_gameManager.GameMap, _gameManager.ActivePlayer, _gameManager.Player, _gameManager.ShieldTimer, _gameManager.SwordUses);
            
            Console.SetCursorPosition(0, _gameManager.GameMap.Height + 4);
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            
            string holeStatus = _gameManager.GameMap.IsHoleSpawned ? "Выход открыт: X" : "Выход: Заблокирован (Убейте врагов)";
            Console.WriteLine($"[ ТЕКУЩИЙ ЯРУС ПОДЗЕМЕЛЬЯ: {_gameManager.CurrentLevel} / 5 ] | {holeStatus}".PadRight(80));
            Console.ResetColor();

            Console.SetCursorPosition(0, _gameManager.GameMap.Height + 5);
            if (!string.IsNullOrEmpty(_statusMessage))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(_statusMessage.PadRight(80));
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine("".PadRight(80));
            }

            Console.SetCursorPosition(0, _gameManager.GameMap.Height + 6);
            Console.WriteLine("[ УПРАВЛЕНИЕ ]: WASD - Ход | Z - Undo | K - Сохранить | L - Загрузить".PadRight(80));
        }

        private void SaveGame()
        {
            var saveData = new SaveData
            {
                CurrentLevel = _gameManager.CurrentLevel,
                PlayerX = _gameManager.Player.X,
                PlayerY = _gameManager.Player.Y,
                CurrentHealth = _gameManager.Player.HealthPoints.Current,
                MaxHealth = _gameManager.Player.HealthPoints.Max,
                ShieldTimer = _gameManager.ShieldTimer,
                SwordUses = _gameManager.SwordUses,
                IsShieldSpawned = _gameManager.GameMap.IsShieldSpawned,
                IsSwordSpawned = _gameManager.GameMap.IsSwordSpawned
            };

            foreach (var enemy in _gameManager.GameMap.Entities.OfType<Enemy>())
            {
                saveData.Enemies.Add(new EnemySaveData
                {
                    Type = enemy.GetType().Name,
                    X = enemy.X,
                    Y = enemy.Y,
                    CurrentHealth = enemy.HealthPoints.Current,
                    MaxHealth = enemy.HealthPoints.Max,
                    StrategyType = enemy.AttackStrategy?.GetType().Name ?? string.Empty
                });
            }

            try
            {
                string json = JsonSerializer.Serialize(saveData, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_saveFilePath, json);
                _statusMessage = "[ СИСТЕМА ]: Игра успешно сохранена!";
                _messageTimer = 2000;
            }
            catch
            {
                _statusMessage = "[ ОШИБКА ]: Не удалось сохранить игру.";
                _messageTimer = 2000;
            }
        }

        private void LoadGame()
        {
            if (!File.Exists(_saveFilePath))
            {
                _statusMessage = "[ ОШИБКА ]: Чекпоинт не найден!";
                _messageTimer = 2000;
                return;
            }

            try
            {
                string json = File.ReadAllText(_saveFilePath);
                var saveData = JsonSerializer.Deserialize<SaveData>(json);
                if (saveData == null) return;

                _gameManager.Player.X = saveData.PlayerX;
                _gameManager.Player.Y = saveData.PlayerY;
                _gameManager.Player.HealthPoints.InitHealth(saveData.CurrentHealth, saveData.MaxHealth);
                _gameManager.ShieldTimer = saveData.ShieldTimer;
                _gameManager.SwordUses = saveData.SwordUses;

                _gameManager.SetupLoadedMap(saveData.CurrentLevel, saveData.IsShieldSpawned, saveData.IsSwordSpawned);

                _gameManager.GameMap.Entities.Clear();
                _gameManager.GameMap.Entities.Add(_gameManager.Player);

                if (_gameManager.ShieldTimer > 0 && _gameManager.SwordUses > 0)
                    _gameManager.ActivePlayer = new SwordDecorator(new ShieldDecorator(_gameManager.Player));
                else if (_gameManager.ShieldTimer > 0)
                    _gameManager.ActivePlayer = new ShieldDecorator(_gameManager.Player);
                else if (_gameManager.SwordUses > 0)
                    _gameManager.ActivePlayer = new SwordDecorator(_gameManager.Player);
                else
                    _gameManager.ActivePlayer = _gameManager.Player;

                foreach (var enemyData in saveData.Enemies)
                {
                    Enemy enemy = enemyData.Type == "Ork" ? new Ork() : new Slime();
                    enemy.X = enemyData.X;
                    enemy.Y = enemyData.Y;
                    enemy.HealthPoints.InitHealth(enemyData.CurrentHealth, enemyData.MaxHealth);

                    if (enemyData.StrategyType == "FleeBehavior")
                        enemy.SetStrategy(new FleeBehavior());
                    else
                        enemy.SetStrategy(new MeleeAttack());

                    _gameManager.GameMap.Entities.Add(enemy);
                }
                
                _gameManager.Player.HealthPoints.ForceUpdateNotification();
                _statusMessage = "[ СИСТЕМА ]: Загручено с ласт чекпоинта!";
                _messageTimer = 2000;
                Console.Clear();
            }
            catch
            {
                _statusMessage = "[ ОШИБКА ]: Не удалось загрузить чекпоинт.";
                _messageTimer = 2000;
            }
        }
    }
}