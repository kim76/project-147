using System;
using System.Linq;
using NUnit.Framework;
using Project147.GameCore.Level;

namespace Project147.Tests.EditMode.GameCore.Level
{
    public sealed class WaveIntelBuilderTests
    {
        [Test]
        public void Build_ReturnsCountsAndTagsForMixedWave()
        {
            var builder = new WaveIntelBuilder();
            var wave = new WaveDefinition(
                new WaveComposition(new[]
                {
                    new WaveSpawnGroup("basic", 6),
                    new WaveSpawnGroup("fast", 2),
                    new WaveSpawnGroup("armoured", 2)
                }),
                0.5f,
                25);

            var summary = builder.Build(2, wave, "fast", "armoured");

            Assert.That(summary.WaveNumber, Is.EqualTo(3));
            Assert.That(summary.TotalAliens, Is.EqualTo(10));
            Assert.That(summary.ClearReward, Is.EqualTo(25));
            Assert.That(summary.Entries.Count, Is.EqualTo(3));
            Assert.That(summary.Tags.Contains("Mixed"), Is.True);
            Assert.That(summary.Tags.Contains("Fast"), Is.True);
            Assert.That(summary.Tags.Contains("Armoured"), Is.True);
            Assert.That(summary.Tags.Contains("Heavy"), Is.True);
            Assert.That(summary.ThreatRating, Is.EqualTo(3));
        }

        [Test]
        public void Build_WhenWaveOnlyHasBasicAliens_ReturnsNoSpecialTags()
        {
            var builder = new WaveIntelBuilder();
            var wave = new WaveDefinition(
                new WaveComposition(new[] { new WaveSpawnGroup("basic", 4) }),
                0.5f,
                25);

            var summary = builder.Build(0, wave, "fast", "armoured");

            Assert.That(summary.Tags.Count, Is.EqualTo(0));
            Assert.That(summary.ThreatRating, Is.EqualTo(1));
            Assert.That(summary.Entries.Single().AlienId, Is.EqualTo("basic"));
            Assert.That(summary.Entries.Single().Count, Is.EqualTo(4));
        }

        [Test]
        public void Build_WhenWaveContainsBoss_AddsBossTag()
        {
            var builder = new WaveIntelBuilder();
            var wave = new WaveDefinition(
                new WaveComposition(new[]
                {
                    new WaveSpawnGroup("basic", 6),
                    new WaveSpawnGroup("boss", 1)
                }),
                0.5f,
                25);

            var summary = builder.Build(4, wave, "fast", "armoured", "boss");

            Assert.That(summary.Tags.Contains("Boss"), Is.True);
            Assert.That(summary.Tags.Contains("Mixed"), Is.True);
            Assert.That(summary.ThreatRating, Is.EqualTo(5));
        }

        [Test]
        public void Build_WhenWaveContainsShieldedAlien_AddsShieldedTag()
        {
            var builder = new WaveIntelBuilder();
            var wave = new WaveDefinition(
                new WaveComposition(new[]
                {
                    new WaveSpawnGroup("basic", 6),
                    new WaveSpawnGroup("shielded", 2)
                }),
                0.5f,
                25);

            var summary = builder.Build(3, wave, "fast", "armoured", "boss", "shielded");

            Assert.That(summary.Tags.Contains("Shielded"), Is.True);
            Assert.That(summary.Tags.Contains("Mixed"), Is.True);
            Assert.That(summary.ThreatRating, Is.EqualTo(4));
        }

        [Test]
        public void Build_WhenWaveContainsBurrowerAlien_AddsBurrowerTag()
        {
            var builder = new WaveIntelBuilder();
            var wave = new WaveDefinition(
                new WaveComposition(new[]
                {
                    new WaveSpawnGroup("basic", 6),
                    new WaveSpawnGroup("burrower", 2)
                }),
                0.5f,
                25);

            var summary = builder.Build(3, wave, "fast", "armoured", "boss", "shielded", "burrower");

            Assert.That(summary.Tags.Contains("Burrower"), Is.True);
            Assert.That(summary.Tags.Contains("Mixed"), Is.True);
            Assert.That(summary.ThreatRating, Is.EqualTo(4));
        }

        [Test]
        public void Build_WhenWaveContainsRegeneratorAlien_AddsRegeneratorTag()
        {
            var builder = new WaveIntelBuilder();
            var wave = new WaveDefinition(
                new WaveComposition(new[]
                {
                    new WaveSpawnGroup("basic", 6),
                    new WaveSpawnGroup("regenerator", 2)
                }),
                0.5f,
                25);

            var summary = builder.Build(3, wave, "fast", "armoured", "boss", "shielded", "burrower", "regenerator");

            Assert.That(summary.Tags.Contains("Regenerator"), Is.True);
            Assert.That(summary.Tags.Contains("Mixed"), Is.True);
            Assert.That(summary.ThreatRating, Is.EqualTo(4));
        }

        [Test]
        public void Build_WhenCompletedWavesIsNegative_Throws()
        {
            var builder = new WaveIntelBuilder();

            Assert.Throws<ArgumentOutOfRangeException>(() => builder.Build(
                -1,
                new WaveDefinition(1, 0.5f, 25),
                "fast",
                "armoured"));
        }

        [Test]
        public void Build_WhenWaveIsNull_Throws()
        {
            var builder = new WaveIntelBuilder();

            Assert.Throws<ArgumentNullException>(() => builder.Build(0, null, "fast", "armoured"));
        }
    }
}
