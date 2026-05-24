using System;

namespace Project147.GameCore.Combat
{
    public readonly struct AttackResult : IEquatable<AttackResult>
    {
        public AttackResult(DamageResult damage, AlienState target, bool killedTarget)
        {
            Damage = damage;
            Target = target ?? throw new ArgumentNullException(nameof(target));
            KilledTarget = killedTarget;
        }

        public DamageResult Damage { get; }

        public AlienState Target { get; }

        public bool KilledTarget { get; }

        public bool Equals(AttackResult other)
        {
            return Damage.Equals(other.Damage)
                && ReferenceEquals(Target, other.Target)
                && KilledTarget == other.KilledTarget;
        }

        public override bool Equals(object obj)
        {
            return obj is AttackResult other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Damage, Target, KilledTarget);
        }
    }
}

