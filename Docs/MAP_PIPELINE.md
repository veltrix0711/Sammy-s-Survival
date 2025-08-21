# Map & Worldbuilding Pipeline

This pipeline enables environment and character work in parallel to systems programming.

## Scene Strategy
- Additive scenes / subscenes per chunk or POI
- Master world scene streams chunk scenes by address
- Per-biome scenes for lighting/tuning profiles

## Authoring Rules
- Units: 1u = 1m; grid size 1m
- Pivot/origin: base-centered for props; terrain-aligned for large pieces
- Collision/LOD: LOD0/1/2 with colliders on appropriate LOD
- Lighting/Volumes: per-biome profiles; URP material presets

## Addressing & Naming
- `World/Chunks/<Biome>/<Chunk_X_Y>`
- `POI/<Name>`
- `Props/<Category>/<AssetName>`

## Import Settings
- Textures: default 2048 max, compression ASTC/BC as platform
- Normals: explicit; generate if missing
- Atlasing: prefer shared atlases per biome; trim + tight packing

## Validation Checklist
- [ ] Correct unit scale and pivots
- [ ] Colliders present and accurate
- [ ] LODs configured; culling tested
- [ ] URP materials assigned; no pink shaders
- [ ] Performance within budget in preview scene

## Character/Creature Pipeline
- Rig scale: 1u = 1m; T-pose convention
- Retargeting via Humanoid where possible
- Placeholder anims for blockout stages

## References
- Example master scene: `LowPolySurvival/Assets/Scenes/World/Master.unity`
- Example biome scene: `LowPolySurvival/Assets/Scenes/World/Biomes/Temperate/Temperate_Biome.unity`
- Example chunk scene: `LowPolySurvival/Assets/Scenes/World/Biomes/Temperate/Chunks/Chunk_0_0.unity`
- Example POI scene: `LowPolySurvival/Assets/Scenes/POI/Safehouse.unity`
