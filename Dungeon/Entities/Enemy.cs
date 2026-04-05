namespace Dungeon.Entities
{
    public class Enemy : Entity
    {
        public int Damage { get; set; }

        public Enemy(string name, int damage)
        {
            Name = name;
            Damage = damage;
            Health = 50;
        }
    }
}