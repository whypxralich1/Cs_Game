using Dungeon.Entities;

namespace Dungeon.Decorators
{
    public class SwordDecorator : EntityDecorator
    {
        public SwordDecorator(IEntity entity) : base(entity) { }

        public override string Name => _innerEntity.Name + " [С МЕЧОМ]";

        public override int CalculateOutgoingDamage(int baseDamage)
        {
            return _innerEntity.CalculateOutgoingDamage(baseDamage + 100);
        }
    }
}