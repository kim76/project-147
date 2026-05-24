using System;

namespace Project147.GameCore.Combat
{
    public sealed class RandomCombatChanceRoller : ICombatChanceRoller
    {
        private readonly Random random;

        public RandomCombatChanceRoller()
            : this(new Random())
        {
        }

        internal RandomCombatChanceRoller(Random random)
        {
            this.random = random ?? throw new ArgumentNullException(nameof(random));
        }

        public bool Succeeds(float chance)
        {
            if (chance < 0 || chance > 1)
            {
                throw new ArgumentOutOfRangeException(nameof(chance), "Chance must be between zero and one.");
            }

            if (chance <= 0)
            {
                return false;
            }

            if (chance >= 1)
            {
                return true;
            }

            return random.NextDouble() < chance;
        }
    }
}
