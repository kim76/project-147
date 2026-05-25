using System;
using System.Collections.Generic;

namespace Project147.GameCore.Level
{
    public sealed class WaveIntelBuilder
    {
        public WaveIntelSummary Build(
            int completedWaves,
            WaveDefinition wave,
            string fastAlienId,
            string armouredAlienId,
            string bossAlienId = null,
            string shieldedAlienId = null,
            string burrowerAlienId = null,
            string regeneratorAlienId = null)
        {
            if (completedWaves < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(completedWaves), "Completed waves cannot be negative.");
            }

            if (wave == null)
            {
                throw new ArgumentNullException(nameof(wave));
            }

            if (string.IsNullOrWhiteSpace(fastAlienId))
            {
                throw new ArgumentException("Fast alien id is required.", nameof(fastAlienId));
            }

            if (string.IsNullOrWhiteSpace(armouredAlienId))
            {
                throw new ArgumentException("Armoured alien id is required.", nameof(armouredAlienId));
            }

            var entries = new List<WaveIntelEntry>();
            var tags = new List<string>();
            var hasFast = false;
            var hasArmoured = false;
            var hasShielded = false;
            var hasBurrower = false;
            var hasRegenerator = false;
            var hasBoss = false;

            foreach (var group in wave.Composition.Groups)
            {
                entries.Add(new WaveIntelEntry(group.AlienId, group.Count));

                if (group.AlienId == fastAlienId)
                {
                    hasFast = true;
                }

                if (group.AlienId == armouredAlienId)
                {
                    hasArmoured = true;
                }

                if (!string.IsNullOrWhiteSpace(shieldedAlienId) && group.AlienId == shieldedAlienId)
                {
                    hasShielded = true;
                }

                if (!string.IsNullOrWhiteSpace(burrowerAlienId) && group.AlienId == burrowerAlienId)
                {
                    hasBurrower = true;
                }

                if (!string.IsNullOrWhiteSpace(regeneratorAlienId) && group.AlienId == regeneratorAlienId)
                {
                    hasRegenerator = true;
                }

                if (!string.IsNullOrWhiteSpace(bossAlienId) && group.AlienId == bossAlienId)
                {
                    hasBoss = true;
                }
            }

            if (entries.Count > 1)
            {
                tags.Add("Mixed");
            }

            if (hasFast)
            {
                tags.Add("Fast");
            }

            if (hasArmoured)
            {
                tags.Add("Armoured");
            }

            if (hasShielded)
            {
                tags.Add("Shielded");
            }

            if (hasBurrower)
            {
                tags.Add("Burrower");
            }

            if (hasRegenerator)
            {
                tags.Add("Regenerator");
            }

            if (hasBoss)
            {
                tags.Add("Boss");
            }

            if (wave.AlienCount >= 10)
            {
                tags.Add("Heavy");
            }

            return new WaveIntelSummary(completedWaves + 1, entries, tags, wave.ClearReward);
        }
    }
}
