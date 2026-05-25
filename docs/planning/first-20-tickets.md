# First 20 Tickets

These tickets are intentionally small. Each should leave the project in a better-tested state.

## Current Progress

- Unity project created in `game/` using the Universal 3D template.
- Initial assembly boundaries added under `game/Assets/Project147`.
- First `GridCoordinate` domain model and Edit Mode tests added.
- `GridBounds` and `TacticalGrid` domain models added with Edit Mode tests.
- `GridPathfinder` added with shortest-path and no-path Edit Mode tests.
- `TowerPlacementValidator` added with no-complete-block placement tests.
- Tower placement validation now supports multiple spawn points and returns a new grid when placement is applied.
- Initial combat definitions added for damage types, targeting modes, towers and aliens.
- Damage request, result and resolver added with resistance calculation tests.
- Alien health state and attack resolution added with tests.
- Tower target selection added for first, last, closest, strongest and weakest modes.
- Tower fire cooldown state and attack step added with tests.
- Debug grid scene creator added under `Project147 > Debug > Create Grid Scene`.
- Debug grid scene now supports click-to-place tower cells using the placement validator.
- Debug grid scene now spawns a simple alien marker that follows the calculated path.
- Playable first-slice loop added with waves, base health, currency, tower fire and alien leaks.
- Tower placement hover preview added with range and valid/invalid feedback.
- Tower shot feedback added with short shot lines and alien hit flashes.
- First-slice tuning moved into `DebugFirstSliceConfig` data asset.
- Combat now supports deterministic crit and dodge rolls for future tower and alien upgrades.
- Wave definition and spawn scheduling moved into tested GameCore level models.
- Tower upgrade definitions added with tested damage, fire-rate, range and crit stat changes.
- Alien upgrade definitions added with tested health, speed, reward, dodge and resistance changes.
- Basic alien status effects added with tested slow duration and movement-speed rules.
- Reward calculation moved into tested GameCore level models with alien kill, wave clear and perfect-wave rewards.
- Unity Test Runner confirmed available through `Window > General > Test Runner`.
- Command-line test run not attempted while the Unity project is open in the editor.

## Foundation

1. Create Unity project in `game/` inside the repository.
2. Add baseline Unity folder structure.
3. Add Edit Mode and Play Mode test assemblies.
4. Add initial game data folder and naming conventions.
5. Configure Unity version control settings for visible meta files and force text serialisation.

## Core Simulation

6. Implement grid coordinate model with tests.
7. Implement blocked and unblocked cell state with tests.
8. Implement shortest-path lookup with tests.
9. Implement placement validation with tests.
10. Implement no-complete-block rule with tests.
11. Implement alien path-following model with tests.

## Combat

12. Implement tower definition data model with tests.
13. Implement alien definition data model with tests.
14. Implement tower targeting modes with tests.
15. Implement damage types and resistance calculation with tests.
16. Implement crit and dodge calculation with controlled randomness and tests.
17. Implement basic status effects with tests.

## Level Loop

18. Implement wave definition model with tests.
19. Implement wave scheduler with tests.
20. Implement reward calculation with tests.
21. Build first Unity scene that visualises the tested simulation.

## Rule

Do not start the first scene by hardcoding gameplay in the scene. The scene should display the tested systems built in the earlier tickets.
