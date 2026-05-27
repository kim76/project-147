using System;
using NUnit.Framework;
using Project147.GameCore.Combat;

namespace Project147.Tests.EditMode.GameCore.Combat
{
    public sealed class AlienUpgradePlanPresetSetTests
    {
        [Test]
        public void Constructor_WhenPresetsAreValid_SelectsFirstPreset()
        {
            var speed = CreatePreset("speed");
            var tough = CreatePreset("tough");

            var presetSet = new AlienUpgradePlanPresetSet(new[] { speed, tough });

            Assert.That(presetSet.SelectedIndex, Is.EqualTo(0));
            Assert.That(presetSet.SelectedPreset, Is.SameAs(speed));
            Assert.That(presetSet.SelectedPlan, Is.SameAs(speed.Plan));
        }

        [Test]
        public void Constructor_WhenPresetsAreNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new AlienUpgradePlanPresetSet(null));
        }

        [Test]
        public void Constructor_WhenPresetsAreEmpty_Throws()
        {
            Assert.Throws<ArgumentException>(() => new AlienUpgradePlanPresetSet(Array.Empty<AlienUpgradePlanPreset>()));
        }

        [Test]
        public void Constructor_WhenPresetsContainDuplicateIds_Throws()
        {
            Assert.Throws<ArgumentException>(() => new AlienUpgradePlanPresetSet(
                new[] { CreatePreset("speed"), CreatePreset("speed") }));
        }

        [Test]
        public void SelectNext_WhenAtEnd_WrapsToFirstPreset()
        {
            var presetSet = new AlienUpgradePlanPresetSet(new[] { CreatePreset("speed"), CreatePreset("tough") })
                .SelectNext();

            var result = presetSet.SelectNext();

            Assert.That(result.SelectedIndex, Is.EqualTo(0));
        }

        [Test]
        public void SelectPrevious_WhenAtStart_WrapsToLastPreset()
        {
            var presetSet = new AlienUpgradePlanPresetSet(new[] { CreatePreset("speed"), CreatePreset("tough") });

            var result = presetSet.SelectPrevious();

            Assert.That(result.SelectedIndex, Is.EqualTo(1));
        }

        private static AlienUpgradePlanPreset CreatePreset(string id)
        {
            return new AlienUpgradePlanPreset(id, AlienUpgradePlanPresetTests.CreatePlan());
        }
    }
}
