using System;
using System.Collections.Generic;

namespace Project147.GameCore.Grid
{
    public sealed class GridPathfinder
    {
        public IReadOnlyList<GridCoordinate> FindShortestPath(
            TacticalGrid grid,
            GridCoordinate start,
            GridCoordinate goal)
        {
            if (grid == null)
            {
                throw new ArgumentNullException(nameof(grid));
            }

            if (!grid.IsWalkable(start) || !grid.IsWalkable(goal))
            {
                return Array.Empty<GridCoordinate>();
            }

            var frontier = new Queue<GridCoordinate>();
            var visited = new HashSet<GridCoordinate>();
            var cameFrom = new Dictionary<GridCoordinate, GridCoordinate>();

            frontier.Enqueue(start);
            visited.Add(start);

            while (frontier.Count > 0)
            {
                var current = frontier.Dequeue();

                if (current == goal)
                {
                    return ReconstructPath(start, goal, cameFrom);
                }

                foreach (var neighbour in grid.WalkableOrthogonalNeighbours(current))
                {
                    if (visited.Contains(neighbour))
                    {
                        continue;
                    }

                    visited.Add(neighbour);
                    cameFrom[neighbour] = current;
                    frontier.Enqueue(neighbour);
                }
            }

            return Array.Empty<GridCoordinate>();
        }

        public bool HasPath(TacticalGrid grid, GridCoordinate start, GridCoordinate goal)
        {
            return FindShortestPath(grid, start, goal).Count > 0;
        }

        private static IReadOnlyList<GridCoordinate> ReconstructPath(
            GridCoordinate start,
            GridCoordinate goal,
            IReadOnlyDictionary<GridCoordinate, GridCoordinate> cameFrom)
        {
            var path = new List<GridCoordinate>();
            var current = goal;

            path.Add(current);

            while (current != start)
            {
                current = cameFrom[current];
                path.Add(current);
            }

            path.Reverse();
            return path;
        }
    }
}

