using System;
using System.Collections.Generic;
using Dungeon.Logic;

namespace Dungeon.Entities
{
    public class Ork : Enemy
    {
        public Ork()
        {
            Name = "Iron Orc";
            HealthPoints = new Health(125); 
            Damage = 15;
            Skills = new List<string> { "Heavy Swing" };
            SetStrategy(new MeleeAttack());
        }

        public override Entity Clone()
        {
            Ork clone = (Ork)this.MemberwiseClone();
            clone.Skills = new List<string>(this.Skills);
            return clone;
        }
    }
}