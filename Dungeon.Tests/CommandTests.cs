using NUnit.Framework;
using Dungeon.Entities;
using Dungeon.Commands;

namespace Dungeon.Tests
{
    [TestFixture]
    public class CommandTests
    {
        [Test]
        public void MoveCommand_Execute_ShouldChangePlayerCoordinates()
        {
            Player player = new Player { X = 10, Y = 10 };
            MoveCommand command = new MoveCommand(player, 1, -2);

            command.Execute();

            if (player.X != 11 || player.Y != 8)
            {
                throw new System.InvalidOperationException($"Команда перемещения отработала некорректно. Получено: X={player.X}, Y={player.Y}");
            }
        }

        [Test]
        public void MoveCommand_Undo_ShouldRestorePreviousCoordinates()
        {
            Player player = new Player { X = 10, Y = 10 };
            MoveCommand command = new MoveCommand(player, 5, 5);

            command.Execute();
            command.Undo();

            if (player.X != 10 || player.Y != 10)
            {
                throw new System.InvalidOperationException($"Отмена команды (Undo) не вернула старые координаты. Получено: X={player.X}, Y={player.Y}");
            }
        }
    }
}