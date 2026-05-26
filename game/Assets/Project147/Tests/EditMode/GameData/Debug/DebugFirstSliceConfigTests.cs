using System.Linq;
using NUnit.Framework;
using Project147.GameCore.Grid;
using Project147.GameData.Debug;
using UnityEngine;

namespace Project147.Tests.EditMode.GameData.Debug
{
    public sealed class DebugFirstSliceConfigTests
    {
        [Test]
        public void CreateTowerDefinitions_ReturnsDebugTowerLoadout()
        {
            var config = ScriptableObject.CreateInstance<DebugFirstSliceConfig>();

            var towers = config.CreateTowerDefinitions();

            Assert.That(towers.Count, Is.EqualTo(4));
            Assert.That(towers[0].Id, Is.EqualTo("debug-railgun"));
            Assert.That(towers[0].StatusEffects.Count, Is.EqualTo(1));
            Assert.That(towers[1].Id, Is.EqualTo("debug-mortar"));
            Assert.That(towers[1].SplashRadius, Is.GreaterThan(0));
            Assert.That(towers[2].Id, Is.EqualTo("debug-energy"));
            Assert.That(towers[3].Id, Is.EqualTo("debug-chemical"));
            Assert.That(towers[3].StatusEffects.Single().DamagePerSecond, Is.GreaterThan(0));
        }

        [Test]
        public void CreateTowerUpgradeDefinitions_ReturnsDistinctUpgradePaths()
        {
            var config = ScriptableObject.CreateInstance<DebugFirstSliceConfig>();

            var upgrades = config.CreateTowerUpgradeDefinitions();

            Assert.That(upgrades.Count, Is.EqualTo(4));
            Assert.That(upgrades.Select(upgrade => upgrade.Id), Does.Contain("debug-upgrade-damage"));
            Assert.That(upgrades.Select(upgrade => upgrade.Id), Does.Contain("debug-upgrade-rapid"));
            Assert.That(upgrades.Select(upgrade => upgrade.Id), Does.Contain("debug-upgrade-range"));
            Assert.That(upgrades.Select(upgrade => upgrade.Id), Does.Contain("debug-upgrade-status"));
            Assert.That(upgrades.Single(upgrade => upgrade.Id == "debug-upgrade-status").StatusDurationMultiplier, Is.GreaterThan(1));
            Assert.That(upgrades.Single(upgrade => upgrade.Id == "debug-upgrade-status").StatusDamageMultiplier, Is.GreaterThan(1));
            Assert.That(upgrades.Single(upgrade => upgrade.Id == "debug-upgrade-status").StatusMovementSpeedMultiplier, Is.LessThan(1));
            Assert.That(upgrades.Select(upgrade => upgrade.Cost).Distinct().Single(), Is.EqualTo(75));
        }

        [Test]
        public void CreateTowerLoadoutPlans_ReturnsThreeTowerPlans()
        {
            var config = ScriptableObject.CreateInstance<DebugFirstSliceConfig>();

            var plans = config.CreateTowerLoadoutPlans();

            Assert.That(plans.Count, Is.EqualTo(3));
            Assert.That(plans.Select(plan => plan.Id), Does.Contain("debug-loadout-balanced"));
            Assert.That(plans.Select(plan => plan.Id), Does.Contain("debug-loadout-status"));
            Assert.That(plans.Select(plan => plan.Id), Does.Contain("debug-loadout-heavy"));
            Assert.That(plans.All(plan => plan.Towers.Count == 3), Is.True);
        }

        [Test]
        public void CreateLevelLayouts_ReturnsDebugLayouts()
        {
            var config = ScriptableObject.CreateInstance<DebugFirstSliceConfig>();

            var layouts = config.CreateLevelLayouts();

            Assert.That(layouts.Count, Is.EqualTo(3));
            Assert.That(layouts.Select(layout => layout.Id), Does.Contain("debug-relay-yard"));
            Assert.That(layouts.Select(layout => layout.Id), Does.Contain("debug-switchback"));
            Assert.That(layouts.Select(layout => layout.Id), Does.Contain("debug-twin-lane"));
            Assert.That(layouts.All(layout => layout.Width > 0), Is.True);
            Assert.That(layouts.All(layout => layout.Height > 0), Is.True);
        }

        [Test]
        public void CreateLevelLayouts_AllLayoutsHaveValidPath()
        {
            var config = ScriptableObject.CreateInstance<DebugFirstSliceConfig>();
            var pathfinder = new GridPathfinder();

            foreach (var layout in config.CreateLevelLayouts())
            {
                var grid = new TacticalGrid(new GridBounds(layout.Width, layout.Height), layout.BlockedCells);

                var path = pathfinder.FindShortestPath(grid, layout.Spawn, layout.Goal);

                Assert.That(path, Is.Not.Empty, layout.Id);
            }
        }

        [Test]
        public void CreateLevelDefinitions_ReturnsLayoutSpecificRunSettings()
        {
            var config = ScriptableObject.CreateInstance<DebugFirstSliceConfig>();

            var levels = config.CreateLevelDefinitions();

            Assert.That(levels.Count, Is.EqualTo(3));
            Assert.That(levels.Select(level => level.Layout.Id), Does.Contain("debug-relay-yard"));
            Assert.That(levels.Select(level => level.Layout.Id), Does.Contain("debug-switchback"));
            Assert.That(levels.Select(level => level.Layout.Id), Does.Contain("debug-twin-lane"));
            Assert.That(levels.All(level => level.StartingCurrency >= 0), Is.True);
            Assert.That(levels.All(level => level.BaseHealth > 0), Is.True);
            Assert.That(levels.All(level => level.TotalWaves > 0), Is.True);
            Assert.That(levels.Select(level => level.TotalWaves).Distinct().Count(), Is.GreaterThan(1));
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
            Assert.That(wave.SpawnEntries.Any(entry => entry.AlienId == config.BurrowerAlienId), Is.True);
            Assert.That(wave.SpawnEntries.Any(entry => entry.AlienId == config.RegeneratorAlienId), Is.False);
            Assert.That(wave.SpawnEntries.Any(entry => entry.AlienId == config.BossAlienId), Is.False);
        }

        [Test]
        public void CreateWaveDefinition_WhenRunTotalWavesIsDifferent_UsesRunTotalForBossWave()
        {
            var config = ScriptableObject.CreateInstance<DebugFirstSliceConfig>();

            var wave = config.CreateWaveDefinition(5, 6);

            Assert.That(wave.SpawnEntries.Any(entry => entry.AlienId == config.BossAlienId), Is.True);
        }

        [Test]
        public void CreateWaveDefinition_WhenLateWave_ReturnsShieldedAlienId()
        {
            var config = ScriptableObject.CreateInstance<DebugFirstSliceConfig>();

            var wave = config.CreateWaveDefinition(3);

            Assert.That(wave.SpawnEntries.Any(entry => entry.AlienId == config.ShieldedAlienId), Is.True);
            Assert.That(wave.SpawnEntries.Any(entry => entry.AlienId == config.RegeneratorAlienId), Is.True);
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
        public void CreateAlienDefinition_WhenBurrowerId_ReturnsBurrowerAlien()
        {
            var config = ScriptableObject.CreateInstance<DebugFirstSliceConfig>();

            var burrower = config.CreateAlienDefinition(config.BurrowerAlienId, 0);

            Assert.That(burrower.Id, Is.EqualTo(config.BurrowerAlienId));
            Assert.That(burrower.TargetableAfterPathProgress, Is.GreaterThan(0));
        }

        [Test]
        public void CreateAlienDefinition_WhenRegeneratorId_ReturnsRegeneratorAlien()
        {
            var config = ScriptableObject.CreateInstance<DebugFirstSliceConfig>();

            var regenerator = config.CreateAlienDefinition(config.RegeneratorAlienId, 0);

            Assert.That(regenerator.Id, Is.EqualTo(config.RegeneratorAlienId));
            Assert.That(regenerator.HealthRegenerationPerSecond, Is.GreaterThan(0));
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
        public void CreateShieldBurstAbilityDefinition_ReturnsBaseShieldAbility()
        {
            var config = ScriptableObject.CreateInstance<DebugFirstSliceConfig>();

            var ability = config.CreateShieldBurstAbilityDefinition();

            Assert.That(ability.Id, Is.EqualTo("debug-shield-burst"));
            Assert.That(ability.CooldownSeconds, Is.GreaterThan(0));
            Assert.That(ability.HasBaseShield, Is.True);
            Assert.That(ability.BaseShieldAmount, Is.GreaterThan(0));
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
