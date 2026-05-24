using System;

namespace Project147.GameCore.Combat
{
    public readonly struct TowerAttackStepResult : IEquatable<TowerAttackStepResult>
    {
        private TowerAttackStepResult(TowerState tower, bool fired, AttackResult? attack)
        {
            Tower = tower ?? throw new ArgumentNullException(nameof(tower));
            DidFire = fired;
            Attack = attack;
        }

        public TowerState Tower { get; }

        public bool DidFire { get; }

        public AttackResult? Attack { get; }

        public static TowerAttackStepResult Fired(TowerState tower, AttackResult attack)
        {
            return new TowerAttackStepResult(tower, true, attack);
        }

        public static TowerAttackStepResult NotFired(TowerState tower)
        {
            return new TowerAttackStepResult(tower, false, null);
        }

        public bool Equals(TowerAttackStepResult other)
        {
            return ReferenceEquals(Tower, other.Tower)
                && DidFire == other.DidFire
                && Nullable.Equals(Attack, other.Attack);
        }

        public override bool Equals(object obj)
        {
            return obj is TowerAttackStepResult other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Tower, DidFire, Attack);
        }
    }
}
