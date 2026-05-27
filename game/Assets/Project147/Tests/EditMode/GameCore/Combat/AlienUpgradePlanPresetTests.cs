using System;
using NUnit.Framework;
using Project147.GameCore.Combat;

namespace Project147.Tests.EditMode.GameCore.Combat
{
    public sealed class AlienUpgradePlanPresetTests
    {
        [Test]
        public void Constructor_WhenValuesAreValid_StoresValues()
        {
            var plan = CreatePlan();

            var preset = new AlienUpgradePlanPreset("speed", plan);

            Assert.That(preset.Id, Is.EqualTo("speed"));
            Assert.That(preset.Plan, Is.SameAs(plan));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void Constructor_WhenIdIsMissing_Throws(string id)
        {
            Assert.Throws<ArgumentException>(() => new AlienUpgradePlanPreset(id, CreatePlan()));
        }

        [Test]
        public void Constructor_WhenPlanIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new AlienUpgradePlanPreset("speed", null));
        }

        public static AlienUpgradeChoicePlan CreatePlan()
        {
            return new AlienUpgradeChoicePlan(
                5,
                new[] { CreateChoice("speed", 2, 1, 1.15f) });
        }

        private static AlienUpgradeChoiceDefinition CreateChoice(
            string id,
            int cost,
            float healthMultiplier,
            float speedMultiplier)
        {
            return new AlienUpgradeChoiceDefinition(
                id,
                id,
                cost,
                new AlienUpgradeDefinition(
                    $"{id}-upgrade",
                    healthMultiplier,
                    speedMultiplier,
                    1,
                    0,
                    DamageType.Kinetic,
                    0));
        }
    }
}
