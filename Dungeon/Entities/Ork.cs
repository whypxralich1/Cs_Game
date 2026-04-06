using System;

namespace Dungeon.Entities
{
    public class Orc : Enemy
    {
        public Orc()
        {
            Name = "Iron Orc";
            Health = 60;
            Damage = 15;
        }

        public override void Attack()
        {
            Console.WriteLine($"{Name} бьет тяжелой дубиной! Нанесено {Damage} урона.");
        }
    }
}