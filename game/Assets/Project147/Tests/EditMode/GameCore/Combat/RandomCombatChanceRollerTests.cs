using System;
using NUnit.Framework;
using Project147.GameCore.Combat;

namespace Project147.Tests.EditMode.GameCore.Combat
{
    public sealed class RandomCombatChanceRollerTests
    {
        [Test]
        public void Succeeds_WhenChanceIsZero_ReturnsFalse()
        {
            var roller = new RandomCombatChanceRoller();

            Assert.That(roller.Succeeds(0), Is.False);
        }

        [Test]
        public void Succeeds_WhenChanceIsOne_ReturnsTrue()
        {
            var roller = new RandomCombatChanceRoller();

            Assert.That(roller.Succeeds(1), Is.True);
        }

        [TestCase(-0.01f)]
        [TestCase(1.01f)]
        public void Succeeds_WhenChanceIsOutsideZeroToOne_Throws(float chance)
        {
            var roller = new RandomCombatChanceRoller();

            Assert.Throws<ArgumentOutOfRangeException>(() => roller.Succeeds(chance));
        }
    }
}
