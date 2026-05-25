using System;
using NUnit.Framework;
using Project147.GameCore.Level;

namespace Project147.Tests.EditMode.GameCore.Level
{
    public sealed class WaveCompositionTests
    {
        [Test]
        public void Constructor_WhenGroupsAreValid_StoresGroupsAndTotalCount()
        {
            var composition = new WaveComposition(new[]
            {
                new WaveSpawnGroup("basic", 3),
                new WaveSpawnGroup("fast", 2)
            });

            Assert.That(composition.Groups, Has.Count.EqualTo(2));
            Assert.That(composition.TotalCount, Is.EqualTo(5));
        }

        [Test]
        public void Constructor_WhenGroupsAreNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new WaveComposition(null));
        }

        [Test]
        public void Constructor_WhenGroupsAreEmpty_Throws()
        {
            Assert.Throws<ArgumentException>(() => new WaveComposition(Array.Empty<WaveSpawnGroup>()));
        }

        [Test]
        public void Constructor_WhenGroupIsDefaultValue_Throws()
        {
            Assert.Throws<ArgumentException>(() => new WaveComposition(new[] { default(WaveSpawnGroup) }));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void SpawnGroupConstructor_WhenAlienIdIsMissing_Throws(string alienId)
        {
            Assert.Throws<ArgumentException>(() => new WaveSpawnGroup(alienId, 1));
        }

        [Test]
        public void SpawnGroupConstructor_WhenCountIsZero_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new WaveSpawnGroup("basic", 0));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void SpawnEntryConstructor_WhenAlienIdIsMissing_Throws(string alienId)
        {
            Assert.Throws<ArgumentException>(() => new WaveSpawnEntry(alienId));
        }

        [Test]
        public void BuildSpawnEntries_WhenMultipleGroups_InterleavesGroups()
        {
            var composition = new WaveComposition(new[]
            {
                new WaveSpawnGroup("basic", 3),
                new WaveSpawnGroup("fast", 2),
                new WaveSpawnGroup("armoured", 1)
            });

            var entries = composition.BuildSpawnEntries();

            Assert.That(entries, Has.Count.EqualTo(6));
            Assert.That(entries[0].AlienId, Is.EqualTo("basic"));
            Assert.That(entries[1].AlienId, Is.EqualTo("fast"));
            Assert.That(entries[2].AlienId, Is.EqualTo("armoured"));
            Assert.That(entries[3].AlienId, Is.EqualTo("basic"));
            Assert.That(entries[4].AlienId, Is.EqualTo("fast"));
            Assert.That(entries[5].AlienId, Is.EqualTo("basic"));
        }
    }
}
