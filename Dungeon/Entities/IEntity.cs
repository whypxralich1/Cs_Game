namespace Dungeon.Entities
{
    public interface IEntity
    {
        string Name { get; }
        int Health { get; }
        int X { get; set; }
        int Y { get; set; }
        
        int CalculateIncomingDamage(int rawDamage); 
        int CalculateOutgoingDamage(int baseDamage);
    }
}