using System;
using NUnit.Framework;
using Project147.GameCore.Combat;

namespace Project147.Tests.EditMode.GameCore.Combat
{
    public sealed class TowerSaleCalculatorTests
    {
        [Test]
        public void CalculateRefund_WhenTowerIsLevelOne_RefundsTowerCostByMultiplier()
        {
            var calculator = new TowerSaleCalculator();
            var tower = new TowerState(CreateTower(cost: 50));

            var refund = calculator.CalculateRefund(tower, CreateUpgrade(cost: 75), 0.75f);

            Assert.That(refund, Is.EqualTo(38));
        }

        [Test]
        public void CalculateRefund_WhenTowerHasUpgrades_IncludesUpgradeSpend()
        {
            var calculator = new TowerSaleCalculator();
            var upgrade = CreateUpgrade(cost: 75);
            var tower = new TowerState(CreateTower(cost: 50))
                .Upgrade(upgrade)
                .Upgrade(upgrade);

            var refund = calculator.CalculateRefund(tower, upgrade, 0.75f);

            Assert.That(refund, Is.EqualTo(150));
        }

        [Test]
        public void CalculateRefund_WhenTowerIsNull_Throws()
        {
            var calculator = new TowerSaleCalculator();

            Assert.Throws<ArgumentNullException>(() => calculator.CalculateRefund(null, CreateUpgrade(cost: 75), 0.75f));
        }

        [Test]
        public void CalculateRefund_WhenUpgradeDefinitionIsNull_Throws()
        {
            var calculator = new TowerSaleCalculator();

            Assert.Throws<ArgumentNullException>(() => calculator.CalculateRefund(
                new TowerState(CreateTower(cost: 50)),
                null,
                0.75f));
        }

        [TestCase(-0.01f)]
        [TestCase(1.01f)]
        public void CalculateRefund_WhenRefundMultiplierIsOutsideZeroToOne_Throws(float refundMultiplier)
        {
            var calculator = new TowerSaleCalculator();

            Assert.Throws<ArgumentOutOfRangeException>(() => calculator.CalculateRefund(
                new TowerState(CreateTower(cost: 50)),
                CreateUpgrade(cost: 75),
                refundMultiplier));
        }

        private static TowerDefinition CreateTower(int cost)
        {
            return new TowerDefinition(
                "debug-railgun",
                cost,
                2,
                1,
                10,
                DamageType.Kinetic,
                TowerTargetingMode.First);
        }

        private static TowerUpgradeDefinition CreateUpgrade(int cost)
        {
            return new TowerUpgradeDefinition(
                "debug-upgrade",
                cost,
                1.2f,
                1.1f,
                0,
                0,
                0);
        }
    }
}
