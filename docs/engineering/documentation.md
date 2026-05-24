# Documentation Standards

## File Naming

Documentation lives in grouped folders. The folder identifies the area; the filename identifies the document's role or topic.

Use lowercase kebab-case for folder and file names.

Good examples:

- `docs/design/core-game.md`
- `docs/design/alien-mode.md`
- `docs/engineering/testing.md`
- `docs/engineering/standards.md`
- `docs/monetisation/rewarded-ads.md`
- `docs/planning/first-build.md`

Avoid:

- `GameSpec.md`
- `GAME_SPEC.md`
- `docs/monetisation.md`
- `docs/engineering-standards.md`
- `random-notes.md`

## Groups

Use a top-level folder that shows the document's purpose.

- `docs/design/` for game design, mechanics, modes, balance and feel.
- `docs/engineering/` for architecture, standards, testing and tooling.
- `docs/art/` for visual style, UI, VFX and asset direction.
- `docs/monetisation/` for ads, purchases, subscriptions and store policy.
- `docs/planning/` for delivery sequence, tickets, milestones and decisions.
- `docs/research/` for external references and competitive analysis.

## Document Roles

Prefer stable role names where possible.

- `standards.md` means enforceable rules.
- `architecture.md` means structural boundaries and ownership.
- `testing.md` means automated and manual quality approach.
- `naming.md` means vocabulary and naming conventions.
- `tooling.md` means editor, build and environment setup.
- `principles.md` means guiding trade-offs for an area.
- `first-build.md` means the current first playable build plan.

If a folder grows too large, split by topic while keeping names obvious, such as `docs/design/alien-mode.md` or `docs/monetisation/rewarded-ads.md`.

## Writing Standard

Write decisions clearly enough that future tickets can point to them.

Each design document should explain:

- what the rule or feature is;
- why it exists;
- what is deliberately out of scope;
- what tests or checks should protect it.

Do not let documentation become a dumping ground. If a note becomes a decision, move it into the right grouped document.
