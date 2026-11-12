# Project technical knowledge

Development snapshot updated November 11, 2026. Inspect current code and saved Unity scene/prefab wiring before changing behavior. An existing script or an individual play test does not establish complete end-to-end coverage. See GAME_DESIGN.md and TODO.md.

## Scope and environment

- Gameplay feature freeze is planned for Wednesday November 11; unfinished extras should be cut/simplified. November 12 focuses on presentation; essential release flow and fixes still remain.
- Unity 6000.3.9f1; gameplay scripts in `Assets/_Project/Scripts/`, Game and UI scenes in `Assets/_Project/Scenes/`, assets/prefabs under `Assets/_Project/`. Input actions in `Assets/InputSystem_Actions.inputactions`.
- Input System 1.18.0, Cinemachine 3.1.7, AI Navigation, URP 17.3.0 and Unity Test Framework 1.6.0 are project dependencies. Preserve existing packages and team work.
- Build settings currently enable only `Assets/_Project/Scenes/Game.unity`; no verified exported build. UI scene exists but a complete start/restart/ending flow has not been established.

## Systems

- `RunManager` generates three 1–9 Opening Fortune rolls and puzzle/enemy counts and boss-frequency multiplier. `GameManager` tracks Cleaning, Encounter, Boss, Complete and GameOver, selected/completed puzzle instances and resolved selected enemies. Start/restart UI is not implemented; `GameManager.Start()` expects a RunManager.
- `CleaningPuzzle` exposes `CompleteFromLuckyReel()` and reports completion to GameManager. Pressure plates have randomized fixed-on-retry sequences; `TalismanPuzzle` selects placed child colliders, default four; `LanternPuzzle` selects child colliders, default four, with Inspector timer, reset and lit Gizmos. Both interact via E. Confirm GameManager's explicit pool and scene objects, and ensure at least three valid unique references. Talisman takes E priority if both puzzle types are in range.
- `PlayerMovement` owns input, facing and Rigidbody2D movement. PlayerInput persistent `OnStun` callback remains the E spiritual ability: Cleaning attempts talisman then lantern; combat seals first or stuns. Preserve scene/prefab input callbacks. `Health.OnDeath` drives `PlayerDeath` and `EnemyDeath`; enemy AI manages environmental/player stun, sealing and Karma.
- `WaterFall` uses trigger entry, checks EnemyAI and excludes boss, with per-enemy per-waterfall cooldown. It does not retrigger while an enemy stays inside. Boss environmental stun comes from charge hitting a pillar; sealing also checks configured health threshold.
- `DragonAttackCoordinator` starts charge and then randomly chooses charge/breath, maximum two consecutive per type, preserving collision restoration/stun checks. Boss Bad reel effect changes wait frequency 1.5× for 12s and shortens newly started warning durations to 75%; refreshes, does not stack. Opening Fortune multiplier also applies to attack waits.
- `LuckyReel` weights 1–9 numbers by Happiness; average selects Bad/Neutral/Good. Cleaning modifies time or completes a puzzle. Encounter and Boss implement Bad/Neutral and Good outcomes. Karma 0–44 Peaceful, 56–100 Offensive, 45–55 randomly chooses; Boss offensive can kill, peaceful is a regular stun. Encounter peaceful environmentally stuns one; offensive damages all. Triple matches have no special gameplay effect.
- Regular enemy `EnemyMovement` uses NavMeshAgent desired velocity with Rigidbody2D; boss movement differs. Enemy AI attack range controls state transition, Enemy Combat range/radius controls hitbox. Inspect actual configured values; boss melee was tuned in play tests.

## Configuration and risks

- Boss prefab last inspected has max HP 100 and seal threshold 0.1; script default and scene overrides may differ. Manager prefab has Happiness/Karma 50; verify live scene overrides. Do not overwrite tuning or test values without asking.
- GameManager prefab last inspected lists three puzzle references. Verify actual scene pool, valid reference wiring and inactive unlisted puzzles. Too few puzzles can prevent normal early completion; empty encounter selection or missing boss may stall progression.
- Timers and lantern state currently rely on logs/Gizmos; build needs visible countdown, puzzle feedback and seal eligibility. No runtime player-facing action log is confirmed.
- Overlap attacks can hit the same target multiple times if it has multiple colliders. NavMesh rebakes/asset changes need verification. Former development stun in EnemyAI is commented out. Preserve user-created scene and prefab changes.
- Use existing MonoBehaviour conventions, serialized private tuning fields, Unity lifecycle and direct component relationships. Keep changes focused; test one checklist item at a time.

## Validation

Play tests reported working waterfall, talisman, timed/random lanterns, boss Bad/Neutral and high-Karma Good effects, plus previous core combat tests. Do not claim all branches or a build were tested. Verify low/mid boss outcomes, Good encounter random branches, boss kill/seal victory, restart, edge cases and end-to-end export. Provide exact Unity GameObjects, component fields, inputs and expected outcomes. Do not claim Unity or compilation ran unless it did.
