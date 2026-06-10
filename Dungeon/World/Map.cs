using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Dungeon.Entities;

namespace Dungeon.World
{
    public class Map
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public List<Entity> Entities { get; set; } = new List<Entity>();
        
        public int ShieldX { get; set; } = 10;
        public int ShieldY { get; set; } = 5;
        public bool IsShieldSpawned { get; set; } = true;

        public int SwordX { get; set; } = 20;
        public int SwordY { get; set; } = 7;
        public bool IsSwordSpawned { get; set; } = true;

        public int TrapX { get; private set; } = -1;
        public int TrapY { get; private set; } = -1;
        public bool IsHoleSpawned => TrapX != -1 && TrapY != -1;

        private readonly int _levelNumber;
        private readonly bool[,] _innerWalls;

        public Map(int width, int height, int levelNumber)
        {
            Width = width;
            Height = height;
            _levelNumber = levelNumber;
            _innerWalls = new bool[width, height];

            GenerateLevelLayout();
        }

        public bool IsWall(int x, int y)
        {
            if (x <= 0 || x >= Width - 1 || y <= 0 || y >= Height - 1) return true;
            return _innerWalls[x, y];
        }

        private void GenerateLevelLayout()
        {
            if (_levelNumber == 2)
            {
                for (int y = 2; y < Height - 2; y++) _innerWalls[12, y] = true;
            }
            else if (_levelNumber == 3)
            {
                for (int x = 4; x < Width - 4; x++) _innerWalls[x, 4] = true;
            }
            else if (_levelNumber == 4)
            {
                for (int y = 1; y < Height - 3; y++) _innerWalls[8, y] = true;
                for (int y = 3; y < Height - 1; y++) _innerWalls[22, y] = true;
            }
            else if (_levelNumber == 5)
            {
                for (int x = 2; x < Width - 2; x += 3)
                {
                    _innerWalls[x, 3] = true;
                    _innerWalls[x, 6] = true;
                }
            }
        }

        public void SpawnHole()
        {
            if (IsHoleSpawned) return;

            Random rand = new Random();
            while (true)
            {
                int x = rand.Next(2, Width - 2);
                int y = rand.Next(2, Height - 2);

                if (!_innerWalls[x, y] && (x != playerX() || y != playerY()) && (x != ShieldX || y != ShieldY) && (x != SwordX || y != SwordY))
                {
                    TrapX = x;
                    TrapY = y;
                    break;
                }
            }
        }

        private int playerX() => Entities.OfType<Player>().FirstOrDefault()?.X ?? 15;
        private int playerY() => Entities.OfType<Player>().FirstOrDefault()?.Y ?? 7;

        public string GetView()
        {
            StringBuilder sb = new StringBuilder();
            
            var player = Entities.OfType<Player>().FirstOrDefault();
            int hp = player?.HealthPoints?.Current ?? 0;

            sb.AppendLine("==========================================");
            sb.AppendLine($"   DUNGEON CRAWLER | ЯРУС: {_levelNumber}/5 | HP: {hp} ");
            sb.AppendLine("==========================================");

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Entity? target = Entities.Find(e => e.X == x && e.Y == y);
                    
                    if (target != null)
                    {
                        if (target is Player) sb.Append("@");
                        else if (target is Slime) sb.Append("S");
                        else if (target is Ork) sb.Append("O");
                        else sb.Append("E"); 
                    }
                    else if (IsHoleSpawned && x == TrapX && y == TrapY)
                    {
                        sb.Append("X");
                    }
                    else if (IsShieldSpawned && x == ShieldX && y == ShieldY)
                    {
                        sb.Append("[");
                    }
                    else if (IsSwordSpawned && x == SwordX && y == SwordY)
                    {
                        sb.Append("!");
                    }
                    else if (y == 0 || y == Height - 1 || x == 0 || x == Width - 1 || _innerWalls[x, y])
                    {
                        sb.Append("#");
                    }
                    else
                    {
                        sb.Append(".");
                    }
                }
                sb.AppendLine();
            }
            sb.AppendLine("==========================================");
            return sb.ToString();
        }
    }
}