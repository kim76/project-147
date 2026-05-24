# Tooling

## Editor Setup

Use Visual Studio Code for browsing and editing the repository.

Use Unity for scenes, prefabs, play mode, mobile layout, materials, animation, VFX and visual tuning.

Recommended VS Code extensions:

- C# Dev Kit, if it behaves well with Unity on the machine;
- Unity extension for Visual Studio Code;
- GitLens, optional;
- markdownlint, optional.

Do not add editor-specific settings unless they are useful to the whole project.

## Unity Setup

Use Unity 6 LTS or the agreed Unity 6 version for the project.

Create the Unity project in `game/`, not at the repository root. See [Unity project setup](unity-project-setup.md).

Install these Unity modules where practical:

- iOS Build Support;
- Android Build Support;
- Android SDK and NDK tools;
- OpenJDK.

Android module downloads can fail part-way through. If that happens, retry from Unity Hub first. The editor itself is enough to start project setup, documentation, core C# structure and early editor tests.

## Docker Policy

Unity is allowed as a local game-development tool.

Outside Unity, use Docker for supporting tooling where practical. Do not introduce local Node, npm, dotnet, EF or similar project tooling without explicit agreement.

## Local Development Loop

The intended loop is:

1. update a small design or engineering decision;
2. write or update tests for the rule;
3. implement the smallest production-shaped change;
4. run the relevant tests;
5. inspect behaviour in Unity when the change affects gameplay or presentation.

## Build Automation

Mobile builds will eventually need platform-specific signing and store credentials.

Prefer Unity Build Automation or a dedicated mobile CI service when the project reaches device-testing stage.
