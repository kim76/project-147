# Engineering Standards

## Principles

Use simple, testable, production-shaped code from the start.

Small early builds are acceptable. Disposable design is not.

## DRY

Do not repeat gameplay rules, formulas, constants or configuration.

Examples that must have one source of truth:

- tower stats;
- alien stats;
- damage formulas;
- upgrade cost curves;
- resistance calculations;
- economy rewards;
- wave definitions;
- level unlock rules.

If two systems need the same value, they should read it from shared data or a shared domain service.

## KISS

Prefer the simplest design that protects the real requirement.

Good simplicity means:

- fewer moving parts;
- clear ownership;
- easy tests;
- data-driven tuning;
- readable code;
- minimal hidden state.

Bad simplicity means:

- hardcoded tuning values;
- behaviour hidden in Unity scenes;
- duplicated formulas;
- untested gameplay rules;
- magic strings shared across files.

## No Shortcuts

Avoid temporary systems that are likely to survive by accident.

If a feature is not ready for its final version, build the narrowest version of the final shape. For example, a local save service can be small, but it should still sit behind a save boundary that can later support cloud save.

## Data-Driven Gameplay

Tower, alien, wave, level and upgrade tuning should be data-driven where practical.

The goal is to rebalance the game by changing definitions, not by editing scattered code.

## Input Validation

Validate domain inputs at system boundaries before using them.

Examples:

- reject unknown enum values before switch or selection logic;
- reject missing ids when creating definitions;
- reject negative stats unless the design explicitly allows them;
- reject out-of-bounds grid coordinates before mutating grid state.

Tests should cover invalid inputs where the failure mode matters to gameplay or debugging.

## Unity Boundaries

Unity components should handle presentation and platform concerns.

Core gameplay rules should live in plain C# where practical, especially:

- grid rules;
- pathfinding;
- placement validation;
- targeting;
- damage resolution;
- status effects;
- upgrades;
- economy;
- wave timing;
- win and loss conditions.
