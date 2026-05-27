using System;

namespace Project147.GameCore.Level
{
    public sealed class WaveAlienRoster
    {
        public WaveAlienRoster(
            string basicAlienId,
            string fastAlienId,
            string armouredAlienId,
            string shieldedAlienId,
            string burrowerAlienId,
            string regeneratorAlienId,
            string bossAlienId)
        {
            BasicAlienId = RequireId(basicAlienId, nameof(basicAlienId));
            FastAlienId = RequireId(fastAlienId, nameof(fastAlienId));
            ArmouredAlienId = RequireId(armouredAlienId, nameof(armouredAlienId));
            ShieldedAlienId = RequireId(shieldedAlienId, nameof(shieldedAlienId));
            BurrowerAlienId = RequireId(burrowerAlienId, nameof(burrowerAlienId));
            RegeneratorAlienId = RequireId(regeneratorAlienId, nameof(regeneratorAlienId));
            BossAlienId = RequireId(bossAlienId, nameof(bossAlienId));
        }

        public string BasicAlienId { get; }

        public string FastAlienId { get; }

        public string ArmouredAlienId { get; }

        public string ShieldedAlienId { get; }

        public string BurrowerAlienId { get; }

        public string RegeneratorAlienId { get; }

        public string BossAlienId { get; }

        private static string RequireId(string value, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Wave alien roster ids are required.", parameterName);
            }

            return value;
        }
    }
}
