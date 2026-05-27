using System.Collections.Generic;
using System.Linq;

namespace Project147.PlatformServices.Audio
{
    public sealed class Project147AudioCatalog
    {
        public IReadOnlyList<AudioEventDefinition> CreateGameplayEvents()
        {
            return new[]
            {
                new AudioEventDefinition(Project147AudioEvents.TowerPlaced, 0.7f, 1f),
                new AudioEventDefinition(Project147AudioEvents.TowerUpgraded, 0.72f, 1.08f),
                new AudioEventDefinition(Project147AudioEvents.TowerSold, 0.58f, 0.92f),
                new AudioEventDefinition(Project147AudioEvents.TowerShot, 0.42f, 1f),
                new AudioEventDefinition(Project147AudioEvents.AlienHit, 0.45f, 1f),
                new AudioEventDefinition(Project147AudioEvents.AlienDestroyed, 0.62f, 0.95f),
                new AudioEventDefinition(Project147AudioEvents.AlienLeaked, 0.82f, 0.8f),
                new AudioEventDefinition(Project147AudioEvents.WaveStarted, 0.66f, 1f),
                new AudioEventDefinition(Project147AudioEvents.WaveCleared, 0.78f, 1.08f),
                new AudioEventDefinition(Project147AudioEvents.AbilityFreezePulse, 0.76f, 0.85f),
                new AudioEventDefinition(Project147AudioEvents.AbilityOrbitalStrike, 0.88f, 0.92f),
                new AudioEventDefinition(Project147AudioEvents.AbilityShieldBurst, 0.72f, 1.05f),
                new AudioEventDefinition(Project147AudioEvents.AbilityTowerOvercharge, 0.76f, 1.16f),
                new AudioEventDefinition(Project147AudioEvents.RewardSelected, 0.68f, 1.05f),
                new AudioEventDefinition(Project147AudioEvents.RunVictory, 0.9f, 1f),
                new AudioEventDefinition(Project147AudioEvents.RunDefeat, 0.9f, 0.82f)
            };
        }

        public AudioEventDefinition GetRequiredEvent(string id)
        {
            return CreateGameplayEvents().Single(definition => definition.Id == id);
        }
    }
}
