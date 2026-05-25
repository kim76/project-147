# Current Roadmap

## Purpose

This is the working next-task list for Project 147.

Use this file to see what we are doing next. Use `first-20-tickets.md` as historical foundation progress, not as the active plan.

## Current State

The debug first slice has:

- Grid placement with no-complete-block validation.
- Manual waves.
- Base health, leaks, victory and defeat.
- Scrap economy for tower placement, kills, wave clear and perfect waves.
- Tower upgrading between waves.
- Two debug towers:
  - Railgun: faster, applies a short slow effect.
  - Mortar: slower, explosive splash damage.
- Mixed enemy waves with basic, fast and armoured debug aliens.
- Alien upgrades across later waves.
- Freeze Pulse player ability with cooldown and debug feedback.
- First-slice tuning grouped by level, towers, aliens, upgrades and abilities.
- First-pass in-scene visual profiles for towers and aliens.
- Debug HUD, hover range preview, shot feedback and event feed.

## Immediate Verification

Before the next development slice:

1. Run Edit Mode tests in Unity.
2. Confirm the new ability tests appear and pass.
3. Press Play.
4. Start a wave with active aliens.
5. Press `Freeze Pulse`.
6. Confirm the event feed reports slowed aliens and the cooldown begins.

## Next Slice Candidates

### 1. Make Enemies Visibly Different

Goal: introduce enemy variety without adding messy scene-only logic.

Deliverables:

- Done: add basic, fast and armoured alien definitions to the debug config.
- Done: add a tested alien wave composition model.
- Done: spawn mixed waves instead of one repeated alien.
- Done: show visual differences for basic, fast and armoured aliens.
- Done: show enemy type in event/debug information where useful.

Why next: tower variety is now meaningful, but enemy variety is still too flat.

### 2. Add Player Ability Prototype

Goal: introduce the first special ability in a controlled, testable way.

Deliverables:

- Done: add Freeze Pulse ability definition.
- Done: add cooldown rules in GameCore with tests.
- Done: add ability resolver that applies slow to living aliens.
- Done: wire one HUD button in the debug scene.
- Done: add simple debug visual feedback.

Why next: this starts the special-abilities direction without committing to final UI or art.

### 3. Improve Wave And Level Data

Goal: stop the debug config from becoming a giant dumping ground.

Deliverables:

- Done: extract debug tuning into smaller level, tower, alien, upgrade and ability tuning classes.
- Done: keep `DebugFirstSliceConfig` as the Unity-facing entry point.
- Done: add config-level tests for defaults.
- Done: keep the debug scene consuming tested models.

Why next: the first-slice config is becoming too broad. This is where we keep quality high instead of accepting future pain.

### 4. Add First-Pass Visual Profiles

Goal: make the slice easier to read without pretending debug art is final production art.

Deliverables:

- Done: move actor construction out of the scene controller.
- Done: add reusable first-pass tower visual profiles.
- Done: add reusable first-pass alien visual profiles.
- Done: keep visuals code-native for now so they remain fast to iterate.

Why next: the game needs a clearer visual language, but final art should wait until the loop survives more iteration.

### 5. Add Basic In-Run Choice

Goal: make each run less automatic.

Deliverables:

- After a wave, offer a simple choice such as extra scrap, tower discount, damage boost or base repair.
- Add choice definitions and application rules with tests.
- Display a temporary debug choice panel.

Why next: this starts the “addictive” part without monetisation or content bloat.

## Recommended Order

1. Verify config cleanup and first-pass visuals.
2. Add one between-wave choice.
3. Generate concept boards for final art direction.
4. Split debug tuning into separate ScriptableObject assets only when the current single asset becomes painful.

## Not Yet

Do not start these yet:

- Ads.
- Subscriptions.
- Store purchases.
- Backend accounts.
- Final art pipeline.
- Polished mobile UI.

These are important, but they should wait until the core loop is fun enough to test honestly.
