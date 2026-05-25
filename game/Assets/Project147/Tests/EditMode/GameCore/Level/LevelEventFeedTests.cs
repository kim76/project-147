using System;
using NUnit.Framework;
using Project147.GameCore.Level;

namespace Project147.Tests.EditMode.GameCore.Level
{
    public sealed class LevelEventFeedTests
    {
        [Test]
        public void Constructor_WhenCapacityIsValid_StartsEmpty()
        {
            var feed = new LevelEventFeed(3);

            Assert.That(feed.Capacity, Is.EqualTo(3));
            Assert.That(feed.Entries, Is.Empty);
        }

        [Test]
        public void Constructor_WhenCapacityIsZero_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new LevelEventFeed(0));
        }

        [Test]
        public void Add_WhenMessageIsMissing_Throws()
        {
            var feed = new LevelEventFeed(3);

            Assert.Throws<ArgumentException>(() => feed.Add("   "));
        }

        [Test]
        public void Add_ReturnsNewFeedWithMessageAppended()
        {
            var feed = new LevelEventFeed(3);

            var result = feed.Add("Wave started");

            Assert.That(result.Entries, Is.EqualTo(new[] { "Wave started" }));
            Assert.That(feed.Entries, Is.Empty);
        }

        [Test]
        public void Add_WhenCapacityIsExceeded_DropsOldestMessage()
        {
            var feed = new LevelEventFeed(2)
                .Add("First")
                .Add("Second");

            var result = feed.Add("Third");

            Assert.That(result.Entries, Is.EqualTo(new[] { "Second", "Third" }));
        }

        [Test]
        public void Clear_ReturnsEmptyFeedWithSameCapacity()
        {
            var feed = new LevelEventFeed(2).Add("First");

            var result = feed.Clear();

            Assert.That(result.Capacity, Is.EqualTo(2));
            Assert.That(result.Entries, Is.Empty);
        }
    }
}
