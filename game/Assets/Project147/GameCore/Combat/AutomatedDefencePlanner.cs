using System;
using System.Collections.Generic;
using Project147.GameCore.Grid;

namespace Project147.GameCore.Combat
{
    public sealed class AutomatedDefencePlanner
    {
        private readonly GridPathfinder pathfinder;
        private readonly TowerPlacementValidator placementValidator;

        public AutomatedDefencePlanner(GridPathfinder pathfinder, TowerPlacementValidator placementValidator)
        {
            this.pathfinder = pathfinder ?? throw new ArgumentNullException(nameof(pathfinder));
            this.placementValidator = placementValidator ?? throw new ArgumentNullException(nameof(placementValidator));
        }

        public AutomatedDefencePlan CreatePlan(
            TacticalGrid grid,
            GridCoordinate spawn,
            GridCoordinate goal,
            IReadOnlyList<TowerDefinition> towerOptions,
            int budget,
            int maxPlacements)
        {
            if (grid == null)
            {
                throw new ArgumentNullException(nameof(grid));
            }

            if (towerOptions == null)
            {
                throw new ArgumentNullException(nameof(towerOptions));
            }

            if (towerOptions.Count == 0)
            {
                throw new ArgumentException("Automated defence requires at least one tower option.", nameof(towerOptions));
            }

            foreach (var tower in towerOptions)
            {
                if (tower == null)
                {
                    throw new ArgumentException("Automated defence tower options cannot contain null entries.", nameof(towerOptions));
                }
            }

            if (budget < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(budget), "Automated defence budget cannot be negative.");
            }

            if (maxPlacements < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxPlacements), "Automated defence max placements cannot be negative.");
            }

            var placements = new List<AutomatedDefencePlacement>();
            var currentGrid = grid;
            var remainingBudget = budget;

            while (placements.Count < maxPlacements)
            {
                var tower = SelectBestAffordableTower(towerOptions, remainingBudget);

                if (tower == null)
                {
                    break;
                }

                var path = pathfinder.FindShortestPath(currentGrid, spawn, goal);

                if (path.Count == 0)
                {
                    break;
                }

                var coordinate = SelectBestCoordinate(currentGrid, spawn, goal, path, tower);

                if (!coordinate.HasValue)
                {
                    break;
                }

                placements.Add(new AutomatedDefencePlacement(coordinate.Value, tower));
                currentGrid = currentGrid.WithBlockedCell(coordinate.Value);
                remainingBudget -= tower.Cost;
            }

            return new AutomatedDefencePlan(budget, placements);
        }

        private static TowerDefinition SelectBestAffordableTower(
            IReadOnlyList<TowerDefinition> towerOptions,
            int remainingBudget)
        {
            TowerDefinition best = null;

            foreach (var tower in towerOptions)
            {
                if (tower.Cost > remainingBudget)
                {
                    continue;
                }

                if (best == null || CompareTowerPressure(tower, best) > 0)
                {
                    best = tower;
                }
            }

            return best;
        }

        private static int CompareTowerPressure(TowerDefinition left, TowerDefinition right)
        {
            var leftPressure = left.Damage * left.FireRatePerSecond;
            var rightPressure = right.Damage * right.FireRatePerSecond;

            if (leftPressure > rightPressure)
            {
                return 1;
            }

            if (leftPressure < rightPressure)
            {
                return -1;
            }

            if (left.Range > right.Range)
            {
                return 1;
            }

            if (left.Range < right.Range)
            {
                return -1;
            }

            return left.Cost.CompareTo(right.Cost);
        }

        private GridCoordinate? SelectBestCoordinate(
            TacticalGrid grid,
            GridCoordinate spawn,
            GridCoordinate goal,
            IReadOnlyList<GridCoordinate> path,
            TowerDefinition tower)
        {
            GridCoordinate? best = null;
            var bestCoverage = -1;
            var bestGoalDistance = int.MaxValue;

            foreach (var coordinate in grid.Bounds.Coordinates())
            {
                if (!placementValidator.CanPlaceTower(grid, coordinate, spawn, goal))
                {
                    continue;
                }

                var coverage = CountCoveredPathCells(coordinate, path, tower.Range);
                var goalDistance = coordinate.ManhattanDistanceTo(goal);

                if (coverage > bestCoverage
                    || coverage == bestCoverage && goalDistance < bestGoalDistance
                    || coverage == bestCoverage && goalDistance == bestGoalDistance && IsEarlierCoordinate(coordinate, best))
                {
                    best = coordinate;
                    bestCoverage = coverage;
                    bestGoalDistance = goalDistance;
                }
            }

            return best;
        }

        private static bool IsEarlierCoordinate(GridCoordinate coordinate, GridCoordinate? currentBest)
        {
            if (!currentBest.HasValue)
            {
                return true;
            }

            return coordinate.Row < currentBest.Value.Row
                || coordinate.Row == currentBest.Value.Row && coordinate.Column < currentBest.Value.Column;
        }

        private static int CountCoveredPathCells(
            GridCoordinate coordinate,
            IReadOnlyList<GridCoordinate> path,
            float range)
        {
            var rangeSquared = range * range;
            var count = 0;

            foreach (var pathCell in path)
            {
                var columnDistance = coordinate.Column - pathCell.Column;
                var rowDistance = coordinate.Row - pathCell.Row;
                var distanceSquared = columnDistance * columnDistance + rowDistance * rowDistance;

                if (distanceSquared <= rangeSquared)
                {
                    count++;
                }
            }

            return count;
        }
    }
}
