using System;
using Project147.GameCore.Grid;

namespace Project147.GameCore.Combat
{
    public sealed class AutomatedDefencePlacement
    {
        public AutomatedDefencePlacement(GridCoordinate coordinate, TowerDefinition tower)
        {
            Tower = tower ?? throw new ArgumentNullException(nameof(tower));
            Coordinate = coordinate;
            Cost = tower.Cost;
        }

        public GridCoordinate Coordinate { get; }

        public TowerDefinition Tower { get; }

        public int Cost { get; }
    }
}
