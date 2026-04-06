using System;

namespace Dungeon.Entities
{
    public class Slime : Enemy
    {
        public Slime()
        {
            Name = "Green Slime";
            Health = 20;
            Damage = 5;
        }

        public override void Attack()
        {
            Console.WriteLine($"{Name} брызгает слизью! Нанесено {Damage} урона.");
        }
    }
}