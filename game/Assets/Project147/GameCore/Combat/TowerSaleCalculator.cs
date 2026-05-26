using System;

namespace Project147.GameCore.Combat
{
    public sealed class TowerSaleCalculator
    {
        public int CalculateRefund(
            TowerState tower,
            float refundMultiplier)
        {
            if (tower == null)
            {
                throw new ArgumentNullException(nameof(tower));
            }

            if (refundMultiplier < 0 || refundMultiplier > 1)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(refundMultiplier),
                    "Refund multiplier must be between zero and one.");
            }

            return Convert.ToInt32(Math.Round(tower.TotalSpend * refundMultiplier, MidpointRounding.AwayFromZero));
        }
    }
}
