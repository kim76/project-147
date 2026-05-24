using System;

namespace Project147.GameCore.Combat
{
    public sealed class DamageResolver
    {
        public DamageResult Resolve(DamageRequest request, AlienDefinition target)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            var resistance = target.GetResistance(request.DamageType);
            var finalAmount = request.Amount * (1 - resistance);

            return new DamageResult(request.Amount, resistance, finalAmount);
        }
    }
}

