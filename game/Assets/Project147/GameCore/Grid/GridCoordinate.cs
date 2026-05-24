using System;

namespace Project147.GameCore.Grid
{
    public readonly struct GridCoordinate : IEquatable<GridCoordinate>
    {
        public GridCoordinate(int column, int row)
        {
            Column = column;
            Row = row;
        }

        public int Column { get; }

        public int Row { get; }

        public GridCoordinate Offset(int columnOffset, int rowOffset)
        {
            return new GridCoordinate(Column + columnOffset, Row + rowOffset);
        }

        public int ManhattanDistanceTo(GridCoordinate other)
        {
            return Math.Abs(Column - other.Column) + Math.Abs(Row - other.Row);
        }

        public bool Equals(GridCoordinate other)
        {
            return Column == other.Column && Row == other.Row;
        }

        public override bool Equals(object obj)
        {
            return obj is GridCoordinate other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Column, Row);
        }

        public override string ToString()
        {
            return $"({Column}, {Row})";
        }

        public static bool operator ==(GridCoordinate left, GridCoordinate right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(GridCoordinate left, GridCoordinate right)
        {
            return !left.Equals(right);
        }
    }
}
