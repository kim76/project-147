using System;
using NUnit.Framework;
using Project147.GameCore.Combat;
using Project147.GameCore.Grid;

namespace Project147.Tests.EditMode.GameCore.Combat
{
    public sealed class AutomatedDefencePlacementTests
    {
        [Test]
        public void Constructor_WhenValuesAreValid_StoresValues()
        {
            var tower = AutomatedDefencePlannerTests.CreateTower("railgun", 40, 10, 1);

            var placement = new AutomatedDefencePlacement(new GridCoordinate(1, 2), tower);

            Assert.That(placement.Coordinate, Is.EqualTo(new GridCoordinate(1, 2)));
            Assert.That(placement.Tower, Is.SameAs(tower));
            Assert.That(placement.Cost, Is.EqualTo(40));
        }

        [Test]
        public void Constructor_WhenTowerIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new AutomatedDefencePlacement(
                new GridCoordinate(1, 2),
                null));
        }
    }
}
