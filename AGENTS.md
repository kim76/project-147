# Project Instructions

Always use Australian English.

This is a Unity game project. Unity Editor, Unity Hub and Unity-managed mobile build modules are allowed for game development.

Outside Unity, prefer Docker for supporting tooling. Do not introduce local Node, npm, dotnet, EF or similar project tooling unless explicitly agreed first.

Code must follow DRY and KISS principles. Do not duplicate rules, formulas, configuration, tuning values or gameplay constants across systems.

No shortcuts or throwaway architecture. Early builds can be small, but they must still use production-shaped boundaries so successful ideas do not require a rewrite.

Use test-driven development for core gameplay rules. Pathfinding, placement validation, targeting, damage, upgrades, economy, waves and save data should be covered by automated tests before or alongside implementation.

Keep Unity scene objects out of core game rules where practical. Game simulation should live in plain C# services and models, with Unity components responsible for presentation, input, audio, animation and effects.

Documentation must live in grouped lowercase folders with lowercase kebab-case filenames, such as `docs/design/core-loop.md`, `docs/engineering/standards.md` and `docs/monetisation/rewarded-ads.md`.
