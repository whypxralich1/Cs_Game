using System;

namespace Dungeon.Logic
{
    public class Health
    {
        public event Action<int, int>? OnHealthChanged;

        private int _current;
        private int _max;

        public int Max
        {
            get => _max;
            private set => _max = value;
        }

        public int Current
        {
            get => _current;
            private set
            {
                _current = Math.Clamp(value, 0, _max);
                OnHealthChanged?.Invoke(_current, _max);
            }
        }

        public bool IsDead => _current <= 0;

        public Health(int maxHealth)
        {
            _max = maxHealth;
            _current = maxHealth;
        }

        public void TakeDamage(int amount)
        {
            Current -= amount;
        }

        public void Heal(int amount)
        {
            Current += amount;
        }

        public void InitHealth(int current, int max)
        {
            _max = max;
            _current = current;
        }

        public void ForceUpdateNotification()
        {
            OnHealthChanged?.Invoke(_current, _max);
        }
    }
}