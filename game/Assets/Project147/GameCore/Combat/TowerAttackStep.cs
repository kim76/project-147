using System;
using System.Collections.Generic;

namespace Project147.GameCore.Combat
{
    public sealed class TowerAttackStep
    {
        private readonly TowerTargetSelector targetSelector;
        private readonly AttackResolver attackResolver;

        public TowerAttackStep(TowerTargetSelector targetSelector, AttackResolver attackResolver)
        {
            this.targetSelector = targetSelector ?? throw new ArgumentNullException(nameof(targetSelector));
            this.attackResolver = attackResolver ?? throw new ArgumentNullException(nameof(attackResolver));
        }

        public TowerAttackStepResult Step(TowerState tower, IEnumerable<TargetCandidate> candidates)
        {
            if (tower == null)
            {
                throw new ArgumentNullException(nameof(tower));
            }

            if (!tower.CanFire)
            {
                return TowerAttackStepResult.NotFired(tower);
            }

            var target = targetSelector.SelectTarget(candidates, tower.TargetingMode);

            if (!target.HasValue)
            {
                return TowerAttackStepResult.NotFired(tower);
            }

            var attack = attackResolver.Resolve(tower.Definition, target.Value.Alien);
            return TowerAttackStepResult.Fired(tower.MarkFired(), attack);
        }
    }
}
