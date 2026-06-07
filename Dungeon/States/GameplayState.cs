using System;
using System.Linq;
using Dungeon.Core;
using Dungeon.Entities;
using Dungeon.Decorators;

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
            var (dx, dy, escape) = _gameManager.Input.GetMovement();

            if (escape)
            {
                _gameManager.ChangeState(new PauseState(_gameManager));
                return;
            }

            if (dx == 0 && dy == 0) return;

            int nextX = _gameManager.Player.X + dx;
            int nextY = _gameManager.Player.Y + dy;

            if (nextX > 0 && nextX < _gameManager.GameMap.Width - 1 && nextY > 0 && nextY < _gameManager.GameMap.Height - 1)
            {
                _gameManager.Player.X = nextX;
                _gameManager.Player.Y = nextY;
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
        }
    }
}