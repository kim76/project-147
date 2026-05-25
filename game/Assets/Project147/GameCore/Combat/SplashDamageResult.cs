using System;

namespace Project147.GameCore.Combat
{
    public readonly struct SplashDamageResult : IEquatable<SplashDamageResult>
    {
        public SplashDamageResult(AlienState source, AlienState target, DamageResult damage)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));
            Target = target ?? throw new ArgumentNullException(nameof(target));
            Damage = damage;
        }

        public AlienState Source { get; }

        public AlienState Target { get; }

        public DamageResult Damage { get; }

        public bool KilledTarget
        {
            get { return Source.IsAlive && !Target.IsAlive; }
        }

        public bool Equals(SplashDamageResult other)
        {
            return ReferenceEquals(Source, other.Source)
                && ReferenceEquals(Target, other.Target)
                && Damage.Equals(other.Damage);
        }

        public override bool Equals(object obj)
        {
            return obj is SplashDamageResult other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Source, Target, Damage);
        }
    }
}
