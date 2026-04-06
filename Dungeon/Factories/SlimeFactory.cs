using Dungeon.Entities;

namespace Dungeon.Factories
{
    public class SlimeFactory : EnemyFactory
    {
        public override Enemy CreateEnemy() => new Slime();
    }
}