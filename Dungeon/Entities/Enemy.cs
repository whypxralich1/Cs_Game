using System.Collections.Generic;

namespace Dungeon.Entities
{
    public abstract class Enemy : Entity
    {
        public int Damage { get; protected set; }
        public List<string> Skills { get; set; } = new List<string>();

        public abstract void Attack();
    }
}