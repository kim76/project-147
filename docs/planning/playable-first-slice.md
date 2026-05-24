# Playable First Slice

## Purpose

The playable first slice proves the core tower-defence loop using debug visuals.

It is not the MVP and it is not final art. It is a playable test bed for the rules built so far.

## Current Loop

- Player places towers between waves.
- Towers cost scrap.
- Placement is rejected if it blocks all paths.
- Player starts waves manually.
- Aliens spawn, follow the current path and damage the base if they reach the goal.
- Towers select targets, fire on cooldown and damage aliens.
- Killed aliens award scrap.
- Clearing waves awards bonus scrap.
- The slice ends in victory after all waves or defeat when the base reaches zero health.

## How To Run

1. Wait for Unity to finish compiling.
2. Run Edit Mode tests from `Window > General > Test Runner`.
3. Recreate the debug scene from `Project147 > Debug > Create Grid Scene`.
4. Press Play.
5. Place towers by clicking open cells.
6. Press `Start Wave` in the debug HUD.
7. Continue placing towers between waves.

## Debug Colours

- Green: spawn.
- Yellow: goal/base.
- Dark: fixed blocked cells.
- Blue: calculated path.
- Red: placed tower.
- Purple: alien.

## Deliberate Limits

- Towers can only be placed between waves.
- There is one tower type.
- There is one alien type.
- There are no upgrades yet.
- There is no polished UI.
- There is no save system.
- There is no mobile touch polish.
- There is no final art.

These limits are intentional. The slice exists to prove the loop before expanding content.

