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
            Assert.That(wave.SpawnEntries, Has.Count.EqualTo(6));
            Assert.That(wave.SecondsBetweenSpawns, Is.EqualTo(0.75f));
            Assert.That(wave.ClearReward, Is.EqualTo(25));
        }

        [Test]
        public void Constructor_WhenCompositionIsValid_StoresCompositionSpawnEntries()
        {
            var composition = new WaveComposition(new[]
            {
                new WaveSpawnGroup("basic", 2),
                new WaveSpawnGroup("fast", 1)
            });

            var wave = new WaveDefinition(composition, 0.75f, 25);

            Assert.That(wave.AlienCount, Is.EqualTo(3));
            Assert.That(wave.SpawnEntries[0].AlienId, Is.EqualTo("basic"));
            Assert.That(wave.SpawnEntries[1].AlienId, Is.EqualTo("fast"));
            Assert.That(wave.SpawnEntries[2].AlienId, Is.EqualTo("basic"));
        }

        [Test]
        public void Constructor_WhenCompositionIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new WaveDefinition(null, 0.75f, 25));
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
