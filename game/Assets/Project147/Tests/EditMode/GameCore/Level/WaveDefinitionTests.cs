using System;
using NUnit.Framework;
using Project147.GameCore.Level;

namespace Project147.Tests.EditMode.GameCore.Level
{
    public sealed class WaveDefinitionTests
    {
        [Test]
        public void Constructor_WhenValuesAreValid_StoresValues()
        {
            var wave = new WaveDefinition(6, 0.75f, 25);

            Assert.That(wave.AlienCount, Is.EqualTo(6));
            Assert.That(wave.SecondsBetweenSpawns, Is.EqualTo(0.75f));
            Assert.That(wave.ClearReward, Is.EqualTo(25));
        }

        [Test]
        public void Constructor_WhenAlienCountIsZero_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new WaveDefinition(0, 0.75f, 25));
        }

        [Test]
        public void Constructor_WhenSecondsBetweenSpawnsIsZero_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new WaveDefinition(6, 0, 25));
        }

        [Test]
        public void Constructor_WhenClearRewardIsNegative_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new WaveDefinition(6, 0.75f, -1));
        }
    }
}
