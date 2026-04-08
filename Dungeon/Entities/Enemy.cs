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

        public void UpdateCooldown(int deltaTime)
        {
            if (_attackCooldown > 0)
                _attackCooldown -= deltaTime;
        }

        public void ResetCooldown()
        {
            _attackCooldown = CooldownDuration;
        }
        public abstract void Attack();
        public override abstract Entity Clone();
    }
}