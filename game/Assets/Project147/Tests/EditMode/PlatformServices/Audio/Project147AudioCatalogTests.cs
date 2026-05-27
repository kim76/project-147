using System.Linq;
using NUnit.Framework;
using Project147.PlatformServices.Audio;

namespace Project147.Tests.EditMode.PlatformServices.Audio
{
    public sealed class Project147AudioCatalogTests
    {
        [Test]
        public void CreateGameplayEvents_ReturnsExpectedEvents()
        {
            var catalog = new Project147AudioCatalog();

            var events = catalog.CreateGameplayEvents();

            Assert.That(events.Select(definition => definition.Id), Does.Contain(Project147AudioEvents.TowerPlaced));
            Assert.That(events.Select(definition => definition.Id), Does.Contain(Project147AudioEvents.AbilityFreezePulse));
            Assert.That(events.Select(definition => definition.Id), Does.Contain(Project147AudioEvents.AbilityTowerOvercharge));
            Assert.That(events.Select(definition => definition.Id), Does.Contain(Project147AudioEvents.RunVictory));
        }

        [Test]
        public void CreateGameplayEvents_ReturnsUniqueIds()
        {
            var catalog = new Project147AudioCatalog();

            var ids = catalog.CreateGameplayEvents().Select(definition => definition.Id).ToList();

            Assert.That(ids.Distinct().Count(), Is.EqualTo(ids.Count));
        }

        [Test]
        public void GetRequiredEvent_WhenIdExists_ReturnsEvent()
        {
            var catalog = new Project147AudioCatalog();

            var definition = catalog.GetRequiredEvent(Project147AudioEvents.AbilityOrbitalStrike);

            Assert.That(definition.DefaultVolume, Is.GreaterThan(0));
        }
    }
}
