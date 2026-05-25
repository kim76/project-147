using System;

namespace Project147.GameCore.Abilities
{
    public sealed class PlayerAbilityState
    {
        public PlayerAbilityState(PlayerAbilityDefinition definition)
            : this(definition, 0)
        {
        }

        private PlayerAbilityState(PlayerAbilityDefinition definition, float remainingCooldownSeconds)
        {
            Definition = definition ?? throw new ArgumentNullException(nameof(definition));

            if (remainingCooldownSeconds < 0 || remainingCooldownSeconds > definition.CooldownSeconds)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(remainingCooldownSeconds),
                    "Remaining ability cooldown must be between zero and full cooldown.");
            }

            RemainingCooldownSeconds = remainingCooldownSeconds;
        }

        public PlayerAbilityDefinition Definition { get; }

        public float RemainingCooldownSeconds { get; }

        public bool CanActivate
        {
            get { return RemainingCooldownSeconds <= 0; }
        }

        public PlayerAbilityState Activate()
        {
            if (!CanActivate)
            {
                throw new InvalidOperationException("Player ability is still on cooldown.");
            }

            return new PlayerAbilityState(Definition, Definition.CooldownSeconds);
        }

        public PlayerAbilityState Tick(float deltaSeconds)
        {
            if (deltaSeconds < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(deltaSeconds), "Delta seconds cannot be negative.");
            }

            if (RemainingCooldownSeconds <= 0)
            {
                return this;
            }

            return new PlayerAbilityState(
                Definition,
                Math.Max(0, RemainingCooldownSeconds - deltaSeconds));
        }
    }
}
