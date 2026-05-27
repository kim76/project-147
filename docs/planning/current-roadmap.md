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
- Actual tower upgrade spend tracking for selling and inspection.
- Multiple selectable debug level layouts.
- Level-specific starting scrap, base health, wave count and perfect-wave bonus.
- Level-specific wave count, spawn pace and clear-reward tuning.
- Selectable three-tower debug loadout presets before a run starts.
- Tested 1x/2x/3x game speed control.
- Active-wave pause/resume without Unity global time scale.
- Four debug towers: railgun, mortar, energy and chemical.
- Poison damage over time for chemical towers.
- Mixed waves with basic, fast, armoured, shielded, burrower, regenerator and boss aliens.
- Shield-first damage for shielded aliens.
- Early-path untargetability for burrowers.
- Health regeneration for regenerator aliens.
- Freeze Pulse, Orbital Strike, Shield Burst and Tower Overcharge player abilities.
- Between-wave reward choices.
- Next-wave intel with enemy tags.
- Next-wave trait text for special aliens.
- Next-wave threat ratings and counter hints.
- End-of-run summary and session progress tracking.
- Per-level-layout session progress tracking.
- Total-star level unlock rules in the debug campaign flow.
- Local save/load for campaign progress.
- Tested analytics event catalogue for retention and balance instrumentation.
- Tested rewarded-ad opportunity catalogue without live ad SDKs.
- Tested local analytics recorder that validates required event properties.
- Tested rewarded-ad offer tracker that enforces per-run offer limits.
- Debug scene records local analytics event counts.
- Debug scene can show fake rewarded-ad offers after wave clears without a live ad SDK.
- First-pass code-native visuals for towers and aliens.
- Health, shield and status markers on active aliens.
- Upgrade pips on upgraded towers.
- Simple wave-start, wave-clear, victory and defeat banners.
- Floating debug damage, critical-hit and dodge feedback.
- Tested tower unlock-state foundation.

## Morning Verification

After pulling/opening the latest work:

1. Run all Edit Mode tests.
2. Press Play in the debug scene.
3. Switch level layouts before placing a tower and confirm the board changes.
4. Confirm the selected level changes starting scrap, base health, wave count or perfect-wave bonus.
5. Confirm the session progress panel sits below the intel panels and does not cover the board.
6. Confirm fresh campaign progress shows only the first level unlocked.
7. Complete a strong run and confirm more levels unlock from total stars.
8. Restart Play mode and confirm saved campaign progress loads.
9. Use `Reset Save` and confirm progress returns to one unlocked level.
10. Cycle pre-run tower loadouts and confirm the selected tower list changes to `1/3`.
11. Select the status loadout and confirm the chemical tower is available.
12. Cycle upgrade paths and confirm the selected upgrade path name changes.
13. Place towers, then click an existing tower between waves to inspect and upgrade it.
14. Confirm the tower detail panel shows level, targeting, invested scrap, current stats and upgrade history.
15. Sell an upgraded tower and confirm the refund reflects actual spend.
16. During a wave, use Shield Burst and confirm base shield increases.
17. During a wave with at least one tower, use Tower Overcharge and confirm the modifier line shows active-wave damage and fire-rate boosts.
18. Start later waves and confirm trait text, threat rating, counter hints and `Regenerator` appear in next-wave intel.
19. Confirm active aliens show health bars and shielded aliens show shield bars.
20. Confirm poisoned or slowed aliens show a small status marker.
21. Upgrade towers and confirm upgraded towers show visible pips.
22. Confirm wave start, wave clear, victory and defeat banners appear.
23. Confirm green halo regenerator aliens appear.
24. Confirm burrowers are still not shot immediately at spawn.
25. Confirm the final boss wave still completes into victory or defeat.

## Next Slice Candidates

### 1. Better Tower Upgrade Strategy

Goal: make upgrading feel like a choice, not a flat stat bump.

Done:

- Add selectable upgrade paths.
- Keep upgrade path selection tested in pure GameCore.
- Wire selected upgrade path into the debug HUD.
- Track each tower's upgrade history and actual upgrade spend.
- Show clicked tower details in the debug HUD.
- Add a status upgrade path for slow and poison towers.
- Fix selling so refund is based on actual upgrade spend.

Next:

- Make upgrade paths tower-specific when the generic paths start feeling too broad.
- Add visual upgrade markers to placed towers.

### 2. Better Alien Progression

Goal: make waves feel strategically different.

Done:

- Add shields.
- Add burrower untargetability.
- Add regeneration.
- Add resistances, dodge and alien upgrades.
- Add simple threat ratings in wave intel.
- Add counter hints for shielded, regenerator, burrower, armoured and boss waves.
- Add clearer trait text in wave intel.
- Add enemy-side upgrade choices for health, speed, resistance, dodge, shields and regeneration.

Next:

- Wire alien upgrade choices into the alien-side prototype flow.

### 3. Defence Loadout

Goal: move from a debug tower carousel to an intentional pre-run tower selection.

Done:

- Add tested tower loadout plan and plan-set models.
- Restrict each run to a selected three-tower debug loadout.
- Add a simple pre-run debug control for changing the loadout.
- Add a tested tower unlock model.

Next:

- Replace preset loadouts with explicit slot editing.

### 4. Multiple Levels

Goal: prove the game works on more than one board shape.

Done:

- Add tested level layout definitions.
- Add three selectable debug layouts.
- Wire level switching into the debug scene before a run starts.
- Add level-specific starting scrap, base health and wave count.
- Add level-specific wave tuning for spawn counts, spawn pace and clear rewards.

Next:

- Add level completion stars per layout to a real campaign screen later.

### 5. Campaign Progression

Goal: make runs feed into longer-term progress.

Done:

- Track completed runs, victories, best stars and best wave.
- Track session progress separately for each debug layout.
- Add unlock rules based on stars.
- Add a progress snapshot/restore model for local save work.
- Add local save/load for progress.

Next:

- Add a simple campaign selection screen later, after the debug scene proves the rules.

### 6. Alien-Side Prototype

Goal: support playing as the invading alien side.

Done:

- Add a tested budgeted alien squad loadout model.
- Add a tested alien spawn-order planning model.
- Add a tested alien upgrade choice plan model.

Next:

- Add a simple automated defence opponent.
- Reuse existing tower, targeting and damage rules rather than building a separate game.

### 7. More Special Abilities

Goal: make both sides feel expressive.

Done:

- Add Shield Burst as a defensive base-shield ability.
- Add Tower Overcharge as an active-wave tower damage and fire-rate ability.

Next:

- Add alien decoy or sabotage ability.
- Add alien tunnel burst ability.
- Keep ability effects in GameCore and scene feedback in UnityPresentation.

### 8. Game Feel

Goal: make the prototype easier to read and more fun to play.

Done:

- Add floating debug damage, critical-hit and dodge feedback.

Next:

- Add more readable ability feedback.
- Add placeholder sound hooks only after the visual feedback is readable.

### 9. Data Cleanup

Goal: avoid letting debug config become a dumping ground.

Next:

- Split tower, alien, ability, level and reward tuning into separate assets when iteration becomes painful.
- Keep ScriptableObjects as data wrappers around tested GameCore models.
- Do not move logic into Unity scene components.

### 10. Monetisation Preparation

Goal: prepare for monetisation without adding it too early.

Done:

- Wire analytics event catalogue into a fake/local analytics service.
- Wire rewarded-ad opportunities into fake run-limit tracking before adding live SDKs.
- Add debug-only rewarded-ad prompt surfaces.

Next:

- Add stronger analytics coverage for economy, churn and difficulty tuning.
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
