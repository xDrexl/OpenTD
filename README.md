# OpenTD

OpenTD is a lightweight 2D top-down tower-defense game used to evaluate an AI-first game-development workflow.

## Technology

- Godot .NET
- C#
- ECS
- SOLID
- Git
- GitHub
- Codex

## Goals

The project has two goals:

1. Build a small polished tower-defense game.
2. Evaluate whether disciplined architecture reduces AI development complexity and token consumption.

The architecture prioritizes:

- small task context;
- modular gameplay systems;
- automated validation;
- data-oriented simulation;
- separation of simulation from presentation;
- agent-friendly source files.

## Initial Game Scope

The first playable version contains:

- one map;
- one enemy;
- one tower;
- enemy pathing;
- tower placement;
- automatic targeting;
- projectiles/damage;
- enemy death;
- currency;
- base health;
- waves;
- basic UI;
- simple generated sprites.

Complexity is added incrementally only after the previous milestone works.

## Documentation

- `AGENTS.md` — mandatory agent rules.
- `docs/ARCHITECTURE.md` — technical architecture.
- `docs/WORKFLOW.md` — development and Git workflow.
- `docs/ROADMAP.md` — incremental development plan.

## Development

Prerequisites:

- Godot 4.7.1 .NET;
- .NET 8 SDK.

Build and test from the repository root:

```text
dotnet build
dotnet test
```

Launch the project from Godot, or validate it without a display:

```text
godot --headless --path . --quit
```

## Principle

> Build the simplest architecture that allows the next feature to be added locally and safely.
