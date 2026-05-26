using System;
using System.Collections.Generic;
using NUnit.Framework;
using Project147.PlatformServices.Analytics;

namespace Project147.Tests.EditMode.PlatformServices.Analytics
{
    public sealed class InMemoryAnalyticsRecorderTests
    {
        [Test]
        public void Track_WhenEventIsKnownAndPropertiesArePresent_RecordsEvent()
        {
            var recorder = CreateRecorder();

            recorder.Track(
                "level_started",
                new Dictionary<string, string>
                {
                    { "level_id", "debug-relay-yard" },
                    { "loadout_id", "debug-loadout-balanced" }
                });

            Assert.That(recorder.Records, Has.Count.EqualTo(1));
            Assert.That(recorder.Records[0].Name, Is.EqualTo("level_started"));
        }

        [Test]
        public void Constructor_WhenDefinitionsAreNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new InMemoryAnalyticsRecorder(null));
        }

        [Test]
        public void Constructor_WhenDefinitionsContainNull_Throws()
        {
            Assert.Throws<ArgumentException>(() => new InMemoryAnalyticsRecorder(
                new AnalyticsEventDefinition[] { null }));
        }

        [Test]
        public void Constructor_WhenDefinitionsRepeatName_Throws()
        {
            Assert.Throws<ArgumentException>(() => new InMemoryAnalyticsRecorder(
                new[]
                {
                    new AnalyticsEventDefinition("level_started", Array.Empty<string>()),
                    new AnalyticsEventDefinition("level_started", Array.Empty<string>())
                }));
        }

        [Test]
        public void Track_WhenEventIsUnknown_Throws()
        {
            var recorder = CreateRecorder();

            Assert.Throws<ArgumentException>(() => recorder.Track(
                "unknown",
                new Dictionary<string, string>()));
        }

        [Test]
        public void Track_WhenPropertiesAreNull_Throws()
        {
            var recorder = CreateRecorder();

            Assert.Throws<ArgumentNullException>(() => recorder.Track("level_started", null));
        }

        [Test]
        public void Track_WhenRequiredPropertyIsMissing_Throws()
        {
            var recorder = CreateRecorder();

            Assert.Throws<ArgumentException>(() => recorder.Track(
                "level_started",
                new Dictionary<string, string> { { "level_id", "debug-relay-yard" } }));
        }

        private static InMemoryAnalyticsRecorder CreateRecorder()
        {
            return new InMemoryAnalyticsRecorder(new Project147AnalyticsCatalog().CreateGameplayEvents());
        }
    }
}
