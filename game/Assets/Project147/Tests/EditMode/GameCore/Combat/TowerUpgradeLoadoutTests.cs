using System;
using System.Collections.Generic;
using NUnit.Framework;
using Project147.GameCore.Combat;

namespace Project147.Tests.EditMode.GameCore.Combat
{
    public sealed class TowerUpgradeLoadoutTests
    {
        [Test]
        public void Constructor_WhenUpgradesAreValid_SelectsFirstUpgrade()
        {
            var damage = CreateUpgrade("damage");
            var rapid = CreateUpgrade("rapid");

            var loadout = new TowerUpgradeLoadout(new[] { damage, rapid });

            Assert.That(loadout.Upgrades, Is.EqualTo(new[] { damage, rapid }));
            Assert.That(loadout.SelectedIndex, Is.EqualTo(0));
            Assert.That(loadout.SelectedUpgrade, Is.SameAs(damage));
        }

        [Test]
        public void Constructor_WhenUpgradesAreNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new TowerUpgradeLoadout(null));
        }

        [Test]
        public void Constructor_WhenUpgradesAreEmpty_Throws()
        {
            Assert.Throws<ArgumentException>(() => new TowerUpgradeLoadout(new List<TowerUpgradeDefinition>()));
        }

        [Test]
        public void Constructor_WhenUpgradesContainNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new TowerUpgradeLoadout(new TowerUpgradeDefinition[] { null }));
        }

        [Test]
        public void Select_WhenIndexIsValid_ReturnsLoadoutWithSelectedUpgrade()
        {
            var damage = CreateUpgrade("damage");
            var rapid = CreateUpgrade("rapid");
            var loadout = new TowerUpgradeLoadout(new[] { damage, rapid });

            var result = loadout.Select(1);

            Assert.That(result.SelectedIndex, Is.EqualTo(1));
            Assert.That(result.SelectedUpgrade, Is.SameAs(rapid));
            Assert.That(loadout.SelectedIndex, Is.EqualTo(0));
        }

        [TestCase(-1)]
        [TestCase(2)]
        public void Select_WhenIndexIsOutsideUpgradeList_Throws(int index)
        {
            var loadout = new TowerUpgradeLoadout(new[] { CreateUpgrade("damage"), CreateUpgrade("rapid") });

            Assert.Throws<ArgumentOutOfRangeException>(() => loadout.Select(index));
        }

        [Test]
        public void SelectNext_WhenAtEnd_WrapsToFirstUpgrade()
        {
            var loadout = new TowerUpgradeLoadout(new[] { CreateUpgrade("damage"), CreateUpgrade("rapid") })
                .Select(1);

            var result = loadout.SelectNext();

            Assert.That(result.SelectedIndex, Is.EqualTo(0));
        }

        [Test]
        public void SelectPrevious_WhenAtStart_WrapsToLastUpgrade()
        {
            var loadout = new TowerUpgradeLoadout(new[] { CreateUpgrade("damage"), CreateUpgrade("rapid") });

            var result = loadout.SelectPrevious();

            Assert.That(result.SelectedIndex, Is.EqualTo(1));
        }

        private static TowerUpgradeDefinition CreateUpgrade(string id)
        {
            return new TowerUpgradeDefinition(id, 75, 1.2f, 1.1f, 0.1f, 0.05f, 0.1f);
        }
    }
}
