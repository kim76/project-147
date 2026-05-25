using System;
using NUnit.Framework;
using Project147.GameCore.Choices;

namespace Project147.Tests.EditMode.GameCore.Choices
{
    public sealed class RunChoiceDefinitionTests
    {
        [Test]
        public void Constructor_WhenValuesAreValid_StoresValues()
        {
            var choice = new RunChoiceDefinition(
                "salvage-drop",
                "Salvage Drop",
                RunChoiceEffectType.AddScrap,
                45);

            Assert.That(choice.Id, Is.EqualTo("salvage-drop"));
            Assert.That(choice.Label, Is.EqualTo("Salvage Drop"));
            Assert.That(choice.EffectType, Is.EqualTo(RunChoiceEffectType.AddScrap));
            Assert.That(choice.Amount, Is.EqualTo(45));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void Constructor_WhenIdIsMissing_Throws(string id)
        {
            Assert.Throws<ArgumentException>(() => new RunChoiceDefinition(
                id,
                "Salvage Drop",
                RunChoiceEffectType.AddScrap,
                45));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void Constructor_WhenLabelIsMissing_Throws(string label)
        {
            Assert.Throws<ArgumentException>(() => new RunChoiceDefinition(
                "salvage-drop",
                label,
                RunChoiceEffectType.AddScrap,
                45));
        }

        [Test]
        public void Constructor_WhenAmountIsZero_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new RunChoiceDefinition(
                "salvage-drop",
                "Salvage Drop",
                RunChoiceEffectType.AddScrap,
                0));
        }
    }
}
