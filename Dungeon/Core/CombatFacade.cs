using System;
using Dungeon.Entities;

namespace Dungeon.Core
{
    public class CombatFacade
    {
        public void ResolveCombat(IEntity activeEntity, Player playerBase, Enemy enemy, Action onSwordBreak)
        {
            int incoming = activeEntity.CalculateIncomingDamage(enemy.Damage);
            playerBase.TakeDamage(incoming);

            int outgoing = activeEntity.CalculateOutgoingDamage(0);
            if (outgoing > 0)
            {
                enemy.TakeDamage(outgoing);
                onSwordBreak?.Invoke();
            }
        }
    }
}