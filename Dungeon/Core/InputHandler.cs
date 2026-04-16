using System;

namespace Dungeon.Core
{
    public class InputHandler
    {
        public (int dx, int dy, bool escape) GetMovement()
        {
            if (!Console.KeyAvailable) return (0, 0, false);

            var key = Console.ReadKey(true).Key;
            switch (key)
            {
                case ConsoleKey.W: return (0, -1, false);
                case ConsoleKey.S: return (0, 1, false);
                case ConsoleKey.A: return (-1, 0, false);
                case ConsoleKey.D: return (1, 0, false);
                case ConsoleKey.Escape: return (0, 0, true);
                default: return (0, 0, false);
            }
        }
    }
}