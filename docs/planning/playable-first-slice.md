# Playable First Slice

## Purpose

The playable first slice proves the core tower-defence loop using debug visuals.

It is not the MVP and it is not final art. It is a playable test bed for the rules built so far.

For the active next-task list, see `current-roadmap.md`.

## Current Loop

- Player places towers between waves.
- Player can switch between three debug tower types before placing.
- Railgun, mortar and energy towers use distinct first-pass debug visual profiles.
- Railgun applies a short slow effect, mortar deals splash damage to nearby aliens, and energy provides a direct single-target damage profile.
- Hovering a cell previews tower range and whether placement is valid.
- Towers cost scrap.
- Clicking an existing tower between waves upgrades it if enough scrap is available.
- Toggling `Sell Mode` and clicking an existing tower between waves sells it for a partial refund.
- Placement is rejected if it blocks all paths.
- Player starts waves manually.
- Player can cycle wave speed between 1x, 2x and 3x.
- A `Next Wave` panel previews enemy counts and tags before each wave.
- Mixed waves spawn basic, fast, armoured, shielded, burrower and boss debug aliens.
- Shielded aliens absorb damage into shields before health.
- Burrower aliens cannot be targeted by towers until they have travelled several path steps.
- The final wave includes one boss alien.
- Aliens follow the current path and damage the base if they reach the goal.
- Later waves spawn upgraded aliens, capped by the debug config.
- Towers select targets, fire on cooldown, damage aliens and apply a short slow effect.
- Tower shots draw a short line and briefly flash the alien.
- Freeze Pulse can be triggered during a wave to slow all active aliens.
- Orbital Strike can be triggered during a wave to damage all active aliens.
- Status effects expire automatically.
- Killed aliens award scrap.
- Clearing waves awards bonus scrap.
- Clearing a wave without taking base damage awards a perfect-wave scrap bonus.
- Clearing a non-final wave offers a reward choice before the next wave can start.
- Reward choices show as a random 3-option offer from a larger pool.
- Reward choices can currently add scrap, repair the base, discount the next placed tower, boost next-wave tower damage or boost next-wave tower fire rate.
- A debug event feed shows important placement, upgrade, reward, leak and win/loss events.
- Victory and defeat show a run summary with stars, wave, kill, leak, scrap, reward and ability-use counts.
- Completed runs update session progress with runs, victories, best stars, best waves and best perfect waves.
- The slice ends in victory after all waves or defeat when the base reaches zero health.

## How To Run

1. Wait for Unity to finish compiling.
2. Run Edit Mode tests from `Window > General > Test Runner`.
3. Recreate the debug scene from `Project147 > Debug > Create Grid Scene`.
4. Press Play.
5. Place towers by clicking open cells.
6. Read the `Next Wave` panel, then press `Start Wave` in the debug HUD.
7. After a non-final wave, choose one reward from the reward panel.
8. Continue placing or upgrading towers between waves. If you picked the construction credit, the next tower cost is reduced.
9. Toggle `Sell Mode`, click an existing tower, and confirm scrap increases.
10. Click `Speed` to cycle between 1x, 2x and 3x wave speed.
11. If you picked `Overclock`, the next wave gets boosted tower damage.
12. If you picked `Rapid Loader`, the next wave gets boosted tower fire rate.
13. Use `Freeze Pulse` or `Orbital Strike` during active waves when their cooldowns are ready.
14. On victory or defeat, read the run summary panel.
15. Check the `Session Progress` panel, then press `Restart` to confirm session progress remains visible.

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
- Large purple alien with crown: boss alien.
- Red/black tower: railgun.
- Orange/black tower: mortar.
- Teal/yellow tower: energy.
- Green range ring: valid tower placement or affordable tower upgrade preview.
- Red range ring: invalid, unaffordable or max-level tower preview.
- Cyan pulse ring: Freeze Pulse ability.
- Orange strike ring: Orbital Strike ability.

## Deliberate Limits

- Towers can only be placed between waves.
- Towers can only be upgraded between waves.
- There are three debug tower types.
- There are six debug alien types.
- First-slice tuning is grouped in one debug config asset, not final production content data.
- There is no polished UI.
- There is no save system.
- There is no mobile touch polish.
- There is no final art.
- Reward choices are debug-tuned and not final progression design.
- The tower discount reward only affects the next placed tower, not upgrades.
- Selling refunds 75% of tower and upgrade spend in the debug slice.
- Speed controls scale gameplay updates, not Unity's global time scale.
- Overclock affects damage only, not fire rate.
- Rapid Loader affects fire rate only, not damage.
- Orbital Strike is a debug ability, not final balance.
- Run summary is debug UI, not final progression UX.
- Session progress is in-memory only; it is not a save system yet.

These limits are intentional. The slice exists to prove the loop before expanding content.
