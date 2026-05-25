# Current Roadmap

## Purpose

This is the active roadmap for Project 147.

Use this file for what is coming next. Use `first-20-tickets.md` as historical foundation progress.

## Current Playable Slice

The first playable slice currently has:

- Grid placement with no-complete-block validation.
- Manual waves, base health, leaks, victory and defeat.
- Scrap economy for tower placement, kills, wave clear and perfect waves.
- Tower selling with a tested partial refund rule.
- Per-tower targeting mode changes.
- Selectable tower upgrade paths.
- Multiple selectable debug level layouts.
- Tested 1x/2x/3x game speed control.
- Active-wave pause/resume without Unity global time scale.
- Four debug towers: railgun, mortar, energy and chemical.
- Poison damage over time for chemical towers.
- Mixed waves with basic, fast, armoured, shielded, burrower, regenerator and boss aliens.
- Shield-first damage for shielded aliens.
- Early-path untargetability for burrowers.
- Health regeneration for regenerator aliens.
- Freeze Pulse and Orbital Strike player abilities.
- Between-wave reward choices.
- Next-wave intel with enemy tags.
- End-of-run summary and session progress tracking.
- Per-level-layout session progress tracking.
- First-pass code-native visuals for towers and aliens.

## Morning Verification

After pulling/opening the latest work:

1. Run all Edit Mode tests.
2. Press Play in the debug scene.
3. Switch level layouts before placing a tower and confirm the board changes.
4. Select tower `4/4` and confirm it is the chemical tower with poison text.
5. Cycle upgrade paths and confirm the selected upgrade name changes.
6. Place towers, then click an existing tower between waves to upgrade using the selected path.
7. Start later waves and confirm `Regenerator` appears in next-wave intel.
8. Confirm green halo regenerator aliens appear.
9. Confirm burrowers are still not shot immediately at spawn.
10. Confirm the final boss wave still completes into victory or defeat.

## Next Slice Candidates

### 1. Better Tower Upgrade Strategy

Goal: make upgrading feel like a choice, not a flat stat bump.

Done:

- Add selectable upgrade paths.
- Keep upgrade path selection tested in pure GameCore.
- Wire selected upgrade path into the debug HUD.

Next:

- Track each tower's upgrade history.
- Show each placed tower's selected path and level more clearly.
- Add status-effect upgrade paths, such as longer slow or stronger poison.
- Fix selling so refund is based on actual upgrade spend, not a same-cost assumption.

### 2. Better Alien Progression

Goal: make waves feel strategically different.

Done:

- Add shields.
- Add burrower untargetability.
- Add regeneration.
- Add resistances, dodge and alien upgrades.

Next:

- Add clearer trait text in wave intel.
- Add wave difficulty ratings.
- Add enemy-side upgrade choices for health, speed, resistance, dodge, shields and regeneration.
- Add counterplay prompts, such as chemical counters regeneration or energy counters shields.

### 3. Defence Loadout

Goal: move from a debug tower carousel to an intentional pre-run tower selection.

Next:

- Add a tested tower unlock model.
- Add a tested loadout slot model.
- Restrict each run to a chosen subset of towers.
- Add a simple pre-run debug panel for changing the loadout.

### 4. Multiple Levels

Goal: prove the game works on more than one board shape.

Done:

- Add tested level layout definitions.
- Add three selectable debug layouts.
- Wire level switching into the debug scene before a run starts.

Next:

- Add level-specific starting scrap, base health and wave count.
- Add layout-specific wave tuning.
- Add level completion stars per layout.

### 5. Campaign Progression

Goal: make runs feed into longer-term progress.

Done:

- Track completed runs, victories, best stars and best wave.
- Track session progress separately for each debug layout.

Next:

- Add unlock rules based on stars.
- Add local save/load for progress.
- Add a simple campaign selection screen later, after the debug scene proves the rules.

### 6. Alien-Side Prototype

Goal: support playing as the invading alien side.

Next:

- Add an alien squad loadout model.
- Add alien squad budget rules.
- Add alien spawn-order planning.
- Add a simple automated defence opponent.
- Reuse existing tower, targeting and damage rules rather than building a separate game.

### 7. More Special Abilities

Goal: make both sides feel expressive.

Next:

- Add a defensive shield burst.
- Add a tower overcharge ability.
- Add alien decoy or sabotage ability.
- Add alien tunnel burst ability.
- Keep ability effects in GameCore and scene feedback in UnityPresentation.

### 8. Game Feel

Goal: make the prototype easier to read and more fun to play.

Next:

- Add health bars.
- Add poison, shield and regeneration indicators.
- Add clearer hit feedback.
- Add better wave-start and wave-clear feedback.
- Add placeholder sound hooks only after the visual feedback is readable.

### 9. Data Cleanup

Goal: avoid letting debug config become a dumping ground.

Next:

- Split tower, alien, ability, level and reward tuning into separate assets when iteration becomes painful.
- Keep ScriptableObjects as data wrappers around tested GameCore models.
- Do not move logic into Unity scene components.

### 10. Monetisation Preparation

Goal: prepare for monetisation without adding it too early.

Next:

- Define analytics events for retention and balance.
- Mark natural rewarded-ad moments.
- Keep purchases, subscriptions and backend work out until the core loop has enough retention signal.

## Not Yet

Do not start these yet:

- Ads SDKs.
- Subscriptions.
- Store purchases.
- Backend accounts.
- Final art pipeline.
- Polished mobile UI.

These matter, but adding them before the loop is fun would create noise and slow us down.
