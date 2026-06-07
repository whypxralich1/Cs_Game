using System;
using System.Collections.Generic;

namespace Dungeon.Entities
{
    public abstract class Enemy : Entity
    {
        public int Damage { get; protected set; }
        public List<string> Skills { get; set; } = new List<string>();
        private int _attackCooldown = 0;
        private const int CooldownDuration = 500;

        public bool CanAttack => _attackCooldown <= 0;

        public int HitsReceived { get; set; } = 0;

        public IAttackStrategy AttackStrategy { get; private set; } = null!;

        public void SetStrategy(IAttackStrategy strategy)
        {
            AttackStrategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
        }

        public void ExecuteAttack(IEntity activePlayer, Player playerBase, Core.CombatFacade combat, Action onSwordBreak)
        {
            if (AttackStrategy != null)
            {
                AttackStrategy.Execute(this, activePlayer, playerBase, combat, onSwordBreak);
                ResetCooldown();
            }
        }

        public void UpdateCooldown(int deltaTime)
        {
            if (_attackCooldown > 0)
                _attackCooldown -= deltaTime;
        }

        public void ResetCooldown()
        {
            _attackCooldown = CooldownDuration;
        }

        public override abstract Entity Clone();
    }
}