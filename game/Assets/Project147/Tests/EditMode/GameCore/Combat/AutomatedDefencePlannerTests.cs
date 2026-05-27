using System;
using System.Linq;
using NUnit.Framework;
using Project147.GameCore.Combat;
using Project147.GameCore.Grid;

namespace Project147.Tests.EditMode.GameCore.Combat
{
    public sealed class AutomatedDefencePlannerTests
    {
        [Test]
        public void CreatePlan_WhenBudgetAndCellsAllow_PlansValidPlacements()
        {
            var planner = CreatePlanner();
            var grid = new TacticalGrid(new GridBounds(5, 3));
            var spawn = new GridCoordinate(0, 1);
            var goal = new GridCoordinate(4, 1);

            var plan = planner.CreatePlan(
                grid,
                spawn,
                goal,
                new[] { CreateTower("railgun", 40, 10, 1) },
                100,
                2);

            Assert.That(plan.Placements, Has.Count.EqualTo(2));
            Assert.That(plan.TotalCost, Is.EqualTo(80));
            Assert.That(plan.IsWithinBudget, Is.True);
            Assert.That(plan.Placements.All(placement => placement.Coordinate != spawn && placement.Coordinate != goal), Is.True);

            var plannedGrid = ApplyPlan(grid, plan);
            Assert.That(new GridPathfinder().HasPath(plannedGrid, spawn, goal), Is.True);
        }

        [Test]
        public void CreatePlan_WhenBestTowerIsAffordable_UsesHighestPressureTower()
        {
            var planner = CreatePlanner();
            var grid = new TacticalGrid(new GridBounds(5, 3));

            var plan = planner.CreatePlan(
                grid,
                new GridCoordinate(0, 1),
                new GridCoordinate(4, 1),
                new[]
                {
                    CreateTower("cheap", 20, 5, 1),
                    CreateTower("heavy", 50, 30, 1)
                },
                50,
                1);

            Assert.That(plan.Placements[0].Tower.Id, Is.EqualTo("heavy"));
        }

        [Test]
        public void CreatePlan_WhenBestTowerIsTooExpensive_UsesAffordableTower()
        {
            var planner = CreatePlanner();
            var grid = new TacticalGrid(new GridBounds(5, 3));

            var plan = planner.CreatePlan(
                grid,
                new GridCoordinate(0, 1),
                new GridCoordinate(4, 1),
                new[]
                {
                    CreateTower("cheap", 20, 5, 1),
                    CreateTower("heavy", 60, 30, 1)
                },
                40,
                1);

            Assert.That(plan.Placements[0].Tower.Id, Is.EqualTo("cheap"));
        }

        [Test]
        public void CreatePlan_WhenBudgetCannotBuyTower_ReturnsEmptyPlan()
        {
            var planner = CreatePlanner();
            var grid = new TacticalGrid(new GridBounds(5, 3));

            var plan = planner.CreatePlan(
                grid,
                new GridCoordinate(0, 1),
                new GridCoordinate(4, 1),
                new[] { CreateTower("railgun", 40, 10, 1) },
                39,
                3);

            Assert.That(plan.Placements, Is.Empty);
            Assert.That(plan.TotalCost, Is.EqualTo(0));
        }

        [Test]
        public void CreatePlan_WhenMaxPlacementsIsZero_ReturnsEmptyPlan()
        {
            var planner = CreatePlanner();
            var grid = new TacticalGrid(new GridBounds(5, 3));

            var plan = planner.CreatePlan(
                grid,
                new GridCoordinate(0, 1),
                new GridCoordinate(4, 1),
                new[] { CreateTower("railgun", 40, 10, 1) },
                100,
                0);

            Assert.That(plan.Placements, Is.Empty);
        }

        [Test]
        public void CreatePlan_WhenGridIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => CreatePlanner().CreatePlan(
                null,
                new GridCoordinate(0, 1),
                new GridCoordinate(4, 1),
                new[] { CreateTower("railgun", 40, 10, 1) },
                100,
                1));
        }

        [Test]
        public void CreatePlan_WhenTowerOptionsAreEmpty_Throws()
        {
            Assert.Throws<ArgumentException>(() => CreatePlanner().CreatePlan(
                new TacticalGrid(new GridBounds(5, 3)),
                new GridCoordinate(0, 1),
                new GridCoordinate(4, 1),
                Array.Empty<TowerDefinition>(),
                100,
                1));
        }

        [Test]
        public void CreatePlan_WhenBudgetIsNegative_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => CreatePlanner().CreatePlan(
                new TacticalGrid(new GridBounds(5, 3)),
                new GridCoordinate(0, 1),
                new GridCoordinate(4, 1),
                new[] { CreateTower("railgun", 40, 10, 1) },
                -1,
                1));
        }

        public static TowerDefinition CreateTower(string id, int cost, float damage, float fireRate)
        {
            return new TowerDefinition(
                id,
                cost,
                1.75f,
                fireRate,
                damage,
                DamageType.Kinetic,
                TowerTargetingMode.First);
        }

        private static AutomatedDefencePlanner CreatePlanner()
        {
            var pathfinder = new GridPathfinder();
            return new AutomatedDefencePlanner(pathfinder, new TowerPlacementValidator(pathfinder));
        }

        private static TacticalGrid ApplyPlan(TacticalGrid grid, AutomatedDefencePlan plan)
        {
            var current = grid;

            foreach (var placement in plan.Placements)
            {
                current = current.WithBlockedCell(placement.Coordinate);
            }

            return current;
        }
    }
}
