namespace Dungeon.Core
{
    public interface ICommand
    {
        void Execute();
        void Undo();
    }
}