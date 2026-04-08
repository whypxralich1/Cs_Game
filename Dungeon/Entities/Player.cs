using System;
using Dungeon.Logic;

namespace Dungeon.Entities
{
    public class Player : Entity
    {
        public Player()
        {
            Name = "Hero";
            HealthPoints = new Health(100); 
        }

        public override Entity Clone()
        {
            return new Player
            {
                X = this.X,
                Y = this.Y,
                Name = this.Name,
                HealthPoints = new Health(this.HealthPoints.Max) 
            };
        }
    }
}