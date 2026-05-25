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
- Debug HUD, hover range preview, shot feedback and event feed.

## Immediate Verification

Before the next development slice:

1. Run Edit Mode tests in Unity.
2. Confirm the new splash damage tests appear and pass.
3. Press Play.
4. Select the mortar.
5. Place it near a route where aliens cluster.
6. Start a wave and confirm the event feed reports mortar splash hits.

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

- Add one ability definition, probably an orbital strike or freeze pulse.
- Add cooldown and cost rules in GameCore with tests.
- Wire one HUD button in the debug scene.
- Add simple debug visual feedback.

Why next: this starts the special-abilities direction without committing to final UI or art.

### 3. Improve Wave And Level Data

Goal: stop the debug config from becoming a giant dumping ground.

Deliverables:

- Extract wave composition data into a clearer model.
- Keep Unity config asset as tuning input only.
- Add tests around wave composition.
- Keep the debug scene consuming tested models.

Why next: the first-slice config is becoming too broad. This is where we keep quality high instead of accepting future pain.

### 4. Add Basic In-Run Choice

Goal: make each run less automatic.

Deliverables:

- After a wave, offer a simple choice such as extra scrap, tower discount, damage boost or base repair.
- Add choice definitions and application rules with tests.
- Display a temporary debug choice panel.

Why next: this starts the “addictive” part without monetisation or content bloat.

## Recommended Order

1. Verify mortar splash and mixed enemies.
2. Add one player ability.
3. Add one between-wave choice.
4. Clean up debug tuning into smaller data assets if the config keeps growing.
5. Replace debug shapes with first-pass generated concept visuals.

## Not Yet

Do not start these yet:

- Ads.
- Subscriptions.
- Store purchases.
- Backend accounts.
- Final art pipeline.
- Polished mobile UI.

These are important, but they should wait until the core loop is fun enough to test honestly.
