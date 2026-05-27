using System;
using NUnit.Framework;
using Project147.GameCore.Grid;
using Project147.GameCore.Level;

namespace Project147.Tests.EditMode.GameCore.Level
{
    public sealed class LevelRunDefinitionTests
    {
        [Test]
        public void Constructor_WhenValuesAreValid_CreatesDefinition()
        {
            var layout = CreateLayout();

            var definition = new LevelRunDefinition(layout, 175, 12, 6, 20);

            Assert.That(definition.Layout, Is.SameAs(layout));
            Assert.That(definition.StartingCurrency, Is.EqualTo(175));
            Assert.That(definition.BaseHealth, Is.EqualTo(12));
            Assert.That(definition.TotalWaves, Is.EqualTo(6));
            Assert.That(definition.PerfectWaveScrapBonus, Is.EqualTo(20));
            Assert.That(definition.WaveTuning, Is.Not.Null);
        }

        [Test]
        public void Constructor_WhenWaveTuningIsProvided_StoresWaveTuning()
        {
            var layout = CreateLayout();
            var tuning = new LevelWaveTuningDefinition(5, 3, 0.7f, 30);

            var definition = new LevelRunDefinition(layout, 175, 12, 6, 20, tuning);

            Assert.That(definition.WaveTuning, Is.SameAs(tuning));
        }

        [Test]
        public void CreateWaveDefinition_UsesConfiguredWaveTuning()
        {
            var tuning = new LevelWaveTuningDefinition(5, 3, 0.7f, 30);
            var definition = new LevelRunDefinition(CreateLayout(), 175, 12, 6, 20, tuning);

            var wave = definition.CreateWaveDefinition(1, CreateRoster());

            Assert.That(wave.AlienCount, Is.EqualTo(8));
            Assert.That(wave.SecondsBetweenSpawns, Is.EqualTo(0.7f));
            Assert.That(wave.ClearReward, Is.EqualTo(30));
        }

        [Test]
        public void Constructor_WhenLayoutIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new LevelRunDefinition(null, 100, 10, 5, 10));
        }

        [Test]
        public void Constructor_WhenStartingCurrencyIsNegative_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new LevelRunDefinition(CreateLayout(), -1, 10, 5, 10));
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void Constructor_WhenBaseHealthIsInvalid_Throws(int baseHealth)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new LevelRunDefinition(CreateLayout(), 100, baseHealth, 5, 10));
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void Constructor_WhenTotalWavesIsInvalid_Throws(int totalWaves)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new LevelRunDefinition(CreateLayout(), 100, 10, totalWaves, 10));
        }

        [Test]
        public void Constructor_WhenPerfectWaveScrapBonusIsNegative_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new LevelRunDefinition(CreateLayout(), 100, 10, 5, -1));
        }

        [Test]
        public void Constructor_WhenWaveTuningIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new LevelRunDefinition(
                CreateLayout(),
                100,
                10,
                5,
                10,
                null));
        }

        private static LevelLayoutDefinition CreateLayout()
        {
            return new LevelLayoutDefinition(
                "relay-yard",
                8,
                5,
                new GridCoordinate(0, 2),
                new GridCoordinate(7, 2),
                Array.Empty<GridCoordinate>());
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
