using System.Linq;
using NUnit.Framework;
using Project147.PlatformServices.Monetisation;

namespace Project147.Tests.EditMode.PlatformServices.Monetisation
{
    public sealed class Project147RewardedAdCatalogTests
    {
        [Test]
        public void CreateOpportunities_ReturnsExpectedRewardedAdPlacements()
        {
            var catalog = new Project147RewardedAdCatalog();

            var opportunities = catalog.CreateOpportunities();

            Assert.That(opportunities.Select(opportunity => opportunity.Id), Does.Contain("double-wave-clear-scrap"));
            Assert.That(opportunities.Select(opportunity => opportunity.Id), Does.Contain("emergency-base-repair"));
            Assert.That(opportunities.Select(opportunity => opportunity.Id), Does.Contain("pre-wave-supply-drop"));
        }

        [Test]
        public void CreateOpportunities_ReturnsUniqueIds()
        {
            var catalog = new Project147RewardedAdCatalog();

            var ids = catalog.CreateOpportunities().Select(opportunity => opportunity.Id).ToList();

            Assert.That(ids.Distinct().Count(), Is.EqualTo(ids.Count));
        }

        [Test]
        public void GetRequiredOpportunity_WhenIdExists_ReturnsOpportunity()
        {
            var catalog = new Project147RewardedAdCatalog();

            var opportunity = catalog.GetRequiredOpportunity("emergency-base-repair");

            Assert.That(opportunity.Trigger, Is.EqualTo(RewardedAdOpportunityTrigger.AfterDefeat));
            Assert.That(opportunity.MaxOffersPerRun, Is.EqualTo(1));
        }
    }
}
