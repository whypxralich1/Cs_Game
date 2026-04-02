namespace Dungeon.Entities
{
    public abstract class Entity
    {
        public string Name { get; set; } = string.Empty;
        public int Health { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public void Move(int dx, int dy)
        {
            X += dx;
            Y += dy;
        }
    }
}