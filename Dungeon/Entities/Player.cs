namespace Dungeon.Entities
{
    public class Player : Entity
    {
        public Player()
        {
            Name = "Hero";
            Health = 100;
            X = 15;
            Y = 7;
        }

        public override Entity Clone() => (Player)this.MemberwiseClone();

        public void TakeDamage(int amount) => Health -= amount;

        public bool IsDead => Health <= 0;
    }
}