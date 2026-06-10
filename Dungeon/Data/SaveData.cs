using System.Collections.Generic;

namespace Dungeon.Data
{
    public class SaveData
    {
        public int CurrentLevel { get; set; } = 1;
        public int PlayerX { get; set; }
        public int PlayerY { get; set; }
        public int CurrentHealth { get; set; }
        public int MaxHealth { get; set; }
        public int ShieldTimer { get; set; }
        public int SwordUses { get; set; }
        public bool IsShieldSpawned { get; set; }
        public bool IsSwordSpawned { get; set; }
        public List<EnemySaveData> Enemies { get; set; } = new List<EnemySaveData>();
    }

    public class EnemySaveData
    {
        public string Type { get; set; } = string.Empty;
        public int X { get; set; }
        public int Y { get; set; }
        public int CurrentHealth { get; set; }
        public int MaxHealth { get; set; }
        public string StrategyType { get; set; } = string.Empty;
    }
}