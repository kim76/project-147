using System;
using NUnit.Framework;
using Project147.GameCore.Grid;

namespace Project147.Tests.EditMode.GameCore.Grid
{
    public sealed class TowerPlacementResultTests
    {
        [Test]
        public void Valid_ReturnsValidResultWithNoFailureReason()
        {
            var result = TowerPlacementResult.Valid();

            Assert.That(result.IsValid, Is.True);
            Assert.That(result.FailureReason, Is.EqualTo(TowerPlacementFailureReason.None));
        }

        [Test]
        public void Invalid_WhenFailureReasonIsNone_Throws()
        {
            Assert.Throws<ArgumentException>(() => TowerPlacementResult.Invalid(TowerPlacementFailureReason.None));
        }
    }
}

