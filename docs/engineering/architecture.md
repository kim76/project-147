# Architecture Principles

## Shape

The project should separate simulation from presentation.

Recommended high-level areas:

- `GameCore` for rules and deterministic simulation;
- `GameData` for definitions and tuning;
- `UnityPresentation` for scenes, components, animation, VFX and UI;
- `PlatformServices` for saves, ads, purchases, analytics and backend access;
- `Tests` for Edit Mode and Play Mode coverage.

Names may change to fit Unity conventions, but the boundaries should remain.

## GameCore

`GameCore` should be plain C# where practical.

It should not know about Unity scenes, prefabs, animation controllers or UI panels.

It can know about domain concepts such as towers, aliens, grid cells, effects, waves, damage types and rewards.

## Data Ownership

Definitions should own tuning values.

Examples:

- tower definition owns base range, fire rate, damage and cost;
- alien definition owns health, speed, resistance and reward;
- upgrade definition owns cost and stat changes;
- wave definition owns spawn timing and composition.

Code should consume definitions, not hide tuning values internally.

## Services

Platform services should sit behind interfaces so they can be mocked in tests.

Examples:

- `ISaveService`;
- `IAdsService`;
- `IPurchaseService`;
- `IAnalyticsService`;
- `IRemoteConfigService`.

The first implementation can be local or fake, but the boundary should exist.

## Determinism

Where random behaviour exists, inject randomness through a controlled abstraction.

This keeps tests stable and allows future replay, debugging and balance analysis.

