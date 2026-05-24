# First Build Plan

## Goal

Create a small playable vertical slice that proves the core loop without creating throwaway architecture.

This is not a disposable prototype. It is the narrowest version of the real project shape.

## Step 1: Project Foundation

- Create Unity project in `game/`.
- Establish folder structure.
- Add test assemblies.
- Add basic CI or scripted test entry point when practical.
- Add placeholder data definitions.
- Confirm Unity project serialisation uses visible meta files and force text assets.
- Confirm external script editor is set to Visual Studio Code.
- Commit Unity project settings before gameplay work begins.

## Step 2: Core Simulation

- Grid model.
- Pathfinding.
- Tower placement validation.
- No-complete-block rule.
- Alien movement along paths.
- Tower targeting.
- Damage resolution.
- Basic economy rewards.

## Step 3: First Playable Defence Level

- One test map.
- One spawn.
- One base.
- Three towers.
- Three aliens.
- Ten waves.
- Win and loss states.
- Basic mobile-friendly controls.

## Step 4: Upgrade Systems

- Tower upgrade definitions.
- Alien trait definitions.
- Upgrade cost curves.
- Test coverage for stat changes and cost calculations.

## Step 5: Alien Mode Slice

- One defensive layout.
- Squad selection.
- Alien trait upgrades.
- Attack simulation.
- Result screen.

## Step 6: Art Direction Pass

- Style guide.
- Tower silhouettes.
- Alien silhouettes.
- UI mock-up.
- VFX direction.

## Step 7: Monetisation Planning Only

Do not add live ads or purchases until the game loop is worth returning to.

Prepare clean service boundaries early so ads, purchases and subscriptions can be added later without invading game logic.
