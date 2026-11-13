# Project technical knowledge

Development snapshot updated November 13, 2026. Inspect current code and saved Unity scene/prefab wiring before changing behavior. The complete release loop is reported tested; distinguish that report from build validation. See GAME_DESIGN.md and TODO.md.

## Scope and environment

- Gameplay feature freeze was planned for November 11; prioritize presentation and essential fixes, cutting or simplifying optional extras. Lucky Cat is a separate spirit in the Happiness UI; a hovering/shoulder companion is optional low priority, not the player sprite.
- Unity 6000.3.9f1; gameplay scripts in `Assets/_Project/Scripts/`, Game and UI scenes in `Assets/_Project/Scenes/`, assets/prefabs under `Assets/_Project/`. Input actions in `Assets/InputSystem_Actions.inputactions`.
- Input System 1.18.0, Cinemachine 3.1.7, AI Navigation, URP 17.3.0 and Unity Test Framework 1.6.0 are project dependencies. Preserve existing packages and team work.
- Build settings enable MainMenu, Settings, OutsideTemple, InsideTemple and GameOver. Scene flow, pause and audio settings exist; the complete release loop is reported tested; exported-build validation remains.

## Systems

- `RunManager` generates three 1–9 Opening Fortune rolls and puzzle/enemy counts and boss-frequency multiplier. `GameManager` tracks Cleaning, Encounter, Boss, Complete and GameOver, selected/completed puzzle instances and resolved selected enemies. GameManager loads GameOver after victory/defeat; `GameManager.Start()` expects a RunManager.
- `CleaningPuzzle` exposes `CompleteFromLuckyReel()` and reports completion to GameManager. Pressure plates have randomized fixed-on-retry sequences and assigned pressed/unpressed sprites; each listed plate must have a unique ID. Duplicate ID 1 was reported in the Editor log; verify the scene fix. Intended unpressed sprite is `floor-wall-candle-plate_6`, pressed is `_31`. `TalismanPuzzle` selects placed child colliders, default four; `LanternPuzzle` selects child colliders, default four, with Inspector timer and shared lit/unlit sprite references; sprites update on lighting, reset and reel completion. Lantern visuals were reported working; verify individual edge cases. Both interact via E. Assume the configured puzzle pool is sufficient unless the scene configuration changes. Talisman takes E priority if both puzzle types are in range.
- `PlayerMovement` owns input, facing and Rigidbody2D movement. PlayerInput persistent `OnStun` callback remains the E spiritual ability: Cleaning attempts talisman then lantern; combat seals first or stuns, except player stun is blocked throughout Boss phase without starting cooldown. Preserve scene/prefab input callbacks. `AbilityBar` updates attack/stun/reel fills and Health/Karma from gameplay values; it was reported working. Reel progress includes shared spin gap. `Health.OnDeath` drives `PlayerDeath` and `EnemyDeath`; enemy AI manages environmental/player stun, sealing and Karma.
- `WaterFall` uses trigger entry, checks EnemyAI and excludes boss, with per-enemy per-waterfall cooldown. It does not retrigger while an enemy stays inside. Boss environmental stun comes from charge hitting a pillar; sealing also checks configured health threshold.
- `DragonAttackCoordinator` starts charge and then randomly chooses charge/breath, maximum two consecutive per type, preserving collision restoration/stun checks. Boss Bad reel effect changes wait frequency 1.5× for 12s and shortens newly started warning durations to 75%; refreshes, does not stack. Opening Fortune multiplier also applies to attack waits.
- `LuckyReel` weights 1–9 numbers by Happiness; average selects Bad/Neutral/Good. Cleaning modifies time or completes a puzzle. Encounter and Boss implement Bad/Neutral and Good outcomes. Karma 0–44 Peaceful, 56–100 Offensive, 45–55 randomly chooses; Boss offensive can kill, peaceful is a regular stun. Encounter peaceful environmentally stuns one; offensive damages all. Triple matches have no special gameplay effect.
- Regular enemy `EnemyMovement` uses NavMeshAgent desired velocity with Rigidbody2D; boss movement differs. Enemy AI attack range controls state transition, Enemy Combat range/radius controls hitbox. Inspect actual configured values; boss melee was tuned in play tests.

## Configuration and risks

- Boss prefab last inspected has max HP 100 and seal threshold 0.1; Player prefab attackDamage is 1000 (verify intended test tuning); script default and scene overrides may differ. Manager prefab has Happiness/Karma 50; verify live scene overrides. Do not overwrite tuning or test values without asking.
- Assume the configured puzzle and enemy pools are sufficient unless configuration changes.
- Lantern state has assigned lit/unlit sprites; timers, pressure-plate clues and seal eligibility still need readable player-facing feedback. No runtime player-facing action log is confirmed.
- Overlap attacks can hit the same target multiple times if it has multiple colliders. NavMesh rebakes/asset changes need verification. Former development stun in EnemyAI is commented out. Preserve user-created scene and prefab changes.
- Use existing MonoBehaviour conventions, serialized private tuning fields, Unity lifecycle and direct component relationships. Keep changes focused; test one checklist item at a time.

## Validation

Play tests reported working waterfall, talisman, timed/random lanterns, lantern sprite visuals, ability bar, boss Bad/Neutral and high-Karma Good effects, plus previous core combat tests. Do not claim all branches or a build were tested. Pressure-plate duplicate-ID correction and reset, boss-phase stun behavior, lantern timeout/reel visuals and exported build need targeted verification. The complete release loop and both boss victory routes are reported tested; verify remaining edge cases and the exported build as needed. Provide exact Unity GameObjects, component fields, inputs and expected outcomes. Do not claim Unity or compilation ran unless it did.

- OutsideTemple prepares one-use Fortune data; InsideTemple RunManager consumes it, or generates a fresh roll for direct testing. GameOverFlow consumes victory/defeat; Play Again returns to OutsideTemple. Pause uses PlayerInput callback and timeScale. AudioSettings saves mixer values; the Settings scene is wired, but saved MainMenu startup audio initialization needs verification.
