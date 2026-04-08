using System.Collections.Generic;
using Dungeon.Logic;

namespace Dungeon.Entities
{
    public abstract class Entity : IEntity
    {
        public string Name { get; set; } = string.Empty;
        public int X { get; set; }
        public int Y { get; set; }
        
        public Health HealthPoints { get; set; } = null!;

        public int Health => HealthPoints.Current;

        public bool IsDead => HealthPoints.IsDead;

        public abstract Entity Clone();

        public void Move(int dx, int dy)
        {
            X += dx;
            Y += dy;
        }

        public virtual int CalculateIncomingDamage(int rawDamage) => rawDamage;
        public virtual int CalculateOutgoingDamage(int baseDamage) => baseDamage;

        public void TakeDamage(int amount) => HealthPoints.TakeDamage(amount);
    }
}