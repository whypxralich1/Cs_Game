using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Dungeon.Entities;

namespace Dungeon.World
{
    public class Map
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public List<Entity> Entities { get; set; } = new List<Entity>();

        public Map(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public string GetView()
        {
            StringBuilder sb = new StringBuilder();
            
            var player = Entities.OfType<Player>().FirstOrDefault();
            int hp = player?.Health ?? 0;

            sb.AppendLine("==========================================");
            sb.AppendLine($"   DUNGEON CRAWLER | HP: {hp} | Существ: {Entities.Count} ");
            sb.AppendLine("==========================================");

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Entity? target = Entities.Find(e => e.X == x && e.Y == y);
                    
                    if (target != null)
                    {
                        if (target is Player) sb.Append("@");
                        else if (target is Slime) sb.Append("S");
                        else if (target is Ork) sb.Append("O");
                        else sb.Append("E"); 
                    }
                    else if (y == 0 || y == Height - 1 || x == 0 || x == Width - 1)
                    {
                        sb.Append("#");
                    }
                    else
                    {
                        sb.Append(".");
                    }
                }
                sb.AppendLine();
            }
            sb.AppendLine("==============================");
            return sb.ToString();
        }
    }
}