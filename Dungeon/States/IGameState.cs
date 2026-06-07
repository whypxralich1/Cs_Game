namespace Dungeon.States
{
    public interface IGameState
    {
        void HandleInput();
        void Update(int deltaTime);
        void Render();
    }
}