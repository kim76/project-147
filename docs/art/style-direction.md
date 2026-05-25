# Art Style Direction

## Goal

Create a readable, funny and distinctive mobile tower-defence style without copying another game's theme, characters, silhouettes, UI or map language.

## Recommended Direction

Use stylised low-poly 3D with an orthographic camera.

Reasons:

- readable on phones;
- easier to rotate and upgrade towers;
- efficient on mobile hardware;
- fast to create placeholder and production assets;
- flexible for humour through animation and effects.

## Visual Rules

- Towers need strong silhouettes.
- Aliens need clear role readability at small size.
- Projectile and impact effects must make damage type obvious.
- Upgrade states should be visible without relying only on text.
- The battlefield must stay readable above decoration.
- UI should support repeated play, not behave like a marketing page.

## Early Asset Set

First defence slice:

- one terrain tile set;
- one base objective;
- one spawn portal;
- one build grid overlay;
- one path preview overlay;
- three towers;
- three aliens;
- basic projectile and impact effects;
- simple upgrade panel;
- victory and defeat screens.

## AI Image Use

AI-generated images can be used for concept art, mood boards, icons, UI exploration and store-art exploration.

Do not depend on AI images as final production assets until each asset is checked for consistency, readability, licence suitability and technical fit.

## Placeholder Policy

Placeholders are allowed when they preserve the real production shape.

Good placeholder:

- a simple tower prefab with the same attachment points, range visual and upgrade hooks expected later.

Bad placeholder:

- a scene-only object with hardcoded stats and no connection to data definitions.

## First-Pass In-Scene Profiles

The current debug scene uses code-native low-poly profiles, not final art:

- railgun: red/black tower with a visible barrel and core;
- mortar: orange/black chunky base with angled tube and rim;
- basic alien: purple body with readable eye;
- fast alien: cyan capsule with fin;
- armoured alien: stone cube body with shell and visor.

These profiles are deliberately built from simple primitives so behaviour and silhouette can change quickly without asset pipeline friction.
