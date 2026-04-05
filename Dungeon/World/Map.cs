using System;
using System.Collections.Generic;
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

        public void Draw()
        {
            Console.SetCursorPosition(0, 2);
            Console.WriteLine($"--- Dungeon Crawler: {Width}x{Height} ---");
            Console.WriteLine($"Существ на уровне: {Entities.Count}");
        }
    }
}