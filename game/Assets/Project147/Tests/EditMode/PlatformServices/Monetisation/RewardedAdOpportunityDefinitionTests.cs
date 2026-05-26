using System;
using NUnit.Framework;
using Project147.PlatformServices.Monetisation;

namespace Project147.Tests.EditMode.PlatformServices.Monetisation
{
    public sealed class RewardedAdOpportunityDefinitionTests
    {
        [Test]
        public void Constructor_WhenValuesAreValid_StoresValues()
        {
            var definition = new RewardedAdOpportunityDefinition(
                "double-wave-clear-scrap",
                RewardedAdOpportunityTrigger.AfterWaveClear,
                "Double the scrap reward from the cleared wave.",
                2);

            Assert.That(definition.Id, Is.EqualTo("double-wave-clear-scrap"));
            Assert.That(definition.Trigger, Is.EqualTo(RewardedAdOpportunityTrigger.AfterWaveClear));
            Assert.That(definition.RewardDescription, Does.Contain("scrap"));
            Assert.That(definition.MaxOffersPerRun, Is.EqualTo(2));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void Constructor_WhenIdIsMissing_Throws(string id)
        {
            Assert.Throws<ArgumentException>(() => new RewardedAdOpportunityDefinition(
                id,
                RewardedAdOpportunityTrigger.AfterWaveClear,
                "Reward",
                1));
        }

        [Test]
        public void Constructor_WhenRewardDescriptionIsMissing_Throws()
        {
            Assert.Throws<ArgumentException>(() => new RewardedAdOpportunityDefinition(
                "double-wave-clear-scrap",
                RewardedAdOpportunityTrigger.AfterWaveClear,
                "",
                1));
        }

        [Test]
        public void Constructor_WhenMaxOffersPerRunIsZero_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new RewardedAdOpportunityDefinition(
                "double-wave-clear-scrap",
                RewardedAdOpportunityTrigger.AfterWaveClear,
                "Reward",
                0));
        }
    }
}
