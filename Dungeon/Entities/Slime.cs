// Slime.cs
using System;
using System.Collections.Generic;
using Dungeon.Logic;

namespace Dungeon.Entities
{
    public class Slime : Enemy
    {
        public Slime()
        {
            Name = "Green Slime";
            HealthPoints = new Health(20);
            Damage = 5;
            Skills = new List<string> { "Sticky Spit" };
        }

        public override void Attack()
        {
            Console.WriteLine($"{Name} брызгает слизью! Нанесено {Damage} урона.");
        }

        public override Entity Clone()
        {
            Slime clone = (Slime)this.MemberwiseClone();
            clone.Skills = new List<string>(this.Skills);
            return clone;
        }
    }
}