# OpenTD Development Roadmap

## Strategy

Develop through small independently verifiable vertical increments.

Each phase must work before the next phase begins.

Do not implement speculative future systems.

---

# Phase 0 — Repository Foundation

Goal: reproducible development environment.

Deliver:

- Godot .NET project;
- Git repository;
- GitHub repository;
- `.gitignore`;
- documentation;
- C# build succeeds;
- empty game launches;
- headless validation works.

Architecture:

- initial directory structure only;
- test project configured.

Exit criteria:

> Fresh clone can build and launch.

---

# Phase 1 — Static Map

Goal: display the game world.

Deliver:

- fixed top-down map;
- path visualization;
- spawn location;
- destination/base;
- camera;
- generated SVG terrain sprites.

No enemies yet.

Exit criteria:

> Map displays correctly at runtime.

---

# Phase 2 — ECS Foundation

Goal: create the minimum ECS needed for gameplay.

Deliver:

- Entity ID;
- entity lifecycle;
- component storage;
- basic queries;
- system execution;
- simulation tick;
- simulation tests.

Do not build a generic commercial ECS framework.

Implement only capabilities currently required.

Exit criteria:

> Tests create entities, attach/query components and execute a system.

---

# Phase 3 — Enemy Movement

Goal: first moving gameplay entity.

Components:

- Position;
- Movement;
- PathProgress;
- Enemy.

Systems:

- MovementSystem;
- PathCompletionSystem.

Presentation:

- EnemyView.

Assets:

- one generated enemy sprite.

Exit criteria:

> Enemy travels from spawn to destination.

---

# Phase 4 — Base Health

Goal: enemies create failure pressure.

Add:

- Base;
- Health;
- enemy arrival event;
- base damage;
- enemy removal.

UI:

- base health display.

Exit criteria:

> Reaching the destination damages the base.

---

# Phase 5 — Tower Placement

Goal: player can alter the battlefield.

Add:

- currency;
- build cost;
- tower entity;
- valid/invalid placement;
- player input;
- placement preview.

Asset:

- one generated tower sprite.

Exit criteria:

> Player can purchase and place towers only in valid locations.

---

# Phase 6 — Targeting

Goal: towers detect enemies.

Components:

- AttackRange;
- Target.

System:

- TargetingSystem.

Initial targeting rule:

> nearest valid enemy in range.

Exit criteria:

> Towers acquire and lose targets correctly.

Automated targeting tests required.

---

# Phase 7 — Combat

Goal: towers kill enemies.

Add:

- attack cooldown;
- projectile;
- damage;
- enemy health;
- death event.

Systems:

- AttackSystem;
- ProjectileSystem;
- DamageSystem;
- DeathSystem.

Exit criteria:

> Towers automatically kill enemies.

Combat calculations must be unit tested.

---

# Phase 8 — Economy

Goal: complete gameplay feedback loop.

Add:

- reward on enemy death;
- currency UI;
- tower purchase cost.

Loop:

```text
Kill enemy
    ↓
Earn currency
    ↓
Build tower
    ↓
Kill more enemies
```

Exit criteria:

> Economy supports repeated tower construction.

---

# Phase 9 — Waves

Goal: structured progression.

Add:

- WaveConfiguration;
- WaveSystem;
- spawn timing;
- increasing enemy counts;
- inter-wave delay;
- wave UI.

Wave definitions should be data-driven.

Exit criteria:

> Multiple waves run automatically.

---

# Phase 10 — Game States

Goal: complete playable session.

States:

- Ready;
- Running;
- Victory;
- Defeat.

Add:

- restart;
- victory condition;
- defeat condition.

Exit criteria:

> Player can complete or lose a full game and restart.

This is MVP.

---

# Phase 11 — Architecture Evaluation

Goal: evaluate development methodology before expanding scope.

Status: complete. The architecture is validated with targeted follow-up
constraints. See `docs/ARCHITECTURE_EVALUATION.md`.

Measure:

- Codex token consumption;
- files read per task;
- files modified per feature;
- build/test reliability;
- regression frequency;
- architectural violations;
- human intervention required.

Perform sample extension tasks:

1. second enemy;
2. second tower;
3. armor;
4. splash damage;
5. slow effect.

Evaluate whether each addition remains local.

Exit criteria:

> Decide whether architecture validates the Godot + SOLID + ECS approach.

Do not expand scope until this review occurs.

---

# Phase 12 — Enemy Variety

Status: first variety increment complete with basic and fast enemy archetypes.

Possible additions:

- fast enemy;
- armored enemy;
- high-health enemy;
- swarm enemy.

Prefer component composition over subclasses.

---

# Phase 13 — Tower Variety

Status: first variety increment complete with basic and rapid-fire tower
archetypes.

Possible additions:

- rapid-fire tower;
- artillery tower;
- slowing tower;
- area-damage tower.

Shared mechanics should emerge from reusable components/systems.

---

# Phase 14 — Status Effects

Status: first concrete effect complete with a slow-on-hit tower. Generic status
effect infrastructure remains deferred until a second real effect exists.

Add generic effects only once at least two real use cases exist.

Examples:

- slow;
- burn;
- poison.

Avoid premature generic status-effect frameworks.

---

# Phase 15 — Better Maps

Status: first map upgrade complete with TileMap terrain, explicit build zones,
and obstacle regions.

Add:

- TileMap-based map;
- obstacle regions;
- build zones;
- multiple paths;
- improved terrain visuals.

Only introduce dynamic pathfinding if gameplay requires it.

---

# Phase 16 — Persistence

Status: deferred until the MVP has meaningful settings, unlocks, progression, or
scores to persist. Runtime ECS state will not be saved prematurely.

Add only when meaningful persistent state exists.

Potential:

- settings;
- unlocked maps;
- progression;
- scores.

Do not prematurely save runtime ECS internals.

Persist domain state.

---

# Phase 17 — Presentation Pass

Status: first presentation pass complete and manually validated.

Improve:

- sprites;
- animation;
- particles;
- audio;
- UI;
- feedback;
- transitions.

Gameplay architecture should not change.

---

# Phase 18 — Optional Asset Pipeline

Status: deferred. The current generated SVG workflow produces replaceable assets
without creating an MVP bottleneck, so a dedicated image-generation pipeline
would add infrastructure without a demonstrated need.

Only after the game architecture is proven:

Codex  
→ image-generation tooling / ComfyUI  
→ asset validation  
→ Godot import

Generated content remains replaceable presentation data.

---

# Phase 19 — Optional Godot Agent Integration

Status: deferred. The existing CLI, automated tests, and headless Godot workflow
have not created a measurable editor-automation bottleneck.

Evaluate Godot MCP/editor automation only if CLI/file workflows become a measurable bottleneck.

Potential tools:

- inspect scene tree;
- create nodes;
- modify properties;
- launch scene;
- collect errors;
- capture screenshots.

MCP is optional infrastructure.

The game must not depend on it.

---

# Pre-MVP Feature Cycle

The following features must be completed before producing the MVP candidate.

Run rules agreed for this cycle:

- runs continue through procedurally generated stages until defeat;
- every stage, including stage 1, uses a generated map;
- stage 1 has three waves and each subsequent stage adds exactly one wave;
- enemy health, speed, damage, and rewards do not scale between stages;
- base health, currency, and placed towers do not carry between stages;
- saves are stage-start checkpoints, not mid-wave simulation snapshots;
- defeat ends the run and deletes its checkpoint.

---

# Phase 20 — Main Menu

Goal: provide an explicit entry point for starting and resuming runs.

Status: complete. Manual runtime validation passed on 2026-08-17.

Add:

- a main-menu scene as the application entry point;
- New Game, Continue, and Quit actions;
- a disabled Continue action when no valid checkpoint exists;
- confirmation before New Game replaces an existing run.

Exit criteria:

> The player can start, resume, replace, or exit a run from the main menu.

---

# Phase 21 — Stage Checkpoint Save/Load

Goal: resume an endless run safely without serializing runtime ECS internals.

Status: complete. Manual runtime validation passed on 2026-08-17.

Add:

- a versioned checkpoint containing only the stage number and run seed;
- JSON persistence in Godot's `user://` data directory;
- atomic checkpoint replacement;
- deterministic reconstruction of the saved stage;
- graceful handling of missing, malformed, or incompatible saves.

Behaviour:

- New Game creates a stage-1 checkpoint with a new run seed;
- quitting during a stage leaves its stage-start checkpoint intact;
- completing a stage replaces it with the next-stage checkpoint;
- defeat deletes it;
- no entities, towers, enemies, projectiles, timers, health, or currency are
  serialized.

Automated tests must cover round trips, replacement, missing files, malformed
data, unsupported versions, and deletion.

Exit criteria:

> Continue reconstructs the same stage and map from a small, versioned save.

---

# Phase 22 — Procedural Stage Maps

Goal: generate a different valid battlefield for every stage.

Status: complete. Manual runtime validation passed on 2026-08-17.

Add:

- an engine-independent deterministic map generator using run seed and stage;
- a bounded orthogonal path from the left-side spawn to the right-side base;
- deterministic terrain variation and obstacle placement;
- obstacle clearance from the path, spawn, base, and map boundaries;
- runtime map presentation driven by `MapConfiguration` rather than fixed scene
  coordinates.

Preserve the existing build-zone, path-clearance, obstacle, and tower-placement
rules. Dynamic pathfinding is not required.

Automated tests must verify determinism, cross-stage variation, bounds, obstacle
clearance, and the presence of usable build space.

Exit criteria:

> A saved run always reconstructs the same valid map, while later stages use
> different maps.

---

# Phase 23 — Endless Stage Progression

Goal: connect completed stages into an endless run with increasing wave counts.

Status: complete. Manual runtime validation passed on 2026-08-17.

Add:

- stage-aware wave configuration;
- `stage number + 2` waves per stage;
- cycling of established enemy wave compositions without stat scaling;
- a stage number in the gameplay HUD;
- a stage-complete overlay with a Next Stage action;
- next-stage checkpoint creation before advancement;
- a defeat flow that deletes the checkpoint and returns to the main menu.
- an ESC pause menu with Resume, Main Menu, and Quit actions; returning to the
  main menu preserves the current stage-start checkpoint.

Each new stage creates a fresh simulation with default base health and currency
and no placed towers.

Automated tests must verify wave-count progression, unchanged enemy statistics,
checkpoint advancement, and checkpoint deletion on defeat.

Exit criteria:

> Completing a stage saves and launches a new map with one additional wave;
> losing ends the run.

---

# Pre-MVP Cycle Manual Validation

Before declaring the MVP candidate ready, manually verify:

- New Game, replacement confirmation, Continue, and Quit;
- quit and relaunch during a stage, then confirm that stage restarts;
- complete multiple stages and confirm that maps change deterministically;
- confirm that every later stage adds one wave;
- confirm that health, currency, and towers reset between stages;
- lose a run and confirm that Continue is no longer available.
- pause with ESC, resume, return to the main menu, and confirm the stage can be
  continued from its stage-start checkpoint.

---

# Guiding Rule

At every milestone:

> Add exactly enough architecture for the current requirement plus obvious immediate reuse.

Do not build tomorrow's framework today.

---

# Manual Validation Status

- Phases 7–10: manual runtime validation passed on 2026-08-17.
- Phase 12 fast enemy: manual runtime validation passed on 2026-08-17.
- Phases 13–14 tower variety and slowing effect: manual runtime validation
  passed on 2026-08-17.
- Phase 15 TileMap/build zones/obstacles: manual runtime validation passed on
  2026-08-17.
- Phase 17 presentation feedback: manual runtime validation passed on
  2026-08-17.
