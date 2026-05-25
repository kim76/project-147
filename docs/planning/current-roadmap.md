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
- Mixed enemy waves with basic, fast, armoured and boss debug aliens.
- Alien upgrades across later waves.
- A final-wave boss alien with distinct stats, wave-intel tag and first-pass visual profile.
- Freeze Pulse player ability with cooldown and debug feedback.
- Orbital Strike player ability with cooldown, resistance-aware damage and debug feedback.
- Between-wave reward choices drawn as a random 3-option offer from a larger reward pool.
- Reward effects for extra scrap, base repair, next-tower discount, next-wave tower damage and next-wave tower fire rate.
- Next-wave intel showing upcoming enemy composition and simple tags before the wave starts.
- End-of-run summary for outcome, waves, kills, leaks, scrap, rewards and ability use.
- First-slice tuning grouped by level, towers, aliens, upgrades and abilities.
- First-pass in-scene visual profiles for towers and aliens.
- Debug HUD, hover range preview, shot feedback and event feed.

## Immediate Verification

Before the next development slice:

1. Run Edit Mode tests in Unity.
2. Confirm the new boss wave and wave-intel tests appear and pass.
3. Press Play.
4. Progress to the final wave.
5. Confirm the `Next Wave` panel shows a `Boss` tag before the final wave.
6. Start the final wave and confirm a large purple boss alien appears.
7. Confirm victory still triggers after the boss wave is cleared.
8. Confirm the run summary panel appears with kill, reward and ability counts.

## Next Slice Candidates

### 1. Make Enemies Visibly Different

Goal: introduce enemy variety without adding messy scene-only logic.

Deliverables:

- Done: add basic, fast and armoured alien definitions to the debug config.
- Done: add a boss alien definition to the debug config.
- Done: add a tested alien wave composition model.
- Done: spawn mixed waves instead of one repeated alien.
- Done: show visual differences for basic, fast, armoured and boss aliens.
- Done: show enemy type in event/debug information where useful.
- Done: tag boss waves in next-wave intel.

Why next: tower variety is now meaningful, but enemy variety is still too flat.

### 2. Add Player Ability Prototype

Goal: introduce the first special ability in a controlled, testable way.

Deliverables:

- Done: add Freeze Pulse ability definition.
- Done: add cooldown rules in GameCore with tests.
- Done: add ability resolver that applies slow to living aliens.
- Done: wire one HUD button in the debug scene.
- Done: add simple debug visual feedback.
- Done: add Orbital Strike damage ability with tested resistance-aware damage.
- Done: wire a second HUD ability button in the debug scene.

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

- Done: after a non-final wave, offer reward choices before the next wave.
- Done: add tested choice definitions and application rules.
- Done: support extra scrap, base repair and next-tower discount effects.
- Done: support an Overclock reward for next-wave tower damage.
- Done: support a Rapid Loader reward for next-wave tower fire rate.
- Done: display a temporary debug choice panel with 3 random options.
- Done: block the next wave until a reward is chosen.

Why next: this starts the “addictive” part without monetisation or content bloat.

## Recommended Order

1. Verify the final-wave boss in Unity.
2. Verify the post-run summary in Unity.
3. Generate concept boards for final art direction.
4. Add an enemy-side mechanic such as shields or burrowers.
5. Split debug tuning into separate ScriptableObject assets only when the current single asset becomes painful.

## Not Yet

Do not start these yet:

- Ads.
- Subscriptions.
- Store purchases.
- Backend accounts.
- Final art pipeline.
- Polished mobile UI.

These are important, but they should wait until the core loop is fun enough to test honestly.
