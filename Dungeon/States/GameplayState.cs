using System;
using System.Linq;
using Dungeon.Core;
using Dungeon.Entities;
using Dungeon.Decorators;
using Dungeon.Commands;

namespace Dungeon.States
{
    public class GameplayState : IGameState
    {
        private readonly GameManager _gameManager;

        public GameplayState(GameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public void HandleInput()
        {
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                if (keyInfo.Key == ConsoleKey.Z)
                {
                    _gameManager.UndoLastCommand();
                    return;
                }

                if (keyInfo.Key == ConsoleKey.Escape)
                {
                    _gameManager.ChangeState(new PauseState(_gameManager));
                    return;
                }

                int dx = 0;
                int dy = 0;

                if (keyInfo.Key == ConsoleKey.UpArrow || keyInfo.Key == ConsoleKey.W) dy = -1;
                else if (keyInfo.Key == ConsoleKey.DownArrow || keyInfo.Key == ConsoleKey.S) dy = 1;
                else if (keyInfo.Key == ConsoleKey.LeftArrow || keyInfo.Key == ConsoleKey.A) dx = -1;
                else if (keyInfo.Key == ConsoleKey.RightArrow || keyInfo.Key == ConsoleKey.D) dx = 1;

                if (dx != 0 || dy != 0)
                {
                    int nextX = _gameManager.Player.X + dx;
                    int nextY = _gameManager.Player.Y + dy;

                    if (nextX > 0 && nextX < _gameManager.GameMap.Width - 1 && nextY > 0 && nextY < _gameManager.GameMap.Height - 1)
                    {
                        _gameManager.ExecuteGameCommand(new MoveCommand(_gameManager.Player, dx, dy));
                    }
                }
            }
        }

        public void Update(int deltaTime)
        {
            if (_gameManager.ShieldTimer > 0)
            {
                _gameManager.ShieldTimer -= deltaTime;
                if (_gameManager.ShieldTimer <= 0)
                {
                    _gameManager.ShieldTimer = 0;
                    _gameManager.ActivePlayer = (_gameManager.SwordUses > 0) ? new SwordDecorator(_gameManager.Player) : _gameManager.Player;
                }
            }

            foreach (var enemy in _gameManager.GameMap.Entities.OfType<Enemy>())
            {
                enemy.UpdateCooldown(deltaTime);
            }

            _gameManager.HandleItemPickups();
            _gameManager.HandleCombat();

            if (_gameManager.Player.IsDead)
            {
                _gameManager.ChangeState(new GameOverState(_gameManager, "ГЕРОЙ ПОГИБ!"));
            }
        }

        public void Render()
        {
            _gameManager.Renderer.Render(_gameManager.GameMap, _gameManager.ActivePlayer, _gameManager.Player, _gameManager.ShieldTimer, _gameManager.SwordUses);
            Console.SetCursorPosition(0, _gameManager.GameMap.Height + 3);
            Console.WriteLine("[ УПРАВЛЕНИЕ ]: WASD/Стрелочки - Ход | Z - Отмена шага (Undo)".PadRight(80));
        }
    }
}