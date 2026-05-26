using System;
using NUnit.Framework;
using Project147.PlatformServices.Analytics;

namespace Project147.Tests.EditMode.PlatformServices.Analytics
{
    public sealed class AnalyticsEventDefinitionTests
    {
        [Test]
        public void Constructor_WhenValuesAreValid_StoresValues()
        {
            var definition = new AnalyticsEventDefinition(
                "level_started",
                new[] { "level_id", "loadout_id" });

            Assert.That(definition.Name, Is.EqualTo("level_started"));
            Assert.That(definition.RequiredProperties, Is.EqualTo(new[] { "level_id", "loadout_id" }));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void Constructor_WhenNameIsMissing_Throws(string name)
        {
            Assert.Throws<ArgumentException>(() => new AnalyticsEventDefinition(name, new[] { "level_id" }));
        }

        [Test]
        public void Constructor_WhenPropertiesAreNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new AnalyticsEventDefinition("level_started", null));
        }

        [Test]
        public void Constructor_WhenPropertyIsBlank_Throws()
        {
            Assert.Throws<ArgumentException>(() => new AnalyticsEventDefinition("level_started", new[] { "level_id", "" }));
        }

        [Test]
        public void Constructor_WhenPropertyIsRepeated_Throws()
        {
            Assert.Throws<ArgumentException>(() => new AnalyticsEventDefinition("level_started", new[] { "level_id", "level_id" }));
        }
    }
}
