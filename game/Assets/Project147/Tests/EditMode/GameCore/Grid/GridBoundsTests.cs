using System;
using System.Collections.Generic;
using NUnit.Framework;
using Project147.GameCore.Grid;

namespace Project147.Tests.EditMode.GameCore.Grid
{
    public sealed class GridBoundsTests
    {
        [Test]
        public void Constructor_WhenWidthIsZero_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new GridBounds(0, 5));
        }

        [Test]
        public void Constructor_WhenHeightIsZero_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new GridBounds(5, 0));
        }

        [Test]
        public void Contains_WhenCoordinateIsInsideBounds_ReturnsTrue()
        {
            var bounds = new GridBounds(4, 3);

            Assert.That(bounds.Contains(new GridCoordinate(3, 2)), Is.True);
        }

        [Test]
        public void Contains_WhenCoordinateIsOutsideBounds_ReturnsFalse()
        {
            var bounds = new GridBounds(4, 3);

            Assert.That(bounds.Contains(new GridCoordinate(4, 2)), Is.False);
            Assert.That(bounds.Contains(new GridCoordinate(3, 3)), Is.False);
            Assert.That(bounds.Contains(new GridCoordinate(-1, 0)), Is.False);
        }

        [Test]
        public void Coordinates_ReturnsEveryCoordinateInRowOrder()
        {
            var bounds = new GridBounds(2, 2);

            var result = new List<GridCoordinate>(bounds.Coordinates());

            Assert.That(result, Is.EqualTo(new[]
            {
                new GridCoordinate(0, 0),
                new GridCoordinate(1, 0),
                new GridCoordinate(0, 1),
                new GridCoordinate(1, 1)
            }));
        }
    }
}

