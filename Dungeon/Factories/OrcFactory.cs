using Dungeon.Entities;

namespace Dungeon.Factories
{
    public class OrcFactory : EnemyFactory
    {
        public override Enemy CreateEnemy() => new Ork();
    }
}