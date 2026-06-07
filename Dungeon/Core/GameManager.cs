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

        private const int DefaultShieldDuration = 5000;

        private bool _isLoopRunning = true;
        private string _exitMessage = "Конец";
        private Stopwatch _gameTimer = new Stopwatch();
        private IGameState _currentState = null!;
        private readonly Stack<ICommand> _commandHistory = new Stack<ICommand>();

        public Map GameMap { get; }
        public Player Player { get; }
        public IEntity ActivePlayer { get; set; }
        public int ShieldTimer { get; set; }
        public int SwordUses { get; set; }

        public CombatFacade Combat { get; } = new CombatFacade();
        public ConsoleRenderer Renderer { get; } = new ConsoleRenderer();
        public InputHandler Input { get; } = new InputHandler();
        public IGameState CurrentState => _currentState;

        private GameManager()
        {
            GameMap = new Map(30, 10);
            Player = new Player { Name = "Hero", X = 15, Y = 7 };
            ActivePlayer = Player;
            GameMap.Entities.Add(Player);
            Renderer.SubscribeToPlayerHealth(Player.HealthPoints);
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
        }

        public void UndoLastCommand()
        {
            if (_commandHistory.Count > 0)
            {
                ICommand command = _commandHistory.Pop();
                command.Undo();
            }
        }

        public void Run()
        {
            InitializeConsole();
            
            GameMap.Entities.Add(new Ork { X = 5, Y = 3 });
            GameMap.Entities.Add(new Slime { X = 20, Y = 3 });

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

                System.Threading.Thread.Sleep(10);
            }
        }

        private void InitializeConsole()
        {
            Console.Clear();
            Console.CursorVisible = false;
        }

        public void HandleItemPickups()
        {
            if (GameMap.IsShieldSpawned && IsPlayerOnItem(GameMap.ShieldX, GameMap.ShieldY))
            {
                ActivePlayer = new ShieldDecorator(Player);
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
                if (IsAdjacentToPlayer(enemy))
                {
                    int escapeThreshold = (int)(enemy.HealthPoints.Max * 0.3);

                    if (enemy.HealthPoints.Current <= escapeThreshold && !enemy.HealthPoints.IsDead)
                    {
                        if (!(enemy.AttackStrategy is FleeBehavior))
                        {
                            enemy.SetStrategy(new FleeBehavior());
                        }

                        enemy.ExecuteAttack(ActivePlayer, Player, Combat, () => {});
                        continue;
                    }

                    if (enemy.CanAttack && !enemy.HealthPoints.IsDead)
                    {
                        enemy.ExecuteAttack(ActivePlayer, Player, Combat, () => {
                            if (ActivePlayer is SwordDecorator)
                            {
                                SwordUses++;
                                if (SwordUses >= 2)
                                {
                                    ActivePlayer = (ShieldTimer > 0) ? new ShieldDecorator(Player) : Player;
                                    SwordUses = 0;
                                }
                            }
                        });
                    }
                }

                if (enemy.HealthPoints.IsDead)
                {
                    GameMap.Entities.Remove(enemy);
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