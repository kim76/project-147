using System;
using System.Linq;
using NUnit.Framework;
using Project147.GameCore.Level;

namespace Project147.Tests.EditMode.GameCore.Level
{
    public sealed class LevelWaveTuningDefinitionTests
    {
        [Test]
        public void Constructor_WhenValuesAreValid_StoresValues()
        {
            var tuning = new LevelWaveTuningDefinition(5, 3, 0.7f, 30, 1, 2, 3, 2, 4);

            Assert.That(tuning.StartingWaveAlienCount, Is.EqualTo(5));
            Assert.That(tuning.ExtraAliensPerWave, Is.EqualTo(3));
            Assert.That(tuning.SecondsBetweenSpawns, Is.EqualTo(0.7f));
            Assert.That(tuning.WaveClearScrapReward, Is.EqualTo(30));
            Assert.That(tuning.RegeneratorStartWave, Is.EqualTo(4));
        }

        [Test]
        public void CreateWaveDefinition_WhenFirstWave_ReturnsBasicWave()
        {
            var tuning = new LevelWaveTuningDefinition(5, 3, 0.7f, 30);

            var wave = tuning.CreateWaveDefinition(0, 5, CreateRoster());

            Assert.That(wave.AlienCount, Is.EqualTo(5));
            Assert.That(wave.SecondsBetweenSpawns, Is.EqualTo(0.7f));
            Assert.That(wave.ClearReward, Is.EqualTo(30));
            Assert.That(wave.SpawnEntries.All(entry => entry.AlienId == "basic"), Is.True);
        }

        [Test]
        public void CreateWaveDefinition_WhenLaterWave_ReturnsMixedSpecialAliens()
        {
            var tuning = new LevelWaveTuningDefinition(5, 3, 0.7f, 30);

            var wave = tuning.CreateWaveDefinition(3, 5, CreateRoster());

            Assert.That(wave.AlienCount, Is.EqualTo(14));
            Assert.That(wave.SpawnEntries.Any(entry => entry.AlienId == "fast"), Is.True);
            Assert.That(wave.SpawnEntries.Any(entry => entry.AlienId == "armoured"), Is.True);
            Assert.That(wave.SpawnEntries.Any(entry => entry.AlienId == "shielded"), Is.True);
            Assert.That(wave.SpawnEntries.Any(entry => entry.AlienId == "burrower"), Is.True);
            Assert.That(wave.SpawnEntries.Any(entry => entry.AlienId == "regenerator"), Is.True);
        }

        [Test]
        public void CreateWaveDefinition_WhenFinalWave_IncludesBoss()
        {
            var tuning = new LevelWaveTuningDefinition(5, 3, 0.7f, 30);

            var wave = tuning.CreateWaveDefinition(4, 5, CreateRoster());

            Assert.That(wave.SpawnEntries.Count(entry => entry.AlienId == "boss"), Is.EqualTo(1));
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void Constructor_WhenStartingAlienCountIsInvalid_Throws(int startingAlienCount)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new LevelWaveTuningDefinition(
                startingAlienCount,
                2,
                0.8f,
                25));
        }

        [Test]
        public void Constructor_WhenExtraAliensPerWaveIsNegative_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new LevelWaveTuningDefinition(
                4,
                -1,
                0.8f,
                25));
        }

        [Test]
        public void Constructor_WhenSecondsBetweenSpawnsIsZero_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new LevelWaveTuningDefinition(
                4,
                2,
                0,
                25));
        }

        [Test]
        public void Constructor_WhenClearRewardIsNegative_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new LevelWaveTuningDefinition(
                4,
                2,
                0.8f,
                -1));
        }

        [Test]
        public void CreateWaveDefinition_WhenRosterIsNull_Throws()
        {
            var tuning = new LevelWaveTuningDefinition(5, 3, 0.7f, 30);

            Assert.Throws<ArgumentNullException>(() => tuning.CreateWaveDefinition(0, 5, null));
        }

        private static WaveAlienRoster CreateRoster()
        {
            return new WaveAlienRoster(
                "basic",
                "fast",
                "armoured",
                "shielded",
                "burrower",
                "regenerator",
                "boss");
        }
    }
}
