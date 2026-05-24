using System;
using System.Collections.Generic;

namespace Project147.GameCore.Grid
{
    public sealed class TacticalGrid
    {
        private static readonly GridCoordinate[] OrthogonalOffsets =
        {
            new GridCoordinate(0, -1),
            new GridCoordinate(1, 0),
            new GridCoordinate(0, 1),
            new GridCoordinate(-1, 0)
        };

        private readonly HashSet<GridCoordinate> blockedCells;

        public TacticalGrid(GridBounds bounds)
            : this(bounds, Array.Empty<GridCoordinate>())
        {
        }

        public TacticalGrid(GridBounds bounds, IEnumerable<GridCoordinate> blockedCells)
        {
            Bounds = bounds;
            this.blockedCells = new HashSet<GridCoordinate>();

            foreach (var blockedCell in blockedCells)
            {
                EnsureWithinBounds(blockedCell);
                this.blockedCells.Add(blockedCell);
            }
        }

        public GridBounds Bounds { get; }

        public bool IsBlocked(GridCoordinate coordinate)
        {
            EnsureWithinBounds(coordinate);
            return blockedCells.Contains(coordinate);
        }

        public bool IsWalkable(GridCoordinate coordinate)
        {
            EnsureWithinBounds(coordinate);
            return !blockedCells.Contains(coordinate);
        }

        public TacticalGrid WithBlockedCell(GridCoordinate coordinate)
        {
            EnsureWithinBounds(coordinate);

            var nextBlockedCells = new HashSet<GridCoordinate>(blockedCells)
            {
                coordinate
            };

            return new TacticalGrid(Bounds, nextBlockedCells);
        }

        public TacticalGrid WithOpenCell(GridCoordinate coordinate)
        {
            EnsureWithinBounds(coordinate);

            var nextBlockedCells = new HashSet<GridCoordinate>(blockedCells);
            nextBlockedCells.Remove(coordinate);

            return new TacticalGrid(Bounds, nextBlockedCells);
        }

        public IReadOnlyList<GridCoordinate> WalkableOrthogonalNeighbours(GridCoordinate coordinate)
        {
            EnsureWithinBounds(coordinate);

            var neighbours = new List<GridCoordinate>(4);

            foreach (var offset in OrthogonalOffsets)
            {
                var neighbour = coordinate.Offset(offset.Column, offset.Row);

                if (Bounds.Contains(neighbour) && !blockedCells.Contains(neighbour))
                {
                    neighbours.Add(neighbour);
                }
            }

            return neighbours;
        }

        private void EnsureWithinBounds(GridCoordinate coordinate)
        {
            if (!Bounds.Contains(coordinate))
            {
                throw new ArgumentOutOfRangeException(nameof(coordinate), $"Coordinate {coordinate} is outside grid bounds.");
            }
        }
    }
}

