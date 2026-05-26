using System;
using System.Collections.Generic;
using NUnit.Framework;
using Project147.PlatformServices.Analytics;

namespace Project147.Tests.EditMode.PlatformServices.Analytics
{
    public sealed class AnalyticsEventRecordTests
    {
        [Test]
        public void Constructor_WhenValuesAreValid_StoresValues()
        {
            var record = new AnalyticsEventRecord(
                "level_started",
                new Dictionary<string, string> { { "level_id", "debug-relay-yard" } });

            Assert.That(record.Name, Is.EqualTo("level_started"));
            Assert.That(record.Properties["level_id"], Is.EqualTo("debug-relay-yard"));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void Constructor_WhenNameIsMissing_Throws(string name)
        {
            Assert.Throws<ArgumentException>(() => new AnalyticsEventRecord(
                name,
                new Dictionary<string, string>()));
        }

        [Test]
        public void Constructor_WhenPropertiesAreNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new AnalyticsEventRecord("level_started", null));
        }

        [Test]
        public void Constructor_WhenPropertyNameIsBlank_Throws()
        {
            Assert.Throws<ArgumentException>(() => new AnalyticsEventRecord(
                "level_started",
                new Dictionary<string, string> { { "", "value" } }));
        }
    }
}
