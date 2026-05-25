using System;
using NUnit.Framework;
using Project147.GameCore.Grid;
using Project147.GameCore.Level;

namespace Project147.Tests.EditMode.GameCore.Level
{
    public sealed class LevelLayoutDefinitionTests
    {
        [Test]
        public void Constructor_WhenValuesAreValid_CreatesLayout()
        {
            var layout = new LevelLayoutDefinition(
                "relay-yard",
                8,
                5,
                new GridCoordinate(0, 2),
                new GridCoordinate(7, 2),
                new[]
                {
                    new GridCoordinate(2, 1),
                    new GridCoordinate(2, 1),
                    new GridCoordinate(3, 3)
                });

            Assert.That(layout.Id, Is.EqualTo("relay-yard"));
            Assert.That(layout.Width, Is.EqualTo(8));
            Assert.That(layout.Height, Is.EqualTo(5));
            Assert.That(layout.Spawn, Is.EqualTo(new GridCoordinate(0, 2)));
            Assert.That(layout.Goal, Is.EqualTo(new GridCoordinate(7, 2)));
            Assert.That(layout.BlockedCells, Has.Count.EqualTo(2));
        }

        [Test]
        public void Constructor_WhenIdIsMissing_Throws()
        {
            Assert.Throws<ArgumentException>(() => new LevelLayoutDefinition(
                "",
                8,
                5,
                new GridCoordinate(0, 2),
                new GridCoordinate(7, 2),
                Array.Empty<GridCoordinate>()));
        }

        [TestCase(0, 5)]
        [TestCase(8, 0)]
        public void Constructor_WhenSizeIsInvalid_Throws(int width, int height)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new LevelLayoutDefinition(
                "relay-yard",
                width,
                height,
                new GridCoordinate(0, 0),
                new GridCoordinate(0, 0),
                Array.Empty<GridCoordinate>()));
        }

        [Test]
        public void Constructor_WhenBlockedCellsAreNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new LevelLayoutDefinition(
                "relay-yard",
                8,
                5,
                new GridCoordinate(0, 2),
                new GridCoordinate(7, 2),
                null));
        }

        [Test]
        public void Constructor_WhenSpawnIsOutsideLayout_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new LevelLayoutDefinition(
                "relay-yard",
                8,
                5,
                new GridCoordinate(-1, 2),
                new GridCoordinate(7, 2),
                Array.Empty<GridCoordinate>()));
        }

        [Test]
        public void Constructor_WhenGoalIsOutsideLayout_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new LevelLayoutDefinition(
                "relay-yard",
                8,
                5,
                new GridCoordinate(0, 2),
                new GridCoordinate(8, 2),
                Array.Empty<GridCoordinate>()));
        }

        [Test]
        public void Constructor_WhenBlockedCellIsSpawn_Throws()
        {
            Assert.Throws<ArgumentException>(() => new LevelLayoutDefinition(
                "relay-yard",
                8,
                5,
                new GridCoordinate(0, 2),
                new GridCoordinate(7, 2),
                new[] { new GridCoordinate(0, 2) }));
        }

        [Test]
        public void Constructor_WhenBlockedCellIsGoal_Throws()
        {
            Assert.Throws<ArgumentException>(() => new LevelLayoutDefinition(
                "relay-yard",
                8,
                5,
                new GridCoordinate(0, 2),
                new GridCoordinate(7, 2),
                new[] { new GridCoordinate(7, 2) }));
        }
    }
}
