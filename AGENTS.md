# AGENTS.md

## Purpose

**OpenTD** is a lightweight 2D top-down tower-defense game built with:

- Godot .NET
- C#
- ECS
- SOLID principles
- GitHub source control
- Codex-first development

Primary engineering goal:

> Maximize maintainability, correctness, agent autonomy, and token efficiency.

Prefer simple, explicit code over clever abstractions.

---

## Agent Priorities

In order:

1. Correctness.
2. Preserve architecture.
3. Minimize task scope.
4. Minimize dependencies.
5. Minimize files read or modified.
6. Keep code simple.
7. Verify changes automatically.
8. Keep documentation synchronized.

Do not refactor unrelated code.

Do not inspect the entire repository unless necessary.

---

## Required Reading

Always read:

- `AGENTS.md`

Read only when relevant:

- `docs/ARCHITECTURE.md` — architecture or new systems/components.
- `docs/WORKFLOW.md` — Git, testing, validation, task procedure.
- `docs/ROADMAP.md` — roadmap/progression work.
- `README.md` — product-level overview.

Do not repeatedly read documentation already understood during the current task.

---

## Architecture

The game uses a hybrid architecture:

- ECS owns authoritative gameplay/simulation state.
- Godot Nodes own presentation and engine integration.
- C# services own infrastructure concerns.
- Presentation must not contain authoritative game rules.

Dependency direction:

Presentation → Simulation API  
Infrastructure → Simulation API  
Simulation → nothing engine-specific

Simulation code must remain independently testable where practical.

See `docs/ARCHITECTURE.md`.

---

## ECS Rules

### Entity

An entity is an ID.

Do not create entity class hierarchies.

### Components

Components contain state/data.

Components must not:

- perform gameplay logic;
- access Godot Nodes;
- access global services;
- directly modify other components.

Prefer small cohesive components.

Do not split data into microscopic components without a clear system/query benefit.

### Systems

Systems contain gameplay behaviour.

Each system must have one clearly defined responsibility.

Systems operate only on the data they require.

Avoid direct system-to-system dependencies.

Prefer commands/events or explicit orchestration where communication is required.

### Presentation

Godot Nodes are views/controllers for simulation state.

They may:

- display simulation state;
- receive input;
- send commands;
- play animation/audio/VFX;
- manage scene lifecycle.

They must not become authoritative gameplay state containers.

---

## SOLID Rules

Use SOLID pragmatically.

Prefer:

- single responsibility;
- composition over inheritance;
- explicit dependencies;
- narrow public APIs;
- dependency inversion at meaningful boundaries.

Avoid speculative abstraction.

Do not create an interface merely because a class exists.

Create an interface when:

- multiple implementations are required;
- testing requires substitution;
- an architectural boundary benefits from abstraction.

Avoid:

- manager god-classes;
- service locator abuse;
- deep inheritance;
- unnecessary factories;
- wrapper-on-wrapper abstractions.

---

## Code Rules

Language: C#.

Use nullable reference types.

Prefer strong typing.

Prefer immutable data when practical.

Keep functions small and focused.

Keep public APIs minimal.

Use descriptive names.

Avoid hidden side effects.

Gameplay constants should be centralized or data-driven where appropriate.

Do not introduce a dependency without justification.

Avoid premature optimization.

Do not duplicate logic.

---

## Godot Rules

Godot version: repository-defined stable .NET version.

Use text-based Godot resources wherever practical.

Scenes should remain small and compositional.

Do not place substantial gameplay logic inside scenes.

Do not manually edit imported/generated Godot cache files.

Never commit:

- `.godot/`;
- build output;
- temporary files;
- IDE caches.

Prefer CLI/headless validation when possible.

---

## Generated Assets

Prototype art should be inexpensive and replaceable.

Codex may generate simple SVG assets directly.

Generated assets belong under:

`assets/generated/`

Do not embed gameplay data inside artwork.

Presentation assets must be replaceable without modifying simulation logic.

---

## Testing

Every gameplay rule that can reasonably be tested without Godot presentation should have automated coverage.

Prioritize tests for:

- combat calculations;
- targeting;
- movement/path progression;
- economy;
- waves;
- placement rules;
- ECS state transitions.

Do not test trivial getters/setters.

---

## Validation

Before completing a code task:

1. Build.
2. Run relevant tests.
3. Run broader tests if shared behaviour changed.
4. Perform Godot headless validation when applicable.
5. Review the diff.
6. Confirm no unrelated files changed.

Never claim success when validation failed.

If validation cannot be performed, state why.

---

## Token-Efficient Workflow

Before coding:

1. Identify the smallest affected subsystem.
2. Read only its relevant files.
3. Search symbols before opening large files.
4. Avoid broad repository scans.
5. Reuse existing abstractions.
6. Make the smallest coherent change.

During coding:

- modify the minimum number of files;
- avoid unrelated cleanup;
- avoid large rewrites;
- prefer local reasoning;
- run targeted tests first.

After coding:

- summarize changes concisely;
- report validation;
- mention unresolved risks only.

Do not produce lengthy explanations unless requested.

---

## Documentation Policy

Documentation is authoritative, not exhaustive.

Update documentation only when:

- architecture changes;
- workflow changes;
- repository structure changes materially;
- roadmap status changes.

Do not document obvious implementation details.

Code should explain implementation.  
Documentation should explain constraints and decisions.

---

## Git Rules

GitHub is the source of truth.

Work on a branch.

Prefer small cohesive commits.

Each commit should represent one logical change.

Do not combine unrelated refactoring with feature work.

Do not commit broken builds intentionally.

Never rewrite shared Git history unless explicitly instructed.

Never commit secrets.

See `docs/WORKFLOW.md`.

---

## Scope Rule

When a request would violate architecture or substantially expand scope:

1. stop implementation;
2. identify the conflict;
3. propose the smallest architecture-compatible solution.

Do not silently bypass repository rules.

---

## Completion Format

At task completion report only:

- what changed;
- tests/validation performed;
- remaining issue, if any.

Keep this concise.
