namespace Dungeon.Entities
{
    public class Player : Entity
    {
        public int Experience { get; set; } = 0;

        public Player(string name)
        {
            Name = name;
            Health = 100;
        }
    }
}