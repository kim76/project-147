using System;
using NUnit.Framework;
using Project147.GameCore.Grid;

namespace Project147.Tests.EditMode.GameCore.Grid
{
    public sealed class TacticalGridTests
    {
        [Test]
        public void IsWalkable_WhenCellIsNotBlocked_ReturnsTrue()
        {
            var grid = new TacticalGrid(new GridBounds(3, 3));

            Assert.That(grid.IsWalkable(new GridCoordinate(1, 1)), Is.True);
        }

        [Test]
        public void Constructor_WhenBlockedCellIsOutsideBounds_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new TacticalGrid(
                new GridBounds(3, 3),
                new[] { new GridCoordinate(3, 0) }));
        }

        [Test]
        public void WithBlockedCell_ReturnsNewGridWithCellBlocked()
        {
            var grid = new TacticalGrid(new GridBounds(3, 3));

            var result = grid.WithBlockedCell(new GridCoordinate(1, 1));

            Assert.That(result.IsBlocked(new GridCoordinate(1, 1)), Is.True);
            Assert.That(grid.IsWalkable(new GridCoordinate(1, 1)), Is.True);
        }

        [Test]
        public void WithOpenCell_ReturnsNewGridWithCellOpen()
        {
            var blocked = new GridCoordinate(1, 1);
            var grid = new TacticalGrid(new GridBounds(3, 3), new[] { blocked });

            var result = grid.WithOpenCell(blocked);

            Assert.That(result.IsWalkable(blocked), Is.True);
            Assert.That(grid.IsBlocked(blocked), Is.True);
        }

        [Test]
        public void WalkableOrthogonalNeighbours_ExcludesBlockedAndOutOfBoundsCells()
        {
            var grid = new TacticalGrid(
                new GridBounds(3, 3),
                new[] { new GridCoordinate(1, 0) });

            var result = grid.WalkableOrthogonalNeighbours(new GridCoordinate(0, 0));

            Assert.That(result, Is.EqualTo(new[]
            {
                new GridCoordinate(0, 1)
            }));
        }

        [Test]
        public void IsWalkable_WhenCoordinateIsOutsideBounds_Throws()
        {
            var grid = new TacticalGrid(new GridBounds(3, 3));

            Assert.Throws<ArgumentOutOfRangeException>(() => grid.IsWalkable(new GridCoordinate(3, 0)));
        }
    }
}

