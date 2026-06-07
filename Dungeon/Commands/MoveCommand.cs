using Dungeon.Entities;
using Dungeon.Core;

namespace Dungeon.Commands
{
    public class MoveCommand : ICommand
    {
        private readonly Player _player;
        private readonly int _dx;
        private readonly int _dy;
        private int _prevX;
        private int _prevY;

        public MoveCommand(Player player, int dx, int dy)
        {
            _player = player;
            _dx = dx;
            _dy = dy;
        }

        public void Execute()
        {
            _prevX = _player.X;
            _prevY = _player.Y;
            
            _player.X += _dx;
            _player.Y += _dy;
        }

        public void Undo()
        {
            _player.X = _prevX;
            _player.Y = _prevY;
        }
    }
}