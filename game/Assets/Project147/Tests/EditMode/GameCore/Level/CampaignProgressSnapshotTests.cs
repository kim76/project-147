using System;
using System.Linq;
using NUnit.Framework;
using Project147.GameCore.Level;

namespace Project147.Tests.EditMode.GameCore.Level
{
    public sealed class CampaignProgressSnapshotTests
    {
        [Test]
        public void Capture_ReturnsSnapshotForTrackedLevelProgress()
        {
            var campaign = new CampaignProgressState()
                .ApplyRunSummary("debug-relay-yard", CreateVictory(stars: 3))
                .Campaign
                .ApplyRunSummary("debug-switchback", CreateVictory(stars: 2))
                .Campaign;

            var snapshot = CampaignProgressSnapshot.Capture(campaign);

            Assert.That(snapshot.Levels.Count, Is.EqualTo(2));
            Assert.That(snapshot.Levels.All(level => level.RunsCompleted == 1), Is.True);
        }

        [Test]
        public void Restore_ReturnsCampaignWithSameProgress()
        {
            var snapshot = new CampaignProgressSnapshot(new[]
            {
                new LevelProgressSnapshot("debug-relay-yard", 2, 1, 3, 5, 4),
                new LevelProgressSnapshot("debug-switchback", 1, 0, 0, 2, 1)
            });

            var campaign = snapshot.Restore();

            Assert.That(campaign.TotalStars, Is.EqualTo(3));
            Assert.That(campaign.GetProgress("debug-relay-yard").RunsCompleted, Is.EqualTo(2));
            Assert.That(campaign.GetProgress("debug-relay-yard").BestPerfectWaves, Is.EqualTo(4));
            Assert.That(campaign.GetProgress("debug-switchback").BestWavesCleared, Is.EqualTo(2));
        }

        [Test]
        public void Constructor_WhenDuplicateLevels_Throws()
        {
            Assert.Throws<ArgumentException>(() => new CampaignProgressSnapshot(new[]
            {
                new LevelProgressSnapshot("debug-relay-yard", 1, 1, 3, 5, 5),
                new LevelProgressSnapshot("debug-relay-yard", 1, 1, 3, 5, 5)
            }));
        }

        [Test]
        public void Capture_WhenCampaignIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => CampaignProgressSnapshot.Capture(null));
        }

        private static RunSummaryState CreateVictory(int stars)
        {
            var summary = new RunSummaryState()
                .RecordWaveCleared(25, true)
                .RecordWaveCleared(25, true)
                .RecordWaveCleared(25, true);

            for (var leakIndex = 0; leakIndex < LeaksForStars(stars); leakIndex++)
            {
                summary = summary.RecordAlienLeaked();
            }

            return summary.Complete(RunOutcome.Victory);
        }

        private static int LeaksForStars(int stars)
        {
            switch (stars)
            {
                case 3:
                    return 0;
                case 2:
                    return 1;
                case 1:
                    return 3;
                default:
                    throw new ArgumentOutOfRangeException(nameof(stars), "Stars must be between one and three.");
            }
        }
    }
}
