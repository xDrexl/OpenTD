# Phase 11 Architecture Evaluation

Date: 2026-08-17

## Decision

The Godot + SOLID + ECS architecture is validated for continued development.

No framework rewrite is warranted. The simulation remains engine-independent,
gameplay rules have automated coverage, and the planned extension exercises are
local to a small number of components, systems, configurations, and views.

Proceed to enemy and tower variety with two constraints:

1. Introduce explicit enemy/tower archetype configuration when the second real
   variant is added; do not extend the current single-archetype assumptions with
   conditionals.
2. Keep `Main` as a composition root. Extract view synchronization or session
   setup only when the next concrete feature would otherwise make it materially
   larger.

## Evidence

### Validation

- 46 automated tests pass.
- The solution builds with zero warnings and errors.
- Godot asset import and headless startup pass.
- Accelerated 60-second and 90-second headless sessions pass.
- The complete MVP was manually validated, including combat, economy, waves,
  victory, defeat, and restart.
- No known regression was committed at the end of a phase.

### Dependency boundaries

- Simulation code has no Godot dependency.
- Components contain data only.
- Systems communicate through components, commands, and events rather than
  calling one another.
- Godot nodes create input commands and render simulation state; they do not own
  authoritative gameplay rules.
- All current simulation rules can be tested without starting Godot.

### Change size by phase

Godot `.uid` sidecars are excluded from logical-file counts.

| Phase | Logical files | Simulation | Presentation/assets | Tests | Docs |
|---:|---:|---:|---:|---:|---:|
| 1 | 7 | 0 | 6 | 0 | 0 |
| 2 | 6 | 4 | 0 | 2 | 0 |
| 3 | 11 | 6 | 4 | 1 | 0 |
| 4 | 12 | 7 | 3 | 2 | 0 |
| 5 | 15 | 8 | 6 | 1 | 0 |
| 6 | 9 | 5 | 2 | 2 | 0 |
| 7 | 18 | 12 | 4 | 2 | 0 |
| 8 | 8 | 4 | 1 | 2 | 1 |
| 9 | 10 | 5 | 3 | 1 | 1 |
| 10 | 8 | 3 | 3 | 1 | 1 |

The largest changes were tower placement and combat. Their size came from new
vertical-slice behavior and presentation assets, not cross-cutting rewrites of
existing systems.

## Extension Exercises

These are change-impact exercises against the completed MVP. They intentionally
do not add post-MVP gameplay during the evaluation phase.

### Second enemy

An enemy with different speed, health, damage, and reward is already expressible
through `EnemyConfiguration` and `WaveDefinition`. No gameplay-system change is
required.

A visually distinct enemy requires a domain archetype identifier plus a
presentation mapping to a different scene. This is a local configuration/view
factory extension, not an ECS change.

Expected impact: one configuration edit for stat-only variety; approximately
four to six logical files for a new visual archetype and its tests/assets.

### Second tower

Targeting and combat already operate from components, so a tower with different
cost, range, cooldown, damage, and projectile speed needs no targeting or combat
system change.

Placement currently assumes one tower definition and one tower scene. The first
additional tower should introduce a selected tower archetype in the placement
command, configuration lookup, and presentation mapping.

Expected impact: approximately five to seven logical files. This is the clearest
current single-archetype pressure point.

### Armor

Add an `Armor` component and incorporate it at the damage-resolution boundary.
Targeting, attacks, projectiles, health, death, rewards, and waves remain
unchanged.

Expected impact: one component, one focused `DamageSystem` change, and tests.

### Splash damage

Add area-impact data and resolve nearby targets at projectile impact. The current
`ProjectileSystem` directly emits single-target damage, so it will need a local
extension or a projectile-impact event consumed by a separate area-damage system.

Expected impact: approximately three to five logical files. Do not generalize
projectile effects until both direct and splash impacts exist.

### Slow effect

Add slow data and a focused system or movement calculation that derives effective
speed. An attack-delivery component/event is also needed to apply the effect.
Targeting, health, death, economy, and waves remain unchanged.

Expected impact: approximately four to six logical files. A generic status-effect
framework is still premature until a second real status effect exists.

## Pressure Points

### `Main` composition root

`Main` now creates session entities, orders systems, translates input, and
synchronizes three view types. It is understandable but is the primary
presentation hot spot. Future work should keep game rules out of it and extract
only cohesive presentation/session responsibilities when concrete reuse appears.

### Single-archetype presentation

Enemy and tower views currently load one scene each. This is sufficient for the
MVP but must become an explicit archetype-to-scene mapping when variety begins.

### Minimal ECS query surface

The ECS intentionally supports one- and two-component queries. Some systems fetch
additional required components after querying. This remains acceptable at the
current scale; expand query APIs only when repeated measured friction justifies it.

### Pull-request integration

The MVP is published as a stack of draft pull requests. Before broad post-MVP
development, merge or otherwise consolidate the stack in order so `main` becomes
the clear source-of-truth baseline.

## Methodology Metrics

- Token consumption per phase: unavailable because per-task token telemetry was
  not captured.
- Files read per task: unavailable because file-read telemetry was not captured.
- Files modified per feature: recorded above from Git history.
- Build/test reliability: every completed phase currently passes build and tests.
- Regression frequency: no known regression remains in committed phase heads.
- Architectural violations: none identified in the final MVP; the pressure points
  above are maintainability risks, not boundary violations.
- Human intervention: required for GitHub authentication, locating the local Godot
  executable, and final manual runtime/visual validation.

Future methodology comparisons should capture token and file-read telemetry at
task start rather than attempting to reconstruct it afterward.

## Exit Assessment

Phase 11 passes.

The architecture supports local extension and preserves the intended dependency
direction. Continue to Phase 12 only after the draft PR stack is integrated and
the current MVP baseline is retained as a known-good build.
