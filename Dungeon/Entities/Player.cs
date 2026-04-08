using Dungeon.Logic;

namespace Dungeon.Entities
{
    public class Player : Entity
    {
        public Player()
        {
            Name = "Hero";
            HealthPoints = new Health(100);
            X = 15;
            Y = 7;
        }

        public override Entity Clone() => (Player)this.MemberwiseClone();

        public int Health => HealthPoints.Current;
        public bool IsDead => HealthPoints.IsDead;
        
        public void TakeDamage(int amount) => HealthPoints.TakeDamage(amount);
    }
}