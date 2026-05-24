using NUnit.Framework;
using Project147.GameCore.Grid;

namespace Project147.Tests.EditMode.GameCore.Grid
{
    public sealed class GridCoordinateTests
    {
        [Test]
        public void Constructor_StoresColumnAndRow()
        {
            var coordinate = new GridCoordinate(3, 7);

            Assert.That(coordinate.Column, Is.EqualTo(3));
            Assert.That(coordinate.Row, Is.EqualTo(7));
        }

        [Test]
        public void Offset_ReturnsCoordinateWithOffsetsApplied()
        {
            var coordinate = new GridCoordinate(3, 7);

            var result = coordinate.Offset(-1, 2);

            Assert.That(result, Is.EqualTo(new GridCoordinate(2, 9)));
        }

        [Test]
        public void ManhattanDistanceTo_ReturnsGridDistanceWithoutDiagonals()
        {
            var start = new GridCoordinate(2, 8);
            var end = new GridCoordinate(7, 5);

            var result = start.ManhattanDistanceTo(end);

            Assert.That(result, Is.EqualTo(8));
        }

        [Test]
        public void Equality_UsesColumnAndRow()
        {
            var first = new GridCoordinate(4, 5);
            var second = new GridCoordinate(4, 5);
            var different = new GridCoordinate(5, 4);

            Assert.That(first, Is.EqualTo(second));
            Assert.That(first == second, Is.True);
            Assert.That(first != different, Is.True);
        }
    }
}
