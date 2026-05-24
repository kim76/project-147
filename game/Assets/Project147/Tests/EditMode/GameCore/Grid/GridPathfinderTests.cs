using System;
using NUnit.Framework;
using Project147.GameCore.Grid;

namespace Project147.Tests.EditMode.GameCore.Grid
{
    public sealed class GridPathfinderTests
    {
        [Test]
        public void FindShortestPath_WhenStraightPathExists_ReturnsStartToGoalPath()
        {
            var grid = new TacticalGrid(new GridBounds(3, 1));
            var pathfinder = new GridPathfinder();

            var result = pathfinder.FindShortestPath(
                grid,
                new GridCoordinate(0, 0),
                new GridCoordinate(2, 0));

            Assert.That(result, Is.EqualTo(new[]
            {
                new GridCoordinate(0, 0),
                new GridCoordinate(1, 0),
                new GridCoordinate(2, 0)
            }));
        }

        [Test]
        public void FindShortestPath_WhenObstacleBlocksStraightLine_ReturnsRouteAroundObstacle()
        {
            var grid = new TacticalGrid(
                new GridBounds(3, 3),
                new[] { new GridCoordinate(1, 0) });
            var pathfinder = new GridPathfinder();

            var result = pathfinder.FindShortestPath(
                grid,
                new GridCoordinate(0, 0),
                new GridCoordinate(2, 0));

            Assert.That(result, Is.EqualTo(new[]
            {
                new GridCoordinate(0, 0),
                new GridCoordinate(0, 1),
                new GridCoordinate(1, 1),
                new GridCoordinate(2, 1),
                new GridCoordinate(2, 0)
            }));
        }

        [Test]
        public void FindShortestPath_WhenNoPathExists_ReturnsEmptyPath()
        {
            var grid = new TacticalGrid(
                new GridBounds(3, 3),
                new[]
                {
                    new GridCoordinate(1, 0),
                    new GridCoordinate(1, 1),
                    new GridCoordinate(1, 2)
                });
            var pathfinder = new GridPathfinder();

            var result = pathfinder.FindShortestPath(
                grid,
                new GridCoordinate(0, 1),
                new GridCoordinate(2, 1));

            Assert.That(result, Is.Empty);
        }

        [Test]
        public void FindShortestPath_WhenStartIsBlocked_ReturnsEmptyPath()
        {
            var start = new GridCoordinate(0, 0);
            var grid = new TacticalGrid(new GridBounds(3, 1), new[] { start });
            var pathfinder = new GridPathfinder();

            var result = pathfinder.FindShortestPath(grid, start, new GridCoordinate(2, 0));

            Assert.That(result, Is.Empty);
        }

        [Test]
        public void FindShortestPath_WhenGoalIsBlocked_ReturnsEmptyPath()
        {
            var goal = new GridCoordinate(2, 0);
            var grid = new TacticalGrid(new GridBounds(3, 1), new[] { goal });
            var pathfinder = new GridPathfinder();

            var result = pathfinder.FindShortestPath(grid, new GridCoordinate(0, 0), goal);

            Assert.That(result, Is.Empty);
        }

        [Test]
        public void FindShortestPath_WhenGridIsNull_Throws()
        {
            var pathfinder = new GridPathfinder();

            Assert.Throws<ArgumentNullException>(() => pathfinder.FindShortestPath(
                null,
                new GridCoordinate(0, 0),
                new GridCoordinate(1, 0)));
        }

        [Test]
        public void HasPath_WhenPathExists_ReturnsTrue()
        {
            var grid = new TacticalGrid(new GridBounds(2, 1));
            var pathfinder = new GridPathfinder();

            var result = pathfinder.HasPath(grid, new GridCoordinate(0, 0), new GridCoordinate(1, 0));

            Assert.That(result, Is.True);
        }

        [Test]
        public void HasPath_WhenPathDoesNotExist_ReturnsFalse()
        {
            var grid = new TacticalGrid(
                new GridBounds(3, 1),
                new[] { new GridCoordinate(1, 0) });
            var pathfinder = new GridPathfinder();

            var result = pathfinder.HasPath(grid, new GridCoordinate(0, 0), new GridCoordinate(2, 0));

            Assert.That(result, Is.False);
        }
    }
}

