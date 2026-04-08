using System.Collections.Generic;
using Dungeon.Logic; // Добавили

namespace Dungeon.Entities
{
    public abstract class Entity
    {
        public string Name { get; set; } = string.Empty;
    public Health HealthPoints { get; set; } = null!;
        public int X { get; set; }
        public int Y { get; set; }

        public abstract Entity Clone();

        public void Move(int dx, int dy)
        {
            X += dx;
            Y += dy;
        }
    }
}