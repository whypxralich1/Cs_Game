using NUnit.Framework;
using Dungeon.Entities;

namespace Dungeon.Tests
{
    [TestFixture]
    public class StrategyTests
    {
        private static void DummyCallback() { }

        [Test]
        public void Enemy_ShouldChangeBehavior_WhenStrategyIsSwitched()
        {
            Player player = new Player { Name = "Hero", X = 15, Y = 5 };
            Ork ork = new Ork { X = 14, Y = 5 };
            Dungeon.Core.CombatFacade combat = new Dungeon.Core.CombatFacade();
            System.Action callback = DummyCallback;

            ork.SetStrategy(new MeleeAttack());
            ork.AttackStrategy.Execute(ork, player, player, combat, callback);

            int expectedFleeX = 13; 
            ork.SetStrategy(new FleeBehavior());
            ork.AttackStrategy.Execute(ork, player, player, combat, callback);

            if (ork.X != expectedFleeX)
            {
                throw new System.InvalidOperationException($"Орк не отступил. Ожидалось X={expectedFleeX}, но получили X={ork.X}");
            }
        }
    }
}