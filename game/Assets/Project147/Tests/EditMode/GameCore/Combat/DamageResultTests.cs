using System;
using NUnit.Framework;
using Project147.GameCore.Combat;

namespace Project147.Tests.EditMode.GameCore.Combat
{
    public sealed class DamageResultTests
    {
        [Test]
        public void Constructor_WhenValuesAreValid_StoresValues()
        {
            var result = new DamageResult(100, 0.25f, 75);

            Assert.That(result.BaseAmount, Is.EqualTo(100));
            Assert.That(result.Resistance, Is.EqualTo(0.25f));
            Assert.That(result.FinalAmount, Is.EqualTo(75));
            Assert.That(result.WasCritical, Is.False);
            Assert.That(result.WasDodged, Is.False);
        }

        [Test]
        public void Constructor_WhenOutcomeFlagsAreProvided_StoresValues()
        {
            var result = new DamageResult(100, 0, 200, true, false);

            Assert.That(result.WasCritical, Is.True);
            Assert.That(result.WasDodged, Is.False);
        }

        [Test]
        public void Constructor_WhenBaseAmountIsNegative_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new DamageResult(-1, 0, 0));
        }

        [TestCase(-0.01f)]
        [TestCase(1.01f)]
        public void Constructor_WhenResistanceIsOutsideZeroToOne_Throws(float resistance)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new DamageResult(100, resistance, 75));
        }

        [Test]
        public void Constructor_WhenFinalAmountIsNegative_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new DamageResult(100, 0, -1));
        }

        [Test]
        public void Constructor_WhenDodgedResultHasFinalDamage_Throws()
        {
            Assert.Throws<ArgumentException>(() => new DamageResult(100, 0, 1, false, true));
        }

        [Test]
        public void Constructor_WhenResultIsBothCriticalAndDodged_Throws()
        {
            Assert.Throws<ArgumentException>(() => new DamageResult(100, 0, 0, true, true));
        }
    }
}
