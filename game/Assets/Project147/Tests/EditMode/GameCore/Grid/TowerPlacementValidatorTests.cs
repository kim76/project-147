using System;
using NUnit.Framework;
using Project147.GameCore.Grid;

namespace Project147.Tests.EditMode.GameCore.Grid
{
    public sealed class TowerPlacementValidatorTests
    {
        [Test]
        public void Constructor_WhenPathfinderIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new TowerPlacementValidator(null));
        }

        [Test]
        public void ValidatePlacement_WhenGridIsNull_Throws()
        {
            var validator = new TowerPlacementValidator(new GridPathfinder());

            Assert.Throws<ArgumentNullException>(() => validator.ValidatePlacement(
                null,
                new GridCoordinate(1, 1),
                new GridCoordinate(0, 0),
                new GridCoordinate(2, 2)));
        }

        [Test]
        public void ValidatePlacement_WhenPlacementKeepsPathOpen_ReturnsValid()
        {
            var grid = new TacticalGrid(new GridBounds(3, 3));
            var validator = new TowerPlacementValidator(new GridPathfinder());

            var result = validator.ValidatePlacement(
                grid,
                new GridCoordinate(1, 1),
                new GridCoordinate(0, 0),
                new GridCoordinate(2, 0));

            Assert.That(result, Is.EqualTo(TowerPlacementResult.Valid()));
        }

        [Test]
        public void ValidatePlacement_WhenPlacementIsAlreadyBlocked_ReturnsAlreadyBlocked()
        {
            var placement = new GridCoordinate(1, 1);
            var grid = new TacticalGrid(new GridBounds(3, 3), new[] { placement });
            var validator = new TowerPlacementValidator(new GridPathfinder());

            var result = validator.ValidatePlacement(
                grid,
                placement,
                new GridCoordinate(0, 0),
                new GridCoordinate(2, 0));

            Assert.That(result.FailureReason, Is.EqualTo(TowerPlacementFailureReason.AlreadyBlocked));
        }

        [Test]
        public void ValidatePlacement_WhenPlacementIsSpawn_ReturnsSpawnCell()
        {
            var spawn = new GridCoordinate(0, 0);
            var grid = new TacticalGrid(new GridBounds(3, 3));
            var validator = new TowerPlacementValidator(new GridPathfinder());

            var result = validator.ValidatePlacement(
                grid,
                spawn,
                spawn,
                new GridCoordinate(2, 0));

            Assert.That(result.FailureReason, Is.EqualTo(TowerPlacementFailureReason.SpawnCell));
        }

        [Test]
        public void ValidatePlacement_WhenPlacementIsGoal_ReturnsGoalCell()
        {
            var goal = new GridCoordinate(2, 0);
            var grid = new TacticalGrid(new GridBounds(3, 3));
            var validator = new TowerPlacementValidator(new GridPathfinder());

            var result = validator.ValidatePlacement(
                grid,
                goal,
                new GridCoordinate(0, 0),
                goal);

            Assert.That(result.FailureReason, Is.EqualTo(TowerPlacementFailureReason.GoalCell));
        }

        [Test]
        public void ValidatePlacement_WhenPlacementBlocksOnlyPath_ReturnsBlocksAllPaths()
        {
            var grid = new TacticalGrid(new GridBounds(3, 1));
            var validator = new TowerPlacementValidator(new GridPathfinder());

            var result = validator.ValidatePlacement(
                grid,
                new GridCoordinate(1, 0),
                new GridCoordinate(0, 0),
                new GridCoordinate(2, 0));

            Assert.That(result.FailureReason, Is.EqualTo(TowerPlacementFailureReason.BlocksAllPaths));
        }

        [Test]
        public void ValidatePlacement_WhenPlacementIsOutsideBounds_Throws()
        {
            var grid = new TacticalGrid(new GridBounds(3, 3));
            var validator = new TowerPlacementValidator(new GridPathfinder());

            Assert.Throws<ArgumentOutOfRangeException>(() => validator.ValidatePlacement(
                grid,
                new GridCoordinate(3, 0),
                new GridCoordinate(0, 0),
                new GridCoordinate(2, 0)));
        }

        [Test]
        public void ValidatePlacement_WhenSpawnsAreNull_Throws()
        {
            var grid = new TacticalGrid(new GridBounds(3, 3));
            var validator = new TowerPlacementValidator(new GridPathfinder());

            Assert.Throws<ArgumentNullException>(() => validator.ValidatePlacement(
                grid,
                new GridCoordinate(1, 1),
                null,
                new GridCoordinate(2, 0)));
        }

        [Test]
        public void ValidatePlacement_WhenSpawnsAreEmpty_Throws()
        {
            var grid = new TacticalGrid(new GridBounds(3, 3));
            var validator = new TowerPlacementValidator(new GridPathfinder());

            Assert.Throws<ArgumentException>(() => validator.ValidatePlacement(
                grid,
                new GridCoordinate(1, 1),
                new GridCoordinate[0],
                new GridCoordinate(2, 0)));
        }

        [Test]
        public void ValidatePlacement_WhenPlacementBlocksOneSpawn_ReturnsBlocksAllPaths()
        {
            var grid = new TacticalGrid(
                new GridBounds(3, 3),
                new[]
                {
                    new GridCoordinate(1, 1),
                    new GridCoordinate(1, 2)
                });
            var validator = new TowerPlacementValidator(new GridPathfinder());

            var result = validator.ValidatePlacement(
                grid,
                new GridCoordinate(1, 0),
                new[]
                {
                    new GridCoordinate(0, 0),
                    new GridCoordinate(0, 2)
                },
                new GridCoordinate(2, 0));

            Assert.That(result.FailureReason, Is.EqualTo(TowerPlacementFailureReason.BlocksAllPaths));
        }

        [Test]
        public void CanPlaceTower_WhenPlacementIsValid_ReturnsTrue()
        {
            var grid = new TacticalGrid(new GridBounds(3, 3));
            var validator = new TowerPlacementValidator(new GridPathfinder());

            var result = validator.CanPlaceTower(
                grid,
                new GridCoordinate(1, 1),
                new[] { new GridCoordinate(0, 0) },
                new GridCoordinate(2, 0));

            Assert.That(result, Is.True);
        }

        [Test]
        public void CanPlaceTower_WhenPlacementIsInvalid_ReturnsFalse()
        {
            var grid = new TacticalGrid(new GridBounds(3, 1));
            var validator = new TowerPlacementValidator(new GridPathfinder());

            var result = validator.CanPlaceTower(
                grid,
                new GridCoordinate(1, 0),
                new[] { new GridCoordinate(0, 0) },
                new GridCoordinate(2, 0));

            Assert.That(result, Is.False);
        }

        [Test]
        public void PlaceTower_WhenPlacementIsValid_ReturnsGridWithPlacementBlocked()
        {
            var placement = new GridCoordinate(1, 1);
            var grid = new TacticalGrid(new GridBounds(3, 3));
            var validator = new TowerPlacementValidator(new GridPathfinder());

            var result = validator.PlaceTower(
                grid,
                placement,
                new[] { new GridCoordinate(0, 0) },
                new GridCoordinate(2, 0));

            Assert.That(result.IsBlocked(placement), Is.True);
            Assert.That(grid.IsWalkable(placement), Is.True);
        }

        [Test]
        public void PlaceTower_WhenPlacementIsInvalid_Throws()
        {
            var grid = new TacticalGrid(new GridBounds(3, 1));
            var validator = new TowerPlacementValidator(new GridPathfinder());

            Assert.Throws<InvalidOperationException>(() => validator.PlaceTower(
                grid,
                new GridCoordinate(1, 0),
                new[] { new GridCoordinate(0, 0) },
                new GridCoordinate(2, 0)));
        }
    }
}
