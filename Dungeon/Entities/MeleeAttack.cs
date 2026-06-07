using System;

namespace Dungeon.Entities
{
    public class MeleeAttack : IAttackStrategy
    {
        public void Execute(Enemy enemy, IEntity activePlayer, Player playerBase, Core.CombatFacade combat, Action onSwordBreak)
        {
            combat.ResolveCombat(activePlayer, playerBase, enemy, onSwordBreak);
        }
    }
}