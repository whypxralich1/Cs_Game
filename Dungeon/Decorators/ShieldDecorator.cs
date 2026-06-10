using Dungeon.Entities;

namespace Dungeon.Decorators
{
    public class ShieldDecorator : EntityDecorator
    {
        private const double DamageReductionMultiplier = 0.25;

        public ShieldDecorator(IEntity entity) : base(entity) { }

        public override string Name => _innerEntity.Name + " [С ЩИТОМ]";

        public IEntity InnerEntity => _innerEntity;

        public override int CalculateIncomingDamage(int rawDamage)
        {
            int reducedDamage = (int)(rawDamage * DamageReductionMultiplier);
            return _innerEntity.CalculateIncomingDamage(reducedDamage);
        }
    }
}