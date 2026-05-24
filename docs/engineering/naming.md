# Naming Conventions

## General

Names should describe intent, not implementation trivia.

Use consistent vocabulary across docs, code, data and UI. If the design calls something an `alien`, do not also call it a `mob`, `creep`, `enemy` and `unit` in different systems without a clear reason.

## Domain Terms

Preferred terms for now:

- `tower` for defensive structures;
- `alien` for attacking units;
- `weapon` for the damaging part of a tower;
- `ability` for activated or passive special behaviour;
- `effect` for temporary modifiers such as burn, slow, shield or stun;
- `resistance` for reduced incoming damage from a damage type;
- `dodge` for chance to avoid a hit;
- `crit` for chance to deal bonus damage;
- `scrap` for soft currency placeholder;
- `stars` for level performance rating placeholder.

These can change, but they should change deliberately and everywhere.

## C# Naming

Follow standard C# conventions:

- `PascalCase` for types, methods, properties and public members;
- `camelCase` for local variables and parameters;
- `_camelCase` for private fields if the project standard settles there;
- `IInterfaceName` for interfaces;
- async methods end with `Async` where relevant.

Avoid abbreviations unless they are common in the domain, such as `ui` in Unity-facing code.

## Unity Naming

Scene and prefab names should be readable in the Unity Editor.

Use names such as:

- `Tower_Railgun_Basic`
- `Alien_Runner_Basic`
- `Level_TestGrid_01`
- `VFX_Impact_Electric_01`
- `UI_TowerUpgradePanel`

Do not encode balance values in asset names. A prefab name should not include damage, fire rate or cost.

## Test Naming

Test names should describe behaviour.

Example:

`PlaceTower_WhenPlacementBlocksAllPaths_ReturnsInvalid`

Use behaviour-focused names, not implementation-focused names.

