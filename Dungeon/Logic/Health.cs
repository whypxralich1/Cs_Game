using System;

namespace Dungeon.Logic
{
    public class Health
    {
        public int Current { get; private set; }
        public int Max { get; private set; }
        public bool IsDead => Current <= 0;

        public Health(int max)
        {
            if (max <= 0) throw new ArgumentException("Max health must be positive");
            Max = max;
            Current = max;
        }

        public void TakeDamage(int amount)
        {
            if (amount < 0) return;
            Current -= amount;
            if (Current < 0) Current = 0;
        }

        public void Heal(int amount)
        {
            if (IsDead || amount < 0) return;
            Current += amount;
            if (Current > Max) Current = Max;
        }
    }
}