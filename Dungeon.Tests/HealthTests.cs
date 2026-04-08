using Xunit;
using Dungeon.Logic;

namespace Dungeon.Tests
{
    public class HealthTests
    {
        //1. Позитивный сценарий: Урон уменьшает здоровье
        [Fact]
        public void TakeDamage_ShouldReduceHealth()
        {
            //Arrange
            var health = new Health(100);

            //Act
            health.TakeDamage(30);

            //Assert
            Assert.Equal(70, health.Current);
        }

        //2. Граничный случай: Здоровье не падает ниже нуля
        [Fact]
        public void TakeDamage_MoreThanMax_ShouldStayAtZero()
        {
            //Arrange
            var health = new Health(100);

            //Act
            health.TakeDamage(150);

            //Assert
            Assert.Equal(0, health.Current);
        }

        //3. Позитивный сценарий: Лечение работает
        [Fact]
        public void Heal_ShouldIncreaseHealth()
        {
            //Arrange
            var health = new Health(100);
            health.TakeDamage(50);

            //Act
            health.Heal(20);

            //Assert
            Assert.Equal(70, health.Current);
        }

        //4. Негативный сценарий: Нельзя вылечить мертвого
        [Fact]
        public void Heal_WhenDead_ShouldDoNothing()
        {
            //Arrange
            var health = new Health(100);
            health.TakeDamage(100);

            //Act
            health.Heal(50);

            //Assert
            Assert.Equal(0, health.Current);
            Assert.True(health.IsDead);
        }

        //5. Граничный случай: Лечение не превышает максимум
        [Fact]
        public void Heal_MoreThanMax_ShouldStayAtMax()
        {
            // Arrange
            var health = new Health(100);
            health.TakeDamage(10);

            // Act
            health.Heal(50);

            // Assert
            Assert.Equal(100, health.Current);
        }
    }
}