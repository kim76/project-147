using System;
using NUnit.Framework;
using Project147.GameCore.Level;

namespace Project147.Tests.EditMode.GameCore.Level
{
    public sealed class BaseStateTests
    {
        [Test]
        public void Constructor_WhenHealthIsValid_StartsAtMaxHealth()
        {
            var state = new BaseState(10);

            Assert.That(state.MaxHealth, Is.EqualTo(10));
            Assert.That(state.CurrentHealth, Is.EqualTo(10));
            Assert.That(state.CurrentShield, Is.EqualTo(0));
            Assert.That(state.IsDestroyed, Is.False);
        }

        [Test]
        public void Constructor_WhenHealthIsZero_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new BaseState(0));
        }

        [Test]
        public void ApplyLeakDamage_ReducesHealthAndReturnsNewState()
        {
            var state = new BaseState(10);

            var result = state.ApplyLeakDamage(3);

            Assert.That(result.CurrentHealth, Is.EqualTo(7));
            Assert.That(state.CurrentHealth, Is.EqualTo(10));
        }

        [Test]
        public void ApplyLeakDamage_WhenDamageExceedsHealth_ClampsAtZero()
        {
            var state = new BaseState(10);

            var result = state.ApplyLeakDamage(12);

            Assert.That(result.CurrentHealth, Is.EqualTo(0));
            Assert.That(result.IsDestroyed, Is.True);
        }

        [Test]
        public void ApplyLeakDamage_WhenShielded_ReducesShieldBeforeHealth()
        {
            var state = new BaseState(10).AddShield(2);

            var result = state.ApplyLeakDamage(1);

            Assert.That(result.CurrentShield, Is.EqualTo(1));
            Assert.That(result.CurrentHealth, Is.EqualTo(10));
        }

        [Test]
        public void ApplyLeakDamage_WhenDamageExceedsShield_AppliesOverflowToHealth()
        {
            var state = new BaseState(10).AddShield(2);

            var result = state.ApplyLeakDamage(3);

            Assert.That(result.CurrentShield, Is.EqualTo(0));
            Assert.That(result.CurrentHealth, Is.EqualTo(9));
        }

        [Test]
        public void ApplyLeakDamage_WhenAmountIsNegative_Throws()
        {
            var state = new BaseState(10);

            Assert.Throws<ArgumentOutOfRangeException>(() => state.ApplyLeakDamage(-1));
        }

        [Test]
        public void Repair_IncreasesHealthAndReturnsNewState()
        {
            var state = new BaseState(10).ApplyLeakDamage(4);

            var result = state.Repair(2);

            Assert.That(result.CurrentHealth, Is.EqualTo(8));
            Assert.That(result.CurrentShield, Is.EqualTo(0));
            Assert.That(state.CurrentHealth, Is.EqualTo(6));
        }

        [Test]
        public void Repair_WhenAmountExceedsMissingHealth_ClampsAtMaxHealth()
        {
            var state = new BaseState(10).ApplyLeakDamage(2);

            var result = state.Repair(10);

            Assert.That(result.CurrentHealth, Is.EqualTo(10));
        }

        [Test]
        public void Repair_WhenAmountIsNegative_Throws()
        {
            var state = new BaseState(10);

            Assert.Throws<ArgumentOutOfRangeException>(() => state.Repair(-1));
        }

        [Test]
        public void AddShield_IncreasesShieldAndReturnsNewState()
        {
            var state = new BaseState(10);

            var result = state.AddShield(3);

            Assert.That(result.CurrentShield, Is.EqualTo(3));
            Assert.That(result.CurrentHealth, Is.EqualTo(10));
            Assert.That(state.CurrentShield, Is.EqualTo(0));
        }

        [Test]
        public void AddShield_WhenAmountIsNegative_Throws()
        {
            var state = new BaseState(10);

            Assert.Throws<ArgumentOutOfRangeException>(() => state.AddShield(-1));
        }
    }
}
