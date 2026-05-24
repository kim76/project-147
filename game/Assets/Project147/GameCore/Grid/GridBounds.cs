using System;
using System.Collections.Generic;

namespace Project147.GameCore.Grid
{
    public readonly struct GridBounds : IEquatable<GridBounds>
    {
        public GridBounds(int width, int height)
        {
            if (width <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(width), "Grid width must be greater than zero.");
            }

            if (height <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(height), "Grid height must be greater than zero.");
            }

            Width = width;
            Height = height;
        }

        public int Width { get; }

        public int Height { get; }

        public bool Contains(GridCoordinate coordinate)
        {
            return coordinate.Column >= 0
                && coordinate.Row >= 0
                && coordinate.Column < Width
                && coordinate.Row < Height;
        }

        public IEnumerable<GridCoordinate> Coordinates()
        {
            for (var row = 0; row < Height; row++)
            {
                for (var column = 0; column < Width; column++)
                {
                    yield return new GridCoordinate(column, row);
                }
            }
        }

        public bool Equals(GridBounds other)
        {
            return Width == other.Width && Height == other.Height;
        }

        public override bool Equals(object obj)
        {
            return obj is GridBounds other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Width, Height);
        }
    }
}

