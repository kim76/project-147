# Playable First Slice

## Purpose

The playable first slice proves the core tower-defence loop using debug visuals.

It is not the MVP and it is not final art. It is a playable test bed for the rules built so far.

For the active next-task list, see `current-roadmap.md`.

## Current Loop

- Player places towers between waves.
- Player can switch between three debug level layouts before placing towers.
- Each debug level can have its own starting scrap, base health, wave count and perfect-wave bonus.
- Each debug level can have its own wave size, spawn pace and clear reward tuning.
- Player can switch between three pre-run debug tower loadout presets before placing towers.
- Player can switch between the three tower types inside the selected loadout before placing.
- Player can switch between four tower upgrade paths before upgrading; upgrade path is separate from upgrade count.
- Railgun, mortar, energy and chemical towers use distinct first-pass debug visual profiles.
- Railgun applies a short slow effect, mortar deals splash damage, energy provides direct single-target damage, and chemical applies poison damage over time.
- Hovering a cell previews tower range and whether placement is valid.
- Towers cost scrap.
- Clicking an existing tower between waves inspects it and upgrades it with the selected upgrade path if enough scrap is available.
- Toggling `Sell Mode` and clicking an existing tower between waves sells it for a partial refund.
- Toggling `Target` and clicking an existing tower between waves cycles its targeting mode.
- Placement is rejected if it blocks all paths.
- Player starts waves manually.
- Player can cycle wave speed between 1x, 2x and 3x.
- Player can pause and resume active waves.
- A `Next Wave` panel previews enemy counts and tags before each wave.
- The `Next Wave` panel shows trait text, a simple threat rating and counter hints.
- Mixed waves spawn basic, fast, armoured, shielded, burrower, regenerator and boss debug aliens.
- Shielded aliens absorb damage into shields before health.
- Burrower aliens cannot be targeted by towers until they have travelled several path steps.
- Regenerator aliens heal over time while alive.
- The final wave includes one boss alien.
- Aliens follow the current path and damage the base if they reach the goal.
- Later waves spawn upgraded aliens, capped by the debug config.
- Towers select targets, fire on cooldown, damage aliens and apply slow or poison effects.
- Tower shots draw a short line and briefly flash the alien.
- Tower hits show floating debug damage, critical-hit or dodge text.
- Active aliens show simple health, shield and status markers.
- Upgraded towers show simple upgrade pips.
- Wave start, wave clear, victory and defeat show short debug banners.
- Freeze Pulse can be triggered during a wave to slow all active aliens.
- Orbital Strike can be triggered during a wave to damage all active aliens.
- Shield Burst can be triggered during a wave to add temporary base shield.
- Tower Overcharge can be triggered during a wave to boost tower damage and fire rate for that wave.
- Shield Burst and Tower Overcharge draw short debug rings when activated.
- Status effects expire automatically.
- Killed aliens award scrap.
- Clearing waves awards bonus scrap.
- Clearing a wave without taking base damage awards a perfect-wave scrap bonus.
- Clearing a non-final wave offers a reward choice before the next wave can start.
- Reward choices show as a random 3-option offer from a larger pool.
- Reward choices can currently add scrap, repair the base, discount the next placed tower, boost next-wave tower damage or boost next-wave tower fire rate.
- A debug event feed shows important placement, upgrade, reward, leak and win/loss events.
- Victory and defeat show a run summary with stars, wave, kill, leak, scrap, reward and ability-use counts.
- Completed runs update per-level session progress with runs, victories, best stars, best waves and best perfect waves.
- Total stars unlock later debug levels.
- Campaign progress is saved locally after completed runs and loaded when the debug scene starts.
- Analytics events and rewarded-ad opportunities are defined as tested catalogues, but no live SDKs are wired in.
- Local analytics recording and rewarded-ad offer limits are testable without connecting external services.
- Placeholder audio events are defined as a tested catalogue and recorded locally, but no real clips play yet.
- The debug scene shows fake rewarded-ad offers after some wave clears, with claim/skip buttons and per-run limits.
- Alien-side upgrade choices now cover health, speed, resistance, dodge, shields and regeneration in pure GameCore.
- The debug scene shows a prototype alien-side planning panel for squad, spawn order, upgrades and automated defence preview.
- The slice ends in victory after all waves or defeat when the base reaches zero health.

## How To Run

1. Wait for Unity to finish compiling.
2. Run Edit Mode tests from `Window > General > Test Runner`.
3. Recreate the debug scene from `Project147 > Debug > Create Grid Scene`.
4. Press Play.
5. Switch between unlocked level layouts before placing a tower if you want a different board.
6. Confirm the selected level changes the resource and wave numbers shown in the HUD.
7. Switch levels and confirm the `Next Wave` panel can change enemy counts or clear reward.
8. Switch tower loadout before placing a tower if you want a different three-tower preset.
9. Place towers by clicking open cells.
10. Select an upgrade path, then click an existing tower between waves to upgrade it.
11. Read the `Next Wave` panel, then press `Start Wave` in the debug HUD.
12. Confirm tower hits show floating damage, critical or dodge text.
13. After a non-final wave, choose one reward from the reward panel.
14. Continue placing or upgrading towers between waves. If you picked the construction credit, the next tower cost is reduced.
15. Toggle `Sell Mode`, click an existing tower, and confirm scrap increases.
16. Toggle `Target`, click an existing tower, and confirm the event feed shows the new targeting mode.
17. Click `Speed` to cycle between 1x, 2x and 3x wave speed.
18. During a wave, click `Pause`, then `Resume`.
19. If you picked `Overclock`, the next wave gets boosted tower damage.
20. If you picked `Rapid Loader`, the next wave gets boosted tower fire rate.
21. Use `Freeze Pulse`, `Orbital Strike`, `Shield Burst` or `Overcharge` during active waves when their cooldowns are ready.
22. Confirm `Overcharge` changes the modifier line to show active wave damage and fire-rate boosts.
23. After a wave clear, claim or skip any `Fake Rewarded Ad` panel that appears.
24. Confirm Shield Burst draws a short blue ring near the base.
25. Confirm Overcharge draws short yellow rings around towers.
26. Confirm the `Analytics`, `Audio` and `Fake ads` counts update in the left HUD.
27. Confirm the `Alien Side Prototype` panel shows squad, order, upgrades and automated defence counts.
28. On victory or defeat, read the run summary panel.
29. Check the `Session Progress` panel, then press `Restart` to confirm session progress remains visible.
30. Stop and restart Play mode to confirm saved progress loads.
31. Use `Reset Save` to clear debug campaign progress when you want a fresh campaign.

The scene creator also creates or reuses `Assets/Project147/GameData/Debug/DebugFirstSliceConfig.asset`. Tune first-slice values there instead of editing the controller.

## Debug Colours

- Green: spawn.
- Yellow: goal/base.
- Dark: fixed blocked cells.
- Blue: calculated path.
- Red: placed tower.
- Purple: alien.
- Cyan capsule: fast alien.
- Stone cube: armoured alien.
- Blue alien with pale shell: shielded alien.
- Brown low-profile alien with dust ring: burrower alien.
- Green alien with halo: regenerator alien.
- Large purple alien with crown: boss alien.
- Red/black tower: railgun.
- Orange/black tower: mortar.
- Teal/yellow tower: energy.
- Green chemical tower: chemical.
- Green range ring: valid tower placement or affordable tower upgrade preview.
- Red range ring: invalid, unaffordable or max-level tower preview.
- Green bar: alien health.
- Cyan bar: alien shield.
- Small green/cyan marker: poison or slow status on an alien.
- Small yellow pips: tower upgrade level markers.
- Cyan pulse ring: Freeze Pulse ability.
- Orange strike ring: Orbital Strike ability.
- Blue base ring: Shield Burst ability.
- Yellow tower rings: Tower Overcharge ability.
- White/yellow/blue text: damage, critical-hit or dodge feedback.

## Deliberate Limits

- Towers can only be placed between waves.
- Towers can only be upgraded between waves.
- Level layout can only be changed before placing towers in a fresh run.
- Later debug levels require enough total stars before the level selector reaches them.
- Level run settings are attached to debug level selection, not final campaign data.
- Tower loadout can only be changed before placing towers in a fresh run.
- Tower upgrade paths currently share one debug cost to keep selling deterministic.
- Towers currently have two upgrade opportunities after placement because max tower level is 3.
- There are four debug tower types.
- Each debug run uses one selected three-tower loadout.
- There are three debug level layouts.
- There are four debug tower upgrade paths.
- The status upgrade path only matters for towers that already apply slow or poison.
- There are seven debug alien types.
- First-slice tuning is grouped in one debug config asset, not final production content data.
- There is no polished UI.
- There is no cloud save or account system.
- There is no mobile touch polish.
- There is no final art.
- There are no live ads, purchase SDKs, subscriptions or backend accounts.
- Fake rewarded-ad offers are debug-only and do not play real ads.
- Placeholder audio hooks are recorded for testing, but there are no real sound effects yet.
- Alien-side planning is visible but not yet playable as a separate run mode.
- Reward choices are debug-tuned and not final progression design.
- The tower discount reward only affects the next placed tower, not upgrades.
- Selling refunds 75% of tower and upgrade spend in the debug slice.
- Targeting cycles through first, last, closest, strongest and weakest.
- Speed controls scale gameplay updates, not Unity's global time scale.
- Pause freezes gameplay updates, not Unity's global time scale.
- Overclock affects damage only, not fire rate.
- Rapid Loader affects fire rate only, not damage.
- Orbital Strike is a debug ability, not final balance.
- Shield Burst is a debug ability, not final balance.
- Tower Overcharge is a debug ability, not final balance.
- Run summary is debug UI, not final progression UX.
- Campaign progress is saved locally for this debug slice.

These limits are intentional. The slice exists to prove the loop before expanding content.
