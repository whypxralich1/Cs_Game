using System;
using Dungeon.World;
using Dungeon.Entities;

namespace Dungeon.Core
{
    public class ConsoleRenderer
    {
        public void Render(Map map, IEntity activePlayer, Player playerBase, int shieldTimer)
        {
            Console.SetCursorPosition(0, 0);
            Console.Write(map.GetView());

            string statusLine = $"[ СТАТУС ]: {activePlayer.Name} | HP: {playerBase.Health}";
            Console.WriteLine(statusLine.PadRight(60));

            if (shieldTimer > 0)
                Console.WriteLine($"[ ЩИТ ]: {(shieldTimer / 1000.0):F1} сек.".PadRight(60));
            else
                Console.WriteLine("".PadRight(60));
        }

        public void ShowGameOver(string message)
        {
            Console.Clear();
            Console.SetCursorPosition(0, 0);
            Console.WriteLine("######################################");
            Console.WriteLine($"# {message.PadLeft(message.Length + (34 - message.Length) / 2).PadRight(34)} #");
            Console.WriteLine("######################################");
            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey();
        }
    }
}