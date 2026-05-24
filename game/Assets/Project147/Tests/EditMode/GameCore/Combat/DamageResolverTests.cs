using System;
using System.Collections.Generic;
using NUnit.Framework;
using Project147.GameCore.Combat;

namespace Project147.Tests.EditMode.GameCore.Combat
{
    public sealed class DamageResolverTests
    {
        [Test]
        public void Resolve_WhenTargetHasResistance_ReducesDamageByResistance()
        {
            var target = new AlienDefinition(
                "stoneback-basic",
                100,
                1,
                10,
                new Dictionary<DamageType, float>
                {
                    { DamageType.Kinetic, 0.4f }
                });
            var resolver = new DamageResolver();

            var result = resolver.Resolve(new DamageRequest(50, DamageType.Kinetic), target);

            Assert.That(result.BaseAmount, Is.EqualTo(50));
            Assert.That(result.Resistance, Is.EqualTo(0.4f));
            Assert.That(result.FinalAmount, Is.EqualTo(30).Within(0.0001f));
        }

        [Test]
        public void Resolve_WhenTargetHasNoResistanceForDamageType_UsesFullDamage()
        {
            var target = new AlienDefinition(
                "runner-basic",
                50,
                1,
                5,
                new Dictionary<DamageType, float>());
            var resolver = new DamageResolver();

            var result = resolver.Resolve(new DamageRequest(25, DamageType.Energy), target);

            Assert.That(result.Resistance, Is.EqualTo(0));
            Assert.That(result.FinalAmount, Is.EqualTo(25));
        }

        [Test]
        public void Resolve_WhenTargetHasFullResistance_ReturnsZeroFinalDamage()
        {
            var target = new AlienDefinition(
                "shielded-basic",
                50,
                1,
                5,
                new Dictionary<DamageType, float>
                {
                    { DamageType.Energy, 1 }
                });
            var resolver = new DamageResolver();

            var result = resolver.Resolve(new DamageRequest(25, DamageType.Energy), target);

            Assert.That(result.FinalAmount, Is.EqualTo(0));
        }

        [Test]
        public void Resolve_WhenTargetIsNull_Throws()
        {
            var resolver = new DamageResolver();

            Assert.Throws<ArgumentNullException>(() => resolver.Resolve(
                new DamageRequest(25, DamageType.Energy),
                null));
        }
    }
}

