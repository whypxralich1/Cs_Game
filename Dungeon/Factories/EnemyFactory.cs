using Dungeon.Entities;

namespace Dungeon.Factories
{
    public abstract class EnemyFactory
    {
        public abstract Enemy CreateEnemy();

        public Enemy Spawn()
        {
            var enemy = CreateEnemy();
            return enemy;
        }
    }
}