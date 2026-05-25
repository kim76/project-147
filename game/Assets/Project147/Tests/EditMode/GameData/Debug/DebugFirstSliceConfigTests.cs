using System.Linq;
using NUnit.Framework;
using Project147.GameData.Debug;
using UnityEngine;

namespace Project147.Tests.EditMode.GameData.Debug
{
    public sealed class DebugFirstSliceConfigTests
    {
        [Test]
        public void CreateTowerDefinitions_ReturnsRailgunMortarAndEnergyTower()
        {
            var config = ScriptableObject.CreateInstance<DebugFirstSliceConfig>();

            var towers = config.CreateTowerDefinitions();

            Assert.That(towers.Count, Is.EqualTo(3));
            Assert.That(towers[0].Id, Is.EqualTo("debug-railgun"));
            Assert.That(towers[0].StatusEffects.Count, Is.EqualTo(1));
            Assert.That(towers[1].Id, Is.EqualTo("debug-mortar"));
            Assert.That(towers[1].SplashRadius, Is.GreaterThan(0));
            Assert.That(towers[2].Id, Is.EqualTo("debug-energy"));
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
            Assert.That(wave.SpawnEntries.Any(entry => entry.AlienId == config.ShieldedAlienId), Is.False);
            Assert.That(wave.SpawnEntries.Any(entry => entry.AlienId == config.BossAlienId), Is.False);
        }

        [Test]
        public void CreateWaveDefinition_WhenLateWave_ReturnsShieldedAlienId()
        {
            var config = ScriptableObject.CreateInstance<DebugFirstSliceConfig>();

            var wave = config.CreateWaveDefinition(3);

            Assert.That(wave.SpawnEntries.Any(entry => entry.AlienId == config.ShieldedAlienId), Is.True);
        }

        [Test]
        public void CreateWaveDefinition_WhenFinalWave_ReturnsBossAlienId()
        {
            var config = ScriptableObject.CreateInstance<DebugFirstSliceConfig>();

            var wave = config.CreateWaveDefinition(config.TotalWaves - 1);

            Assert.That(wave.SpawnEntries.Any(entry => entry.AlienId == config.BossAlienId), Is.True);
            Assert.That(wave.SpawnEntries.Count(entry => entry.AlienId == config.BossAlienId), Is.EqualTo(1));
        }

        [Test]
        public void CreateAlienDefinition_WhenBossId_ReturnsBossAlien()
        {
            var config = ScriptableObject.CreateInstance<DebugFirstSliceConfig>();

            var boss = config.CreateAlienDefinition(config.BossAlienId, 0);

            Assert.That(boss.Id, Is.EqualTo(config.BossAlienId));
            Assert.That(boss.MaxHealth, Is.GreaterThan(200));
            Assert.That(boss.Reward, Is.GreaterThan(50));
        }

        [Test]
        public void CreateAlienDefinition_WhenShieldedId_ReturnsShieldedAlien()
        {
            var config = ScriptableObject.CreateInstance<DebugFirstSliceConfig>();

            var shielded = config.CreateAlienDefinition(config.ShieldedAlienId, 0);

            Assert.That(shielded.Id, Is.EqualTo(config.ShieldedAlienId));
            Assert.That(shielded.ShieldCapacity, Is.GreaterThan(0));
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

            Assert.That(choices.Count, Is.EqualTo(6));
            Assert.That(choices.Any(choice => choice.Id == "salvage-drop"), Is.True);
            Assert.That(choices.Any(choice => choice.Id == "field-repair"), Is.True);
            Assert.That(choices.Any(choice => choice.Id == "construction-credit"), Is.True);
            Assert.That(choices.Any(choice => choice.Id == "overclock"), Is.True);
            Assert.That(choices.Any(choice => choice.Id == "rapid-loader"), Is.True);
            Assert.That(choices.All(choice => choice.Amount > 0), Is.True);
        }
    }
}
