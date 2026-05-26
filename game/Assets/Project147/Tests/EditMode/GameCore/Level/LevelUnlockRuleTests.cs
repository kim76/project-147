using System;
using NUnit.Framework;
using Project147.GameCore.Level;

namespace Project147.Tests.EditMode.GameCore.Level
{
    public sealed class LevelUnlockRuleTests
    {
        [Test]
        public void IsUnlocked_WhenCampaignHasEnoughStars_ReturnsTrue()
        {
            var rule = new LevelUnlockRule("debug-switchback", 2);
            var campaign = CreateCampaignWithStars(3);

            Assert.That(rule.IsUnlocked(campaign), Is.True);
        }

        [Test]
        public void IsUnlocked_WhenCampaignDoesNotHaveEnoughStars_ReturnsFalse()
        {
            var rule = new LevelUnlockRule("debug-switchback", 2);
            var campaign = CreateCampaignWithStars(1);

            Assert.That(rule.IsUnlocked(campaign), Is.False);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void Constructor_WhenLevelIdIsMissing_Throws(string levelId)
        {
            Assert.Throws<ArgumentException>(() => new LevelUnlockRule(levelId, 0));
        }

        [Test]
        public void Constructor_WhenRequiredStarsIsNegative_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new LevelUnlockRule("debug-switchback", -1));
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
