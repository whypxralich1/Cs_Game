using Dungeon.Core;

namespace Dungeon.States
{
    public class GameOverState : IGameState
    {
        private readonly GameManager _gameManager;
        private readonly string _message;

        public GameOverState(GameManager gameManager, string message)
        {
            _gameManager = gameManager;
            _message = message;
        }

        public void HandleInput()
        {
            _gameManager.StopGame();
        }

        public void Update(int deltaTime)
        {
        }

        public void Render()
        {
            _gameManager.Renderer.ShowGameOver(_message);
        }
    }
}