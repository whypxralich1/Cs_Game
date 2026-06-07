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

        public void Render(Map map, IEntity activePlayer, Player playerBase, int shieldTimer, int swordUses)
        {
            Console.SetCursorPosition(0, 0);
            
            string view = map.GetView();
            if (view.Contains("DUNGEON CRAWLER"))
            {
                int endOfFirstLine = view.IndexOf('\n');
                if (endOfFirstLine != -1)
                {
                    int totalCreatures = map.Entities.Count - 1;
                    string newHeader = $"DUNGEON CRAWLER | Существ: {totalCreatures}";
                    view = newHeader.PadRight(60) + view.Substring(endOfFirstLine);
                }
            }
            
            Console.Write(view);

            string weaponStatus = activePlayer is Decorators.SwordDecorator 
                ? $"Меч (Ударов: {2 - swordUses})" 
                : "Кулаки";

            string statusLine = $"[ СТАТУС ]: {activePlayer.Name} | Оружие: {weaponStatus} | {_hudLine}";
            Console.WriteLine(statusLine.PadRight(80));

            if (shieldTimer > 0)
                Console.WriteLine($"[ ЩИТ ]: {(shieldTimer / 1000.0):F1} сек.".PadRight(80));
            else
                Console.WriteLine("".PadRight(80));
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