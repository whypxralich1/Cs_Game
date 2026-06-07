using System;

namespace Dungeon.Entities
{
    public class FleeBehavior : IAttackStrategy
    {
        public void Execute(Enemy enemy, IEntity activePlayer, Player playerBase, Core.CombatFacade combat, Action onSwordBreak)
        {
            int dx = enemy.X - playerBase.X;
            int dy = enemy.Y - playerBase.Y;

            if (dx == 0 && dy == 0)
            {
                dx = new Random().Next(-1, 2);
                dy = new Random().Next(-1, 2);
            }

            int nextX = enemy.X + Math.Sign(dx);
            int nextY = enemy.Y + Math.Sign(dy);

            if (nextX > 0 && nextX < 29 && nextY > 0 && nextY < 9)
            {
                enemy.X = nextX;
                enemy.Y = nextY;
            }
        }
    }
}