using NUnit.Framework;
using Dungeon.Core;
using Dungeon.States;

namespace Dungeon.Tests
{
    [TestFixture]
    public class StateTests
    {
        [Test]
        public void GameManager_ShouldSwitchToPauseState_WhenGameIsPaused()
        {
            GameManager gameManager = GameManager.Instance;
            gameManager.ChangeState(new GameplayState(gameManager));

            gameManager.ChangeState(new PauseState(gameManager));

            if (gameManager.CurrentState is not PauseState)
            {
                throw new System.InvalidOperationException("Состояние игры не переключилось на Паузу");
            }
        }

        [Test]
        public void GameManager_ShouldSwitchToGameOverState_WhenPlayerDies()
        {
            GameManager gameManager = GameManager.Instance;
            GameplayState gameplayState = new GameplayState(gameManager);
            gameManager.ChangeState(gameplayState);

            gameManager.Player.TakeDamage(gameManager.Player.HealthPoints.Max);
            gameplayState.Update(0);

            if (gameManager.CurrentState is not GameOverState)
            {
                throw new System.InvalidOperationException("Состояние игры не переключилось на GameOver при смерти игрока");
            }
        }
    }
}