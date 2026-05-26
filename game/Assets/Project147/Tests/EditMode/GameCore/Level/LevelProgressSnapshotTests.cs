using System;
using NUnit.Framework;
using Project147.GameCore.Level;

namespace Project147.Tests.EditMode.GameCore.Level
{
    public sealed class LevelProgressSnapshotTests
    {
        [Test]
        public void Restore_ReturnsLevelProgressState()
        {
            var snapshot = new LevelProgressSnapshot("debug-relay-yard", 2, 1, 3, 5, 4);

            var progress = snapshot.Restore();

            Assert.That(progress.RunsCompleted, Is.EqualTo(2));
            Assert.That(progress.Victories, Is.EqualTo(1));
            Assert.That(progress.BestStars, Is.EqualTo(3));
            Assert.That(progress.BestWavesCleared, Is.EqualTo(5));
            Assert.That(progress.BestPerfectWaves, Is.EqualTo(4));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void Constructor_WhenLevelIdIsMissing_Throws(string levelId)
        {
            Assert.Throws<ArgumentException>(() => new LevelProgressSnapshot(levelId, 0, 0, 0, 0, 0));
        }
    }
}
