# OpenTD Architecture

## Objective

Keep gameplay highly modular while minimizing the amount of repository context required to make a change.

The architecture is:

> ECS simulation + thin Godot presentation + explicit infrastructure boundaries.

---

## Layers

```text
┌─────────────────────────────┐
│       Godot Presentation    │
│ UI / Input / Audio / Views  │
└──────────────┬──────────────┘
               │ commands/state
               ▼
┌─────────────────────────────┐
│          Simulation         │
│ Entities / Components       │
│ Systems / Queries / Events  │
└──────────────┬──────────────┘
               │
               ▼
┌─────────────────────────────┐
│        Infrastructure       │
│ Save / Load / Assets / IO   │
└─────────────────────────────┘
```

Simulation must not depend on Godot presentation.

---

## Suggested Repository Structure

```text
src/
├── Simulation/
│   ├── Components/
│   ├── Systems/
│   ├── Events/
│   ├── Commands/
│   ├── Queries/
│   └── World/
│
├── Presentation/
│   ├── Views/
│   ├── UI/
│   ├── Input/
│   └── Scenes/
│
└── Infrastructure/
    ├── Persistence/
    ├── Assets/
    └── Configuration/

tests/
└── Simulation/

scenes/

assets/
├── generated/
└── authored/
```

Do not create directories before they are needed.

---

# ECS

## Entity

An entity is an opaque identifier.

Example:

```text
Entity 128
```

Its meaning comes from its components.

---

## Example Enemy

```text
Entity 128

Position
PathProgress
Movement
Health
Enemy
Reward
```

Applicable systems discover the entity from its components.

---

## Example Tower

```text
Entity 450

Position
Tower
AttackRange
AttackCooldown
Targeting
```

---

## Systems

Example:

```text
MovementSystem
    reads: Movement, PathProgress
    writes: Position, PathProgress

TargetingSystem
    reads: Position, AttackRange, Enemy
    writes: Targeting

AttackSystem
    reads: Targeting, AttackCooldown
    emits: AttackCommand

DamageSystem
    consumes: DamageEvent
    writes: Health
```

A system should be understandable without reading unrelated systems.

---

# Communication

Prefer these mechanisms in order:

1. Direct component processing inside one system.
2. Commands for requested actions.
3. Events for completed state changes.
4. Explicit service interfaces at subsystem boundaries.

Avoid global event buses unless justified.

Avoid systems calling arbitrary systems.

---

# Godot Presentation

Presentation Nodes represent currently visible simulation entities.

Example:

```text
EnemyView
├── Sprite2D
├── AnimationPlayer
└── AudioStreamPlayer2D
```

`EnemyView` references an entity ID.

It renders simulation state.

It is not the authoritative enemy.

Destroying a view must not implicitly destroy the simulation entity unless an explicit command requests it.

---

# Interfaces

Use interfaces at architectural seams.

Good:

```text
ISaveStore
IAudioService
IEntityViewFactory
IRandomSource
```

Usually unnecessary:

```text
IHealthComponent
IEnemyComponent
IPositionComponent
```

Components are data, not polymorphic objects.

---

# Dependency Rule

Code closer to core gameplay should know less about external systems.

Simulation must not depend upon:

- concrete UI;
- scenes;
- sprites;
- audio;
- filesystem paths;
- editor state.

This allows simulation testing without launching the full game.

---

# Performance

Correct architecture comes first.

Optimize only after measurement.

ECS should make future batching/data-oriented optimization possible without requiring premature low-level optimization.

---

# Architecture Test

For every new feature ask:

> How many existing subsystems must understand this feature?

The preferred answer is:

> As few as possible.
