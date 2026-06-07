namespace Dungeon.Entities
{
    public interface IAttackStrategy
    {
        void Execute(Enemy enemy, IEntity activePlayer, Player playerBase, Core.CombatFacade combat, System.Action onSwordBreak);
    }
}