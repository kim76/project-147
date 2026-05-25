using System;
using System.Collections.Generic;
using NUnit.Framework;
using Project147.GameCore.Combat;

namespace Project147.Tests.EditMode.GameCore.Combat
{
    public sealed class TowerLoadoutTests
    {
        [Test]
        public void Constructor_WhenTowersAreValid_SelectsFirstTower()
        {
            var railgun = CreateTower("railgun-basic");
            var mortar = CreateTower("mortar-basic");

            var loadout = new TowerLoadout(new[] { railgun, mortar });

            Assert.That(loadout.Towers, Is.EqualTo(new[] { railgun, mortar }));
            Assert.That(loadout.SelectedIndex, Is.EqualTo(0));
            Assert.That(loadout.SelectedTower, Is.SameAs(railgun));
        }

        [Test]
        public void Constructor_WhenTowersAreNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new TowerLoadout(null));
        }

        [Test]
        public void Constructor_WhenTowersAreEmpty_Throws()
        {
            Assert.Throws<ArgumentException>(() => new TowerLoadout(new List<TowerDefinition>()));
        }

        [Test]
        public void Constructor_WhenTowersContainNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new TowerLoadout(new TowerDefinition[] { null }));
        }

        [Test]
        public void Select_WhenIndexIsValid_ReturnsLoadoutWithSelectedTower()
        {
            var railgun = CreateTower("railgun-basic");
            var mortar = CreateTower("mortar-basic");
            var loadout = new TowerLoadout(new[] { railgun, mortar });

            var result = loadout.Select(1);

            Assert.That(result.SelectedIndex, Is.EqualTo(1));
            Assert.That(result.SelectedTower, Is.SameAs(mortar));
            Assert.That(loadout.SelectedIndex, Is.EqualTo(0));
        }

        [TestCase(-1)]
        [TestCase(2)]
        public void Select_WhenIndexIsOutsideTowerList_Throws(int index)
        {
            var loadout = new TowerLoadout(new[] { CreateTower("railgun-basic"), CreateTower("mortar-basic") });

            Assert.Throws<ArgumentOutOfRangeException>(() => loadout.Select(index));
        }

        [Test]
        public void SelectNext_WhenAtEnd_WrapsToFirstTower()
        {
            var loadout = new TowerLoadout(new[] { CreateTower("railgun-basic"), CreateTower("mortar-basic") })
                .Select(1);

            var result = loadout.SelectNext();

            Assert.That(result.SelectedIndex, Is.EqualTo(0));
        }

        [Test]
        public void SelectPrevious_WhenAtStart_WrapsToLastTower()
        {
            var loadout = new TowerLoadout(new[] { CreateTower("railgun-basic"), CreateTower("mortar-basic") });

            var result = loadout.SelectPrevious();

            Assert.That(result.SelectedIndex, Is.EqualTo(1));
        }

        private static TowerDefinition CreateTower(string id)
        {
            return new TowerDefinition(
                id,
                50,
                2,
                1,
                10,
                DamageType.Kinetic,
                TowerTargetingMode.First);
        }
    }
}
