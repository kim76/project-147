using System.Linq;
using NUnit.Framework;
using Project147.GameData.Debug;
using UnityEngine;

namespace Project147.Tests.EditMode.GameData.Debug
{
    public sealed class DebugFirstSliceConfigTests
    {
        [Test]
        public void CreateTowerDefinitions_ReturnsRailgunAndMortar()
        {
            var config = ScriptableObject.CreateInstance<DebugFirstSliceConfig>();

            var towers = config.CreateTowerDefinitions();

            Assert.That(towers.Count, Is.EqualTo(2));
            Assert.That(towers[0].Id, Is.EqualTo("debug-railgun"));
            Assert.That(towers[0].StatusEffects.Count, Is.EqualTo(1));
            Assert.That(towers[1].Id, Is.EqualTo("debug-mortar"));
            Assert.That(towers[1].SplashRadius, Is.GreaterThan(0));
        }

        [Test]
        public void CreateWaveDefinition_WhenLaterWave_ReturnsMixedAlienIds()
        {
            var config = ScriptableObject.CreateInstance<DebugFirstSliceConfig>();

            var wave = config.CreateWaveDefinition(2);

            Assert.That(wave.AlienCount, Is.GreaterThan(0));
            Assert.That(wave.SpawnEntries.Any(entry => entry.AlienId == config.BasicAlienId), Is.True);
            Assert.That(wave.SpawnEntries.Any(entry => entry.AlienId == config.FastAlienId), Is.True);
            Assert.That(wave.SpawnEntries.Any(entry => entry.AlienId == config.ArmouredAlienId), Is.True);
        }

        [Test]
        public void CreateFreezePulseAbilityDefinition_ReturnsCooldownAbility()
        {
            var config = ScriptableObject.CreateInstance<DebugFirstSliceConfig>();

            var ability = config.CreateFreezePulseAbilityDefinition();

            Assert.That(ability.Id, Is.EqualTo("debug-freeze-pulse"));
            Assert.That(ability.CooldownSeconds, Is.GreaterThan(0));
            Assert.That(ability.StatusEffect.MovementSpeedMultiplier, Is.LessThan(1));
        }

        [Test]
        public void CreateOrbitalStrikeAbilityDefinition_ReturnsDamageAbility()
        {
            var config = ScriptableObject.CreateInstance<DebugFirstSliceConfig>();

            var ability = config.CreateOrbitalStrikeAbilityDefinition();

            Assert.That(ability.Id, Is.EqualTo("debug-orbital-strike"));
            Assert.That(ability.CooldownSeconds, Is.GreaterThan(0));
            Assert.That(ability.HasDamage, Is.True);
            Assert.That(ability.DamageAmount, Is.GreaterThan(0));
        }

        [Test]
        public void CreateRunChoiceDefinitions_ReturnsBetweenWaveChoices()
        {
            var config = ScriptableObject.CreateInstance<DebugFirstSliceConfig>();

            var choices = config.CreateRunChoiceDefinitions();

            Assert.That(choices.Count, Is.EqualTo(5));
            Assert.That(choices.Any(choice => choice.Id == "salvage-drop"), Is.True);
            Assert.That(choices.Any(choice => choice.Id == "field-repair"), Is.True);
            Assert.That(choices.Any(choice => choice.Id == "construction-credit"), Is.True);
            Assert.That(choices.Any(choice => choice.Id == "overclock"), Is.True);
            Assert.That(choices.All(choice => choice.Amount > 0), Is.True);
        }
    }
}
