# Testing Approach

## Test First Where It Matters

Core gameplay rules should be developed test-first or test-alongside.

The goal is not bureaucracy. The goal is fast, confident changes.

## Edit Mode Tests

Use Unity Edit Mode tests for deterministic plain C# systems:

- grid construction;
- pathfinding;
- placement validation;
- no-complete-block rule;
- tower targeting;
- damage calculations;
- crit and dodge calculations;
- resistance calculations;
- upgrade cost curves;
- wave schedules;
- economy rewards;
- save data serialisation.

## Play Mode Tests

Use Unity Play Mode tests for scene and integration behaviour:

- touch placement flow;
- enemy movement presentation;
- projectile visuals meeting hit events;
- pause and resume;
- win and loss screens;
- tutorial triggers;
- ad callback handling with mocks;
- purchase restore handling with mocks.

## Manual Checks

Manual playtesting still matters for fun, timing and feel.

Every playable build should be checked for:

- readable tower ranges;
- clear alien paths;
- obvious feedback when attacks land;
- understandable upgrade choices;
- no tiny mobile text;
- stable performance on target devices.

## Definition Of Done

A gameplay change is not done until:

- the rule is tested where practical;
- relevant data definitions are updated;
- duplicated constants are avoided;
- edge cases are considered;
- docs are updated when the design changes.

