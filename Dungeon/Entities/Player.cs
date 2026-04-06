namespace Dungeon.Entities
{
    public class Player : Entity
    {
        public Player()
        {
            Name = "Hero";
            Health = 100;
            X = 10;
            Y = 5;
        }

        public void TakeDamage(int amount)
        {
            Health -= amount;
        }

        public bool IsDead => Health <= 0;
    }
}