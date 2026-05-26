using System.Linq;
using NUnit.Framework;
using Project147.PlatformServices.Analytics;

namespace Project147.Tests.EditMode.PlatformServices.Analytics
{
    public sealed class Project147AnalyticsCatalogTests
    {
        [Test]
        public void CreateGameplayEvents_ReturnsExpectedCoreEvents()
        {
            var catalog = new Project147AnalyticsCatalog();

            var events = catalog.CreateGameplayEvents();

            Assert.That(events.Select(definition => definition.Name), Does.Contain("level_started"));
            Assert.That(events.Select(definition => definition.Name), Does.Contain("level_completed"));
            Assert.That(events.Select(definition => definition.Name), Does.Contain("ability_used"));
            Assert.That(events.Select(definition => definition.Name), Does.Contain("rewarded_ad_offer_shown"));
        }

        [Test]
        public void CreateGameplayEvents_ReturnsUniqueNames()
        {
            var catalog = new Project147AnalyticsCatalog();

            var names = catalog.CreateGameplayEvents().Select(definition => definition.Name).ToList();

            Assert.That(names.Distinct().Count(), Is.EqualTo(names.Count));
        }

        [Test]
        public void GetRequiredEvent_WhenNameExists_ReturnsEvent()
        {
            var catalog = new Project147AnalyticsCatalog();

            var definition = catalog.GetRequiredEvent("tower_upgraded");

            Assert.That(definition.RequiredProperties, Does.Contain("upgrade_id"));
            Assert.That(definition.RequiredProperties, Does.Contain("tower_level"));
        }
    }
}
