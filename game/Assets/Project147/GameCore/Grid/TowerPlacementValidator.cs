using System;
using System.Collections.Generic;
using System.Linq;

namespace Project147.GameCore.Grid
{
    public sealed class TowerPlacementValidator
    {
        private readonly GridPathfinder pathfinder;

        public TowerPlacementValidator(GridPathfinder pathfinder)
        {
            this.pathfinder = pathfinder ?? throw new ArgumentNullException(nameof(pathfinder));
        }

        public TowerPlacementResult ValidatePlacement(
            TacticalGrid grid,
            GridCoordinate placement,
            GridCoordinate spawn,
            GridCoordinate goal)
        {
            return ValidatePlacement(grid, placement, new[] { spawn }, goal);
        }

        public TowerPlacementResult ValidatePlacement(
            TacticalGrid grid,
            GridCoordinate placement,
            IReadOnlyCollection<GridCoordinate> spawns,
            GridCoordinate goal)
        {
            if (grid == null)
            {
                throw new ArgumentNullException(nameof(grid));
            }

            if (spawns == null)
            {
                throw new ArgumentNullException(nameof(spawns));
            }

            if (spawns.Count == 0)
            {
                throw new ArgumentException("At least one spawn is required.", nameof(spawns));
            }

            if (spawns.Contains(placement))
            {
                return TowerPlacementResult.Invalid(TowerPlacementFailureReason.SpawnCell);
            }

            if (placement == goal)
            {
                return TowerPlacementResult.Invalid(TowerPlacementFailureReason.GoalCell);
            }

            if (grid.IsBlocked(placement))
            {
                return TowerPlacementResult.Invalid(TowerPlacementFailureReason.AlreadyBlocked);
            }

            var gridWithPlacement = grid.WithBlockedCell(placement);

            foreach (var spawn in spawns)
            {
                if (!pathfinder.HasPath(gridWithPlacement, spawn, goal))
                {
                    return TowerPlacementResult.Invalid(TowerPlacementFailureReason.BlocksAllPaths);
                }
            }

            return TowerPlacementResult.Valid();
        }

        public bool CanPlaceTower(
            TacticalGrid grid,
            GridCoordinate placement,
            IReadOnlyCollection<GridCoordinate> spawns,
            GridCoordinate goal)
        {
            return ValidatePlacement(grid, placement, spawns, goal).IsValid;
        }

        public bool CanPlaceTower(
            TacticalGrid grid,
            GridCoordinate placement,
            GridCoordinate spawn,
            GridCoordinate goal)
        {
            return ValidatePlacement(grid, placement, spawn, goal).IsValid;
        }

        public TacticalGrid PlaceTower(
            TacticalGrid grid,
            GridCoordinate placement,
            IReadOnlyCollection<GridCoordinate> spawns,
            GridCoordinate goal)
        {
            var result = ValidatePlacement(grid, placement, spawns, goal);

            if (!result.IsValid)
            {
                throw new InvalidOperationException($"Cannot place tower: {result.FailureReason}.");
            }

            return grid.WithBlockedCell(placement);
        }
    }
}
