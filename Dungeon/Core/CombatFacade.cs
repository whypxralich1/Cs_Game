using System;
using Dungeon.Entities;

namespace Dungeon.Core
{
    public class CombatFacade
    {
        public void ResolveCombat(IEntity playerProxy, Player playerBase, Enemy enemy, Action onSwordBreak)
        {
            int incoming = playerProxy.CalculateIncomingDamage(enemy.Damage);
            playerBase.TakeDamage(incoming);

            int outgoing = playerProxy.CalculateOutgoingDamage(0);
            if (outgoing > 0)
            {
                enemy.TakeDamage(outgoing);
                onSwordBreak?.Invoke();
            }
        }
    }
}