using System;
using Dungeon.World;
using Dungeon.Entities;

namespace Dungeon.Core
{
    public class ConsoleRenderer
    {
        private string _hudLine = "[HP: ||||||||||] 100/100";

        public void SubscribeToPlayerHealth(Logic.Health healthSystem)
        {
            if (healthSystem != null)
            {
                healthSystem.OnHealthChanged += UpdateHUD;
                UpdateHUD(healthSystem.Current, healthSystem.Max);
            }
        }

        public void UnsubscribeFromPlayerHealth(Logic.Health healthSystem)
        {
            if (healthSystem != null)
            {
                healthSystem.OnHealthChanged -= UpdateHUD;
            }
        }

        private void UpdateHUD(int current, int max)
        {
            int totalBars = 10;
            double percentage = max > 0 ? (double)current / max : 0;
            int activeBars = (int)Math.Round(percentage * totalBars);
            string barText = new string('|', activeBars) + new string('.', totalBars - activeBars);
            _hudLine = $"[HP: {barText}] {current}/{max}";
        }

        public void ShowStartScreen()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("########################################");
            Console.WriteLine("#                                      #");
            Console.WriteLine("#           DUNGEON CRAWLER            #");
            Console.WriteLine("#                                      #");
            Console.WriteLine("########################################");
            Console.ResetColor();
            Console.WriteLine("\nНажмите любую клавишу для начала приключений...");
            Console.ReadKey(true);
            Console.Clear();
        }

        public void Render(Map map, IEntity activePlayer, Player playerBase, int shieldTimer, int swordUses)
        {
            Console.SetCursorPosition(0, 0);
            
            string view = map.GetView();
            Console.Write(view);

            Console.SetCursorPosition(0, map.Height + 1);
            bool hasSword = swordUses > 0 || activePlayer.GetType().Name.Contains("Sword");
            string weaponStatus = hasSword ? $"Меч (Ударов: {5 - swordUses})" : "Кулаки";

            string statusLine = $"[ СТАТУС ]: {playerBase.Name} | Оружие: {weaponStatus} | {_hudLine}";
            Console.Write(statusLine.PadRight(80));

            Console.SetCursorPosition(0, map.Height + 2);
            if (shieldTimer > 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"[ ЩИТ ]: Активен еще {shieldTimer} ход(ов).".PadRight(80));
                Console.ResetColor();
            }
            else
            {
                Console.Write("[ ЩИТ ]: Неактивен".PadRight(80));
            }
        }

        public void ShowGameOver(string message)
        {
            Console.Clear();
            Console.SetCursorPosition(0, 0);
            Console.WriteLine("######################################");
            Console.WriteLine($"# {message.PadLeft(message.Length + (34 - message.Length) / 2).PadRight(34)} #");
            Console.WriteLine("######################################");
            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey(true);
        }
    }
}