using System;
using NUnit.Framework;
using Project147.GameCore.Level;

namespace Project147.Tests.EditMode.GameCore.Level
{
    public sealed class CampaignLevelUnlockStateTests
    {
        [Test]
        public void Constructor_UnlocksLevelsWhenCampaignHasRequiredStars()
        {
            var campaign = CreateCampaignWithStars(3);
            var rules = new[]
            {
                new LevelUnlockRule("debug-relay-yard", 0),
                new LevelUnlockRule("debug-switchback", 2),
                new LevelUnlockRule("debug-twin-lane", 4)
            };

            var unlocks = new CampaignLevelUnlockState(rules, campaign);

            Assert.That(unlocks.IsUnlocked("debug-relay-yard"), Is.True);
            Assert.That(unlocks.IsUnlocked("debug-switchback"), Is.True);
            Assert.That(unlocks.IsUnlocked("debug-twin-lane"), Is.False);
            Assert.That(unlocks.UnlockedLevelIds.Count, Is.EqualTo(2));
        }

        [Test]
        public void Constructor_WhenRulesAreEmpty_Throws()
        {
            Assert.Throws<ArgumentException>(() => new CampaignLevelUnlockState(
                Array.Empty<LevelUnlockRule>(),
                new CampaignProgressState()));
        }

        [Test]
        public void Constructor_WhenRulesContainDuplicateLevelIds_Throws()
        {
            Assert.Throws<ArgumentException>(() => new CampaignLevelUnlockState(
                new[]
                {
                    new LevelUnlockRule("debug-relay-yard", 0),
                    new LevelUnlockRule("debug-relay-yard", 1)
                },
                new CampaignProgressState()));
        }

        [Test]
        public void IsUnlocked_WhenLevelIdIsMissing_Throws()
        {
            var unlocks = new CampaignLevelUnlockState(
                new[] { new LevelUnlockRule("debug-relay-yard", 0) },
                new CampaignProgressState());

            Assert.Throws<ArgumentException>(() => unlocks.IsUnlocked(""));
        }

        private static CampaignProgressState CreateCampaignWithStars(int stars)
        {
            return new CampaignProgressState()
                .ApplyRunSummary("debug-relay-yard", CreateVictory(stars))
                .Campaign;
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
