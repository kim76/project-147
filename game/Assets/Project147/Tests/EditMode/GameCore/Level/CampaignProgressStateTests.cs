using System;
using NUnit.Framework;
using Project147.GameCore.Level;

namespace Project147.Tests.EditMode.GameCore.Level
{
    public sealed class CampaignProgressStateTests
    {
        [Test]
        public void GetProgress_WhenLevelHasNoProgress_ReturnsEmptyProgress()
        {
            var campaign = new CampaignProgressState();

            var progress = campaign.GetProgress("debug-relay-yard");

            Assert.That(progress.RunsCompleted, Is.EqualTo(0));
            Assert.That(progress.BestStars, Is.EqualTo(0));
        }

        [Test]
        public void ApplyRunSummary_RecordsProgressForLevel()
        {
            var campaign = new CampaignProgressState();
            var summary = CreateVictory();

            var result = campaign.ApplyRunSummary("debug-relay-yard", summary);

            var progress = result.Campaign.GetProgress("debug-relay-yard");
            Assert.That(progress.RunsCompleted, Is.EqualTo(1));
            Assert.That(progress.Victories, Is.EqualTo(1));
            Assert.That(progress.BestStars, Is.EqualTo(3));
            Assert.That(result.LevelResult.ImprovedStars, Is.True);
        }

        [Test]
        public void ApplyRunSummary_WhenDifferentLevel_DoesNotChangeOtherLevel()
        {
            var campaign = new CampaignProgressState()
                .ApplyRunSummary("debug-relay-yard", CreateVictory())
                .Campaign;

            var result = campaign.ApplyRunSummary("debug-switchback", CreateVictory());

            Assert.That(result.Campaign.GetProgress("debug-relay-yard").RunsCompleted, Is.EqualTo(1));
            Assert.That(result.Campaign.GetProgress("debug-switchback").RunsCompleted, Is.EqualTo(1));
            Assert.That(campaign.GetProgress("debug-switchback").RunsCompleted, Is.EqualTo(0));
        }

        [Test]
        public void GetProgress_WhenLevelIdIsMissing_Throws()
        {
            var campaign = new CampaignProgressState();

            Assert.Throws<ArgumentException>(() => campaign.GetProgress(""));
        }

        [Test]
        public void ApplyRunSummary_WhenLevelIdIsMissing_Throws()
        {
            var campaign = new CampaignProgressState();

            Assert.Throws<ArgumentException>(() => campaign.ApplyRunSummary("", CreateVictory()));
        }

        [Test]
        public void ApplyRunSummary_WhenSummaryIsNull_Throws()
        {
            var campaign = new CampaignProgressState();

            Assert.Throws<ArgumentNullException>(() => campaign.ApplyRunSummary("debug-relay-yard", null));
        }

        private static RunSummaryState CreateVictory()
        {
            return new RunSummaryState()
                .RecordWaveCleared(25, true)
                .RecordWaveCleared(25, true)
                .RecordWaveCleared(25, true)
                .Complete(RunOutcome.Victory);
        }
    }
}
