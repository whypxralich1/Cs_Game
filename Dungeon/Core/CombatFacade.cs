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

            int basePunchDamage = 10;
            int outgoing = activeEntity.CalculateOutgoingDamage(basePunchDamage);
            
            if (outgoing > 0)
            {
                enemy.TakeDamage(outgoing);
                
                if (activeEntity is Decorators.SwordDecorator)
                {
                    onSwordBreak?.Invoke();
                }
            }
        }
    }
}