# Unity Project Setup

## Project Location

Create the Unity project inside:

`/Users/kim/Repos/project-147/game`

Do not create the Unity project at the repository root.

The repository root is for documentation, shared project files and future non-Unity services. The `game/` folder is for Unity-generated project files such as `Assets`, `Packages` and `ProjectSettings`.

## Unity Version

Use the agreed Unity 6 version installed through Unity Hub.

Current expected version: Unity 6.4, unless we deliberately change it.

## Template

Use the 3D URP mobile-friendly template if available.

If Unity Hub only offers a standard 3D template, use that and we can add URP after project creation.

## Required Initial Settings

After the project opens, configure these before gameplay work begins:

- Version Control Mode: Visible Meta Files.
- Asset Serialisation Mode: Force Text.
- External Script Editor: Visual Studio Code.
- Default platform can remain desktop/editor until device builds matter.

## Initial Unity Folders

Inside `game/Assets`, use this starting structure:

- `Project147/GameCore/`
- `Project147/GameData/`
- `Project147/UnityPresentation/`
- `Project147/PlatformServices/`
- `Project147/Tests/EditMode/`
- `Project147/Tests/PlayMode/`

These names can be refined, but the boundaries matter:

- `GameCore` contains deterministic gameplay rules.
- `GameData` contains definitions and tuning.
- `UnityPresentation` contains scenes, prefabs, visuals, input and UI.
- `PlatformServices` contains saves, ads, purchases, analytics and backend boundaries.
- `Tests` contains automated coverage.

## First Assembly Definitions

Create assembly definitions so dependencies stay controlled:

- `Project147.GameCore`
- `Project147.GameData`
- `Project147.UnityPresentation`
- `Project147.PlatformServices`
- `Project147.Tests.EditMode`
- `Project147.Tests.PlayMode`

Dependency direction should be deliberate:

- presentation can depend on core;
- services can depend on service contracts;
- core should not depend on presentation;
- core should not depend on ad, purchase or analytics SDKs.

## Android Module Failures

Android SDK or NDK download failures do not block early development if the editor opens.

We can start with editor play mode, iOS support and tests. Android build support can be repaired before Android device testing.

