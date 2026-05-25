using System;
using Project147.GameCore.Combat;

namespace Project147.GameCore.Abilities
{
    public readonly struct PlayerAbilityTargetResult : IEquatable<PlayerAbilityTargetResult>
    {
        public PlayerAbilityTargetResult(AlienState source, AlienState target)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));
            Target = target ?? throw new ArgumentNullException(nameof(target));
        }

        public AlienState Source { get; }

        public AlienState Target { get; }

        public bool Equals(PlayerAbilityTargetResult other)
        {
            return ReferenceEquals(Source, other.Source)
                && ReferenceEquals(Target, other.Target);
        }

        public override bool Equals(object obj)
        {
            return obj is PlayerAbilityTargetResult other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Source, Target);
        }
    }
}
