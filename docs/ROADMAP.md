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

Only after the game architecture is proven:

Codex  
→ image-generation tooling / ComfyUI  
→ asset validation  
→ Godot import

Generated content remains replaceable presentation data.

---

# Phase 19 — Optional Godot Agent Integration

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
- Phase 15 TileMap/build zones/obstacles: manual runtime validation pending.
