using System;
using System.Collections.Generic;
using Project147.GameCore.Grid;

namespace Project147.GameCore.Level
{
    public sealed class LevelLayoutDefinition
    {
        public LevelLayoutDefinition(
            string id,
            int width,
            int height,
            GridCoordinate spawn,
            GridCoordinate goal,
            IReadOnlyList<GridCoordinate> blockedCells)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Level layout id is required.", nameof(id));
            }

            if (width <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(width), "Level width must be greater than zero.");
            }

            if (height <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(height), "Level height must be greater than zero.");
            }

            if (blockedCells == null)
            {
                throw new ArgumentNullException(nameof(blockedCells));
            }

            ValidateCoordinate(spawn, width, height, nameof(spawn));
            ValidateCoordinate(goal, width, height, nameof(goal));

            var cells = new List<GridCoordinate>();

            foreach (var blockedCell in blockedCells)
            {
                ValidateCoordinate(blockedCell, width, height, nameof(blockedCells));

                if (blockedCell == spawn)
                {
                    throw new ArgumentException("Spawn cell cannot be blocked.", nameof(blockedCells));
                }

                if (blockedCell == goal)
                {
                    throw new ArgumentException("Goal cell cannot be blocked.", nameof(blockedCells));
                }

                if (!cells.Contains(blockedCell))
                {
                    cells.Add(blockedCell);
                }
            }

            Id = id;
            Width = width;
            Height = height;
            Spawn = spawn;
            Goal = goal;
            BlockedCells = cells;
        }

        public string Id { get; }

        public int Width { get; }

        public int Height { get; }

        public GridCoordinate Spawn { get; }

        public GridCoordinate Goal { get; }

        public IReadOnlyList<GridCoordinate> BlockedCells { get; }

        private static void ValidateCoordinate(GridCoordinate coordinate, int width, int height, string parameterName)
        {
            if (coordinate.Column < 0 || coordinate.Column >= width || coordinate.Row < 0 || coordinate.Row >= height)
            {
                throw new ArgumentOutOfRangeException(parameterName, "Coordinate must be inside the level layout.");
            }
        }
    }
}
