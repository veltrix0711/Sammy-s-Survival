# Build Plan

This plan aligns execution with milestones, acceptance criteria, performance budgets, and test gates.

## Milestones Overview

- M0: Project spine, voxel dig/place, interaction verbs, unique items + save/load
- M1: Craft-anywhere + knowledge system + bench modifiers
- M2: Living NPC survivors (GOAP+BT, AI LOD, dynamic trade/encounters)
- M3: Combat, ballistics, wounds
- M4: Building v2, structural integrity, modular vehicles
- M5: Finite economy + deterministic world seeding + events
- M6: Vertical slice → alpha (content quotas, co-op soak)

---

## M0 — Spine, Voxel, Interaction, Persistence
- Goals:
  - Project settings and folders; URP verified
  - Voxel terrain: chunk storage, async meshing, place/dig
  - Interaction verbs: use, cut, attach, disassemble; tool gating
  - Unique item IDs; inventory/containers; save/load deterministic
- Deliverables:
  - Minimal world master scene with streamed chunk placeholders
  - Systems: `VoxelWorld`, `InteractionSystem`, `Inventory`, `Persistence`
  - Tiny loop: cut cloth with knife → bandage; save/reload exact state
- Acceptance Criteria:
  - Save→load→diff = ∅ for the demo loop
  - Async meshing never blocks main thread >4ms
- Perf Budgets:
  - 60 FPS 1080p; chunk mesh gen on background; ≤5ms/frame average scripting
- Test Plan:
  - Playmode test: cloth→bandage loop
  - Automated serialization diff test

## M1 — Craft Anywhere + Knowledge + Benches
- Goals:
  - Data-driven recipes; real-world verbs; benches modify speed/quality only
  - Knowledge items unlock advanced precision (not basics)
- Deliverables:
  - `Recipe` and `Knowledge` ScriptableObjects; crafting executor
  - Editor: Item/Recipe Designer tool
- Acceptance Criteria:
  - Recipes executable anywhere; bench deltas observable; knowledge gates enforced
- Perf Budgets:
  - Craft graph eval ≤1ms/frame average
- Test Plan:
  - Unit tests: recipe validation, knowledge gating

## M2 — Living NPC Survivors
- Goals:
  - GOAP+BT hybrid; AI LOD; encounters; barter with real inventories
- Deliverables:
  - `SurvivorBrain`, `EncounterSpawner`, `TradeSystem`
- Acceptance Criteria:
  - NPCs survive without cheats; can trade/flee/fight contextually
- Perf Budgets:
  - AI LOD: ≤1ms/frame on average per 20 active agents
- Test Plan:
  - Simulation scene soak tests; determinism snapshots

## M3 — Combat, Ballistics, Wounds
- Goals:
  - Ballistics with gravity/drag, penetration, ricochet; melee blocks/stamina
  - Wound system: bleeding, fractures, burns, frostbite, infection
- Deliverables:
  - `BallisticsSolver`, `HitMaterialDB`, `WoundSystem`
- Acceptance Criteria:
  - Ballistic tolerances within spec; wounds apply/resolve appropriately
- Perf Budgets:
  - Projectile step ≤0.1ms average; no GC spikes
- Test Plan:
  - Range harness; penetration tables; wound state machine tests

## M4 — Building v2, Integrity, Vehicles
- Goals:
  - Structural integrity; modular vehicles
- Deliverables:
  - `IntegritySolver`, building pieces; `VehicleAssembly`
- Acceptance Criteria:
  - Integrity monotonicity; vehicles assemble/disassemble safely
- Perf Budgets:
  - Integrity solve ≤2ms/frame average for 5k elements
- Test Plan:
  - Stress scenes for buildings and vehicles

## M5 — Finite Economy + World Seeding + Events
- Goals:
  - Unique IDs for all items; deterministic world resources/events
- Deliverables:
  - Seeded generator; economy ledger; events
- Acceptance Criteria:
  - No respawns; counts conserved barring destruction/crafting
- Perf Budgets:
  - Seeding offline or async; runtime budget ≤2ms/frame
- Test Plan:
  - Conservation tests; seeded determinism diffs

## M6 — Vertical Slice → Alpha
- Goals:
  - Content quotas; co-op soak; telemetry
- Deliverables:
  - 1–2 biomes playable; 1–8 co-op; smoke tests in CI
- Acceptance Criteria:
  - 60 FPS; ≤50 kB/s/client avg; no blocking spikes
- Perf Budgets:
  - As above; soak stable
- Test Plan:
  - Long-run sessions; CI scenes; perf captures

---

## Parallel Tracks
- Tooling: Item/Recipe Designer, Map/Chunk Composer, Integrity Heatmap, Projectile Range Harness, AI Sandbox, Save Validator
- Testing Gates: playmode/unit suites, serialization diffs, perf captures
- Performance Budgets: tracked per milestone
- Content Pipeline: conventioned import and atlasing; biomes blocked out early

---

## Definition of Done (per system)
- Tests written and passing (unit + playmode as relevant)
- Serialization invariants validated (where applicable)
- Profiling checkpoints on target hardware; budgets met
- Docs updated (overview, plan, needs); examples included

---

## Issue Breakdown (initial)
1. Project settings: URP, color space, play mode options, quality defaults
2. Folder skeletons and scenes scaffolding
3. VoxelWorld prototype: chunks, greedy meshing stub, async job skeleton
4. InteractionSystem prototype: verbs and tool gating skeleton
5. Inventory + Unique IDs: model and persistence interface
6. Persistence: save/load repository and deterministic diff test
7. Crafting data model: items/recipes/knowledge ScriptableObjects
8. Editor Tool: Item/Recipe Designer basics
9. Encounter framework + survivor brain skeleton
10. Ballistics solver scaffold + range harness scene
11. Wound system scaffold
12. Structural integrity solver scaffold
13. Vehicle assembly scaffold
14. Economy ledger + seeded generator scaffold
15. CI: headless build and basic tests

---

## First 7 Days Checklist
- [ ] URP verified, color space linear, play mode options set
- [ ] Scenes and folder skeletons committed
- [ ] VoxelWorld stub with chunk data + greedy meshing interface
- [ ] InteractionSystem stub with verb registry
- [ ] Inventory + UniqueId + basic SaveRepository
- [ ] Test scene for env blocking; content import conventions
- [ ] BUILD_PLAN.md and NEEDS.md reviewed and updated

## Weekly Cadence Template
- Plan: review progress vs milestones; update risks/needs
- Build: implement scoped tasks with tests
- Test: playmode/unit, serialization diffs, perf capture
- Measure: profiling against budgets; bandwidth
- Adjust: update plan/docs; open/close issues; triage
