# Low-Poly Survival — Master Overview

This section is the single source of truth for project alignment. All major changes must update this overview.

## Vision & Pillars
- Tactile, low-poly survival (3D) for PC; everything interactable. Voxels deferred to targeted areas later.
- Craft-anywhere with real-world logic (no blueprint unlocks). Knowledge from books/notes/hard drives for advanced recipes; basics are common sense (knife cuts, tape/glue/screws/bolts attach, grinder/hacksaw cut, oil+rag clean).
- Finite economy: every item is unique (1-of-1) and never respawns; totals only change via crafting/disassembly or destruction.
- NPC survivors are unique individuals with goals/lifestyles (base-builder, van nomad, loner, small groups). They trade/run/fight contextually and live by the same rules as the player.
- Modular systems: vehicles, weapons, buildings; deep wounds/health; ballistics with drop/drag/penetration; real collision.
- Single-player and server-authoritative co-op (1–8). PvE first.

## Feature Summary
- Open-world 3D low-poly environment; interactable terrain/props/structures/furniture. (Voxel areas later.)
- Craft-anywhere; benches only speed/improve quality.
- Inventory (weight/volume), durability, wetness/condition; modular weapons/vehicles/building pieces.
- Ballistics (gravity/drag, material penetration, ricochet), melee with stamina and blocks.
- Health/wounds: bleeding, fractures, burns, frostbite, infection; meds/treatments; temp/hunger/thirst/toxicity.
- Animals (deer/wolf/boar etc.), carcass decay; simple regrowth only for flora/fauna (long cycles).
- Knowledge items (books/manuals/HDs) unlock precision for advanced crafting.
- No fixed traders—only traveling survivors with real inventories.
- Everything persists; items have unique IDs; world state saved.

## Tech Stack
- Unity LTS + URP; C#; server-authoritative netcode; SQLite (dev) → Postgres (later).
- Data-driven items/recipes/knowledge (ScriptableObjects + JSON); editor tools for content authoring.
- Headless Linux dedicated server; CI builds.

## Performance Targets & DoD
- 60 FPS @1080p on min spec; ≤50 kB/s/client avg; chunk mesh gen async.
- Serialization invariants (save→load→diff = ∅); non-respawn economy invariants.
- Ballistic tolerances; integrity monotonicity.

## Risks & Mitigations
- Dynamic nav on diggable terrain: use layered nav, runtime baking windows, agent LOD; fallback waypoint graphs for deep digs.
- Net complexity: strict server authority, deterministic sim surfaces, bandwidth budgets, lag compensation tests.
- Unique item scale: ID compaction, pooled components, content budgets, shardable persistence.

## Links
- Build Plan: `Docs/BUILD_PLAN.md`
- Needs (Live): `Docs/NEEDS.md`
- Map/World Pipeline: `Docs/MAP_PIPELINE.md`
- GitHub Repo: https://github.com/veltrix0711/Sammy-s-Survival

---

## Activity Log
(append dated entries for each step: command, output, PASS/FAIL, TODO)


- unity setup start: 2025-08-21T13:59:14
- git: PASS - git version 2.50.1.windows.1
- hub: PASS - C:\Program Files\Unity Hub\Unity Hub.exe
- unity version fallback: 2022.3.48f1
- editor: installing 2022.3.48f1 via hub
- editor path: C:\Program Files\Unity\Hub\Editor\6000.1.14f1\Editor\Unity.exe
- project path: C:\Users\61448\Desktop\Projects\Sammy's Survival\Sammy-s-Survival\LowPolySurvival
- project: creating
- urp package declare: FAIL
- urp: PASS - pipeline asset assigned
- unity verify open: PASS
- git commit: ok
- commit: 7cc4f10
- remote: https://github.com/veltrix0711/Sammy-s-Survival.git
- unity setup end: 2025-08-21T14:00:41
- unity setup start: 2025-08-21T14:03:01
- git: PASS - git version 2.50.1.windows.1
- hub: PASS - C:\Program Files\Unity Hub\Unity Hub.exe
- unity version fallback: 2022.3.48f1
- editor: installing 2022.3.48f1 via hub
- editor path: C:\Program Files\Unity\Hub\Editor\6000.1.14f1\Editor\Unity.exe
- project path: C:\Users\61448\Desktop\Projects\Sammy's Survival\Sammy-s-Survival\LowPolySurvival
- urp: FAIL - pipeline not assigned
- unity verify open: PASS
- git commit: ok
- commit: 12ba706
- remote: https://github.com/veltrix0711/Sammy-s-Survival.git
- unity setup end: 2025-08-21T14:04:14
- unity setup start: 2025-08-21T14:29:58
- git: PASS - git version 2.50.1.windows.1
- hub: PASS - C:\Program Files\Unity Hub\Unity Hub.exe
- unity version fallback: 2022.3.48f1
- editor: installing 2022.3.48f1 via hub
- editor path: C:\Program Files\Unity\Hub\Editor\6000.1.14f1\Editor\Unity.exe
- project path: C:\Users\61448\Desktop\Projects\Sammy's Survival\Sammy-s-Survival\LowPolySurvival
- urp: FAIL - pipeline not assigned
- unity setup start: 2025-08-21T14:31:29
- git: PASS - git version 2.50.1.windows.1
- hub: PASS - C:\Program Files\Unity Hub\Unity Hub.exe
- unity version fallback: 2022.3.48f1
- editor: installing 2022.3.48f1 via hub
- editor path: C:\Program Files\Unity\Hub\Editor\6000.1.14f1\Editor\Unity.exe
- project path: C:\Users\61448\Desktop\Projects\Sammy's Survival\Sammy-s-Survival\LowPolySurvival
- urp: FAIL - pipeline not assigned
- unity verify open: PASS
- git commit: ok
- commit: 9bbffab
- remote: https://github.com/veltrix0711/Sammy-s-Survival.git
- unity setup end: 2025-08-21T14:32:42
- tools: added advanced Item/Recipe Designer (items, addons, recipes, registry sync, prefab helper, backups) @ Tools → Item & Recipe Designer
- persistence: containers added (identified savables), GameState saves scene containers; chunk painter tool for grid chunks
- crafting: benches added (speed/quality multipliers), CraftingSystem accepts bench context
- ui: minimal Container UI to transfer items between player and container
- crafting: wired inventory consumption/production; nearby bench search; added Recipe UI to trigger crafting
- review: designer expanded (Weapons tab with stats/slots, drag-and-drop addon preview); next: attachment simulation preview and animation hooks
- tools: Weapon Viewer added (preview weapon prefab, select mounts, drag-and-drop addon attach/detach preview)
- tools: Weapon Viewer now supports animation clip preview (play/pause/scrub)
- voxel: initial chunk, flat fill, dig/place sphere carve, naive top-face mesh
- voxel tip: ensure VoxelWorld is at (0,0,0); a green chunk should appear. Add VoxelDebugCarver to Player for dig/place.
- voxel tip: during Play, select `Chunk_0_0` and press F in Scene view to frame it; align Game view to see the platform
- voxel tip: add VoxelDebugCarver to Player, set World reference (or it auto-finds), LMB=dig, RMB=place
- voxel contrast: applied checker texture on lit material for depth cues; chunk has MeshCollider for grounding
- player: adjusted CharacterController movement to ensure proper grounding and gravity
 - change: temporarily disable runtime voxel generation; add simple ground tool for testing. Use Tools → Chunk Painter → Create Simple Ground Plane.
 - editor: Weapon Viewer upgraded — per-slot drag/drop attach, attachment transform editor (with scene handles), Animator parameter controls, and saving prefab variants and `WeaponDefinition` variants. Designer enhanced — addon fields (compatibility, prefab, offsets, modifiers) and item tags authoring.
