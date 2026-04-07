using System;
using System.Collections.Generic;

namespace Dungeon.Entities
{
    public class Ork : Enemy
    {
        public Ork()
        {
            Name = "Iron Orc";
            Health = 60;
            Damage = 15;
            Skills = new List<string> { "Heavy Swing" };
        }

        public override void Attack()
        {
            Console.WriteLine($"{Name} бьет тяжелой дубиной! Нанесено {Damage} урона.");
        }

        public override Entity Clone()
        {
            Ork clone = (Ork)this.MemberwiseClone();
            clone.Skills = new List<string>(this.Skills);
            return clone;
        }
    }
}