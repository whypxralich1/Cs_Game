using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using Dungeon.World;
using Dungeon.Entities;
using Dungeon.Decorators;
using Dungeon.States;

namespace Dungeon.Core
{
    public class GameManager : IDisposable
    {
        private static GameManager? _instance;
        public static GameManager Instance => _instance ??= new GameManager();

        private const int DefaultShieldDuration = 20;

        private bool _isLoopRunning = true;
        private Stopwatch _gameTimer = new Stopwatch();
        private IGameState _currentState = null!;
        private readonly Stack<ICommand> _commandHistory = new Stack<ICommand>();
        private Dictionary<Enemy, int> _enemyDirections = new Dictionary<Enemy, int>();

        public Map GameMap { get; set; } = null!;
        public Player Player { get; }
        public IEntity ActivePlayer { get; set; }
        public int ShieldTimer { get; set; }
        public int SwordUses { get; set; }
        public int CurrentLevel { get; set; } = 1;

        public CombatFacade Combat { get; } = new CombatFacade();
        public ConsoleRenderer Renderer { get; } = new ConsoleRenderer();
        public InputHandler Input { get; } = new InputHandler();
        public IGameState CurrentState => _currentState;

        private GameManager()
        {
            Player = new Player { Name = "Hero", X = 15, Y = 7 };
            ActivePlayer = Player;
            Renderer.SubscribeToPlayerHealth(Player.HealthPoints);
            LoadLevel(CurrentLevel);
            ChangeState(new GameplayState(this));
        }

        public void ChangeState(IGameState newState)
        {
            _currentState = newState;
        }

        public void StopGame()
        {
            _isLoopRunning = false;
        }

        public void ExecuteGameCommand(ICommand command)
        {
            command.Execute();
            _commandHistory.Push(command);
            OnPlayerTurn();
        }

        public void UndoLastCommand()
        {
            if (_commandHistory.Count > 0)
            {
                ICommand command = _commandHistory.Pop();
                command.Undo();
                OnPlayerTurn();
            }
        }

        private void LoadLevel(int level)
        {
            GameMap = new Map(30, 10, level);
            Player.X = 15;
            Player.Y = 7;
            GameMap.Entities.Clear();
            GameMap.Entities.Add(Player);
            _commandHistory.Clear();
            _enemyDirections.Clear();

            ShieldTimer = 0;
            if (SwordUses >= 5)
            {
                SwordUses = 0;
                ActivePlayer = Player;
            }
            else if (SwordUses > 0)
            {
                ActivePlayer = new SwordDecorator(Player);
            }
            else
            {
                ActivePlayer = Player;
            }

            Random rand = new Random();
            
            int orkX = rand.Next(4, 10);
            int orkY = rand.Next(3, 6);
            var ork = new Ork { X = orkX, Y = orkY };
            
            int slimeX = rand.Next(20, 26);
            int slimeY = rand.Next(3, 6);
            var slime = new Slime { X = slimeX, Y = slimeY };

            GameMap.Entities.Add(ork);
            GameMap.Entities.Add(slime);

            _enemyDirections[ork] = rand.Next(0, 2) == 0 ? 1 : -1;
            _enemyDirections[slime] = rand.Next(0, 2) == 0 ? 1 : -1;
        }

        public void SetupLoadedMap(int level, bool isShieldSpawned, bool isSwordSpawned)
        {
            CurrentLevel = level;
            GameMap = new Map(30, 10, level);
            GameMap.IsShieldSpawned = isShieldSpawned;
            GameMap.IsSwordSpawned = isSwordSpawned;
            
            _commandHistory.Clear();
            _enemyDirections.Clear();
        }

        public void OnPlayerTurn()
        {
            if (ShieldTimer > 0)
            {
                ShieldTimer--;
                if (ShieldTimer <= 0)
                {
                    ShieldTimer = 0;
                    if (ActivePlayer is ShieldDecorator shieldOpt)
                    {
                        ActivePlayer = shieldOpt.InnerEntity ?? Player;
                    }
                    else
                    {
                        ActivePlayer = Player;
                    }
                }
            }

            var enemies = GameMap.Entities.OfType<Enemy>().ToList();
            
            if (enemies.Count == 0)
            {
                GameMap.SpawnHole();
                return;
            }

            foreach (var enemy in enemies)
            {
                enemy.UpdateCooldown(-1000);

                if (!_enemyDirections.ContainsKey(enemy))
                {
                    _enemyDirections[enemy] = 1;
                }

                if (IsAdjacentToPlayer(enemy))
                {
                    if (!enemy.HealthPoints.IsDead)
                    {
                        enemy.ExecuteAttack(ActivePlayer, Player, Combat, () => {
                            bool hasSword = ActivePlayer is SwordDecorator || (ActivePlayer is ShieldDecorator sd && sd.InnerEntity is SwordDecorator);
                            if (hasSword)
                            {
                                SwordUses++;
                                if (SwordUses >= 5)
                                {
                                    if (ActivePlayer is SwordDecorator) ActivePlayer = Player;
                                    else if (ActivePlayer is ShieldDecorator shieldDec) ActivePlayer = new ShieldDecorator(Player);
                                    SwordUses = 0;
                                }
                            }
                        });
                    }
                }
                else if (!enemy.HealthPoints.IsDead)
                {
                    int dir = _enemyDirections[enemy];
                    int nextX = enemy.X + dir;

                    if (!GameMap.IsWall(nextX, enemy.Y) && !GameMap.Entities.Any(e => e.X == nextX && e.Y == enemy.Y))
                    {
                        enemy.X = nextX;
                    }
                    else
                    {
                        _enemyDirections[enemy] = -dir;
                    }
                }
            }
        }

        public void NextLevel()
        {
            Console.Clear();
            if (CurrentLevel >= 5)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n\n\n==================================================");
                Console.WriteLine("       ПОЗДРАВЛЯЕМ! ВЫ ПРОШЛИ ВСЕ ПОДЗЕМЕЛЬЕ!     ");
                Console.WriteLine("==================================================");
                Console.ResetColor();
                System.Threading.Thread.Sleep(3000);
                StopGame();
                return;
            }

            CurrentLevel++;
            LoadLevel(CurrentLevel);
        }

        public void Run()
        {
            InitializeConsole();
            Renderer.ShowStartScreen();

            while (Console.KeyAvailable)
            {
                Console.ReadKey(true);
            }
            
            Console.Clear();
            _gameTimer.Start();
            long lastTime = _gameTimer.ElapsedMilliseconds;

            while (_isLoopRunning)
            {
                long currentTime = _gameTimer.ElapsedMilliseconds;
                int deltaTime = (int)(currentTime - lastTime);
                lastTime = currentTime;

                _currentState.HandleInput();
                _currentState.Update(deltaTime);
                _currentState.Render();

                System.Threading.Thread.Sleep(20);
            }
            Console.Clear();
        }

        private void InitializeConsole()
        {
            if (OperatingSystem.IsWindows())
            {
                Console.SetWindowSize(80, 25);
                Console.SetBufferSize(80, 25);
            }
            
            Console.Clear();
            Console.CursorVisible = false;
        }

        public void HandleItemPickups()
        {
            if (GameMap.IsShieldSpawned && IsPlayerOnItem(GameMap.ShieldX, GameMap.ShieldY))
            {
                ActivePlayer = new ShieldDecorator(ActivePlayer);
                ShieldTimer = DefaultShieldDuration;
                GameMap.IsShieldSpawned = false;
            }

            if (GameMap.IsSwordSpawned && IsPlayerOnItem(GameMap.SwordX, GameMap.SwordY))
            {
                ActivePlayer = new SwordDecorator(ActivePlayer);
                SwordUses = 0; 
                GameMap.IsSwordSpawned = false;
            }
        }

        private bool IsPlayerOnItem(int itemX, int itemY)
        {
            return Player.X == itemX && Player.Y == itemY;
        }

        public void HandleCombat()
        {
            foreach (var enemy in GameMap.Entities.OfType<Enemy>().ToList())
            {
                if (enemy.HealthPoints.IsDead)
                {
                    GameMap.Entities.Remove(enemy);
                    _enemyDirections.Remove(enemy);
                    if (!GameMap.Entities.OfType<Enemy>().Any()) GameMap.SpawnHole();
                }
            }
        }

        private bool IsAdjacentToPlayer(Entity entity)
        {
            return Math.Abs(entity.X - Player.X) <= 1 && Math.Abs(entity.Y - Player.Y) <= 1;
        }

        public void Dispose()
        {
            Renderer.UnsubscribeFromPlayerHealth(Player?.HealthPoints!);
        }
    }
}