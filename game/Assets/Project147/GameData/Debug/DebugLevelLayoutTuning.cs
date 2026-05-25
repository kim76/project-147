using System;
using Project147.GameCore.Grid;
using Project147.GameCore.Level;
using UnityEngine;

namespace Project147.GameData.Debug
{
    [Serializable]
    public sealed class DebugLevelLayoutTuning
    {
        [SerializeField]
        private string id;

        [SerializeField]
        private int width;

        [SerializeField]
        private int height;

        [SerializeField]
        private Vector2Int spawn;

        [SerializeField]
        private Vector2Int goal;

        [SerializeField]
        private Vector2Int[] blockedCells;

        public DebugLevelLayoutTuning()
        {
        }

        public DebugLevelLayoutTuning(
            string id,
            int width,
            int height,
            Vector2Int spawn,
            Vector2Int goal,
            Vector2Int[] blockedCells)
        {
            this.id = id;
            this.width = width;
            this.height = height;
            this.spawn = spawn;
            this.goal = goal;
            this.blockedCells = blockedCells;
        }

        public LevelLayoutDefinition CreateDefinition()
        {
            var cells = blockedCells ?? Array.Empty<Vector2Int>();
            var coordinates = new GridCoordinate[cells.Length];

            for (var index = 0; index < cells.Length; index++)
            {
                coordinates[index] = ToGridCoordinate(cells[index]);
            }

            return new LevelLayoutDefinition(
                id,
                width,
                height,
                ToGridCoordinate(spawn),
                ToGridCoordinate(goal),
                coordinates);
        }

        private static GridCoordinate ToGridCoordinate(Vector2Int coordinate)
        {
            return new GridCoordinate(coordinate.x, coordinate.y);
        }
    }
}
