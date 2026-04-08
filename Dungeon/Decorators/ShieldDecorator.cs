using Dungeon.Entities;

namespace Dungeon.Decorators
{
    public class ShieldDecorator : EntityDecorator
    {
        public ShieldDecorator(IEntity entity) : base(entity) { }

        public override string Name => _innerEntity.Name + " [С ЩИТОМ]";

        public override int CalculateIncomingDamage(int rawDamage)
        {
            int reducedDamage = (int)(rawDamage * 0.25);
            return _innerEntity.CalculateIncomingDamage(reducedDamage);
        }
    }
}