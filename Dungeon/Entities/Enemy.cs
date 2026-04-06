namespace Dungeon.Entities
{
    public abstract class Enemy : Entity
    {
        public int Damage { get; protected set; }
        public abstract void Attack();
    }
}