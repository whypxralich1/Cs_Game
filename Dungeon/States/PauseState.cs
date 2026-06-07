using System;
using Dungeon.Core;

namespace Dungeon.States
{
    public class PauseState : IGameState
    {
        private readonly GameManager _gameManager;

        public PauseState(GameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public void HandleInput()
        {
            var (_, _, escape) = _gameManager.Input.GetMovement();
            if (escape)
            {
                _gameManager.ChangeState(new GameplayState(_gameManager));
            }
        }

        public void Update(int deltaTime)
        {
        }

        public void Render()
        {
            _gameManager.Renderer.Render(_gameManager.GameMap, _gameManager.ActivePlayer, _gameManager.Player, _gameManager.ShieldTimer, _gameManager.SwordUses);
            Console.SetCursorPosition(0, _gameManager.GameMap.Height + 2);
            Console.WriteLine("[ ИГРА НА ПАУЗЕ ]: Нажмите ESC, чтобы вернуться...".PadRight(80));
        }
    }
}