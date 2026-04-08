using Dungeon.Entities;

namespace Dungeon.Decorators
{
    public abstract class EntityDecorator : IEntity
    {
        protected IEntity _innerEntity;

        public EntityDecorator(IEntity entity)
        {
            _innerEntity = entity;
        }

        public virtual string Name => _innerEntity.Name;
        public virtual int Health => _innerEntity.Health;
        public virtual int X { get => _innerEntity.X; set => _innerEntity.X = value; }
        public virtual int Y { get => _innerEntity.Y; set => _innerEntity.Y = value; }

        public virtual int CalculateIncomingDamage(int rawDamage) => _innerEntity.CalculateIncomingDamage(rawDamage);
        public virtual int CalculateOutgoingDamage(int baseDamage) => _innerEntity.CalculateOutgoingDamage(baseDamage);
    }
}