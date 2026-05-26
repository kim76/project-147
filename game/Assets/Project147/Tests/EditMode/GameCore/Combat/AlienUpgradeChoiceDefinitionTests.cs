using System;
using System.Collections.Generic;
using NUnit.Framework;
using Project147.GameCore.Combat;

namespace Project147.Tests.EditMode.GameCore.Combat
{
    public sealed class AlienUpgradeChoiceDefinitionTests
    {
        [Test]
        public void Constructor_WhenValuesAreValid_StoresValues()
        {
            var upgrade = CreateUpgrade();

            var choice = new AlienUpgradeChoiceDefinition("hardened-carpaces", "Hardened Carapaces", 3, upgrade);

            Assert.That(choice.Id, Is.EqualTo("hardened-carpaces"));
            Assert.That(choice.Label, Is.EqualTo("Hardened Carapaces"));
            Assert.That(choice.Cost, Is.EqualTo(3));
            Assert.That(choice.Upgrade, Is.SameAs(upgrade));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void Constructor_WhenIdIsMissing_Throws(string id)
        {
            Assert.Throws<ArgumentException>(() => new AlienUpgradeChoiceDefinition(id, "Choice", 1, CreateUpgrade()));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void Constructor_WhenLabelIsMissing_Throws(string label)
        {
            Assert.Throws<ArgumentException>(() => new AlienUpgradeChoiceDefinition("choice", label, 1, CreateUpgrade()));
        }

        [Test]
        public void Constructor_WhenCostIsNegative_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new AlienUpgradeChoiceDefinition(
                "choice",
                "Choice",
                -1,
                CreateUpgrade()));
        }

        [Test]
        public void Constructor_WhenUpgradeIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new AlienUpgradeChoiceDefinition("choice", "Choice", 1, null));
        }

        [Test]
        public void ApplyTo_AppliesUnderlyingUpgrade()
        {
            var choice = new AlienUpgradeChoiceDefinition("hardened-carpaces", "Hardened Carapaces", 3, CreateUpgrade());
            var alien = new AlienDefinition("debug-basic", 100, 1, 10, new Dictionary<DamageType, float>());

            var result = choice.ApplyTo(alien);

            Assert.That(result.MaxHealth, Is.EqualTo(125));
        }

        private static AlienUpgradeDefinition CreateUpgrade()
        {
            return new AlienUpgradeDefinition(
                "hardened-carpaces-upgrade",
                1.25f,
                1,
                1,
                0,
                DamageType.Kinetic,
                0);
        }
    }
}
