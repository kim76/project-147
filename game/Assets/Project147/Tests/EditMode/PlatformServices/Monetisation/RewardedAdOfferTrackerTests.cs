using System;
using NUnit.Framework;
using Project147.PlatformServices.Monetisation;

namespace Project147.Tests.EditMode.PlatformServices.Monetisation
{
    public sealed class RewardedAdOfferTrackerTests
    {
        [Test]
        public void CanOffer_WhenUnderLimit_ReturnsTrue()
        {
            var tracker = new RewardedAdOfferTracker();
            var opportunity = CreateOpportunity(maxOffersPerRun: 2);

            Assert.That(tracker.CanOffer(opportunity), Is.True);
        }

        [Test]
        public void RecordOffer_ReturnsTrackerWithIncrementedCount()
        {
            var tracker = new RewardedAdOfferTracker();
            var opportunity = CreateOpportunity(maxOffersPerRun: 2);

            var result = tracker.RecordOffer(opportunity);

            Assert.That(result.GetOfferCount(opportunity.Id), Is.EqualTo(1));
            Assert.That(result.TotalOffers, Is.EqualTo(1));
            Assert.That(tracker.GetOfferCount(opportunity.Id), Is.EqualTo(0));
            Assert.That(tracker.TotalOffers, Is.EqualTo(0));
        }

        [Test]
        public void CanOffer_WhenLimitIsReached_ReturnsFalse()
        {
            var opportunity = CreateOpportunity(maxOffersPerRun: 1);
            var tracker = new RewardedAdOfferTracker().RecordOffer(opportunity);

            Assert.That(tracker.CanOffer(opportunity), Is.False);
        }

        [Test]
        public void RecordOffer_WhenLimitIsReached_Throws()
        {
            var opportunity = CreateOpportunity(maxOffersPerRun: 1);
            var tracker = new RewardedAdOfferTracker().RecordOffer(opportunity);

            Assert.Throws<InvalidOperationException>(() => tracker.RecordOffer(opportunity));
        }

        [Test]
        public void CanOffer_WhenOpportunityIsNull_Throws()
        {
            var tracker = new RewardedAdOfferTracker();

            Assert.Throws<ArgumentNullException>(() => tracker.CanOffer(null));
        }

        [Test]
        public void RecordOffer_WhenOpportunityIsNull_Throws()
        {
            var tracker = new RewardedAdOfferTracker();

            Assert.Throws<ArgumentNullException>(() => tracker.RecordOffer(null));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void GetOfferCount_WhenIdIsMissing_Throws(string opportunityId)
        {
            var tracker = new RewardedAdOfferTracker();

            Assert.Throws<ArgumentException>(() => tracker.GetOfferCount(opportunityId));
        }

        private static RewardedAdOpportunityDefinition CreateOpportunity(int maxOffersPerRun)
        {
            return new RewardedAdOpportunityDefinition(
                "double-wave-clear-scrap",
                RewardedAdOpportunityTrigger.AfterWaveClear,
                "Double the scrap reward from the cleared wave.",
                maxOffersPerRun);
        }
    }
}
