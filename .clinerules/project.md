# Project technical knowledge

This is a development snapshot. Verify current code and Unity scene/prefab wiring before changes; an existing script does not establish a working feature. Game design is documented in `GAME_DESIGN.md`; status and next steps are in `TODO.md`.

## Environment and layout

- Unity 6000.3.9f1; confirm in `ProjectSettings/ProjectVersion.txt`.
- `Assets/_Project/Scripts/` contains gameplay C#; `Assets/_Project/Scenes/` contains `Game.unity` and `UI.unity`; `Assets/_Project/Prefabs/` includes Player, Melee, Boss, PressurePlate, PressurePlatePuzzle and Wall; artwork is under `Assets/_Project/Art/`.
- `Assets/InputSystem_Actions.inputactions` defines input actions. The Player prefab uses `PlayerInput` persistent callbacks for movement, attack and stun; the Game scene's Player instance has a spin callback to `LuckyReel`.
- Relevant packages in `Packages/manifest.json`: Input System 1.18.0, Cinemachine 3.1.7, URP 17.3.0, and Unity Test Framework 1.6.0. Check scene and asset use before removing packages.
- Project scripts reviewed contain no custom gameplay `ScriptableObject` classes; no gameplay test scripts were found in `Assets`.

## Systems and communication

- `PlayerMovement` reads input and sets `Rigidbody2D` velocity and facing; `PlayerCombat` uses facing and 2D overlap queries for attacks, stun and environmental-stun-based sealing.
- `EnemyAI` owns chase, attack and stun states and sealing; `EnemyMovement` and `EnemyCombat` use its state. Enemy damage scales with `Karma`. `EnemyDeath` changes karma and reports enemy or boss resolution.
- `Health` supports damage, healing, hit feedback and an `OnDeath` event, then deactivates the object. `PlayerDeath` and `EnemyDeath` subscribe to death events.
- `CleaningPuzzle` is the shared base for selected Cleaning instances and exposes `CompleteFromLuckyReel()`. `PressurePlatePuzzle` generates a shuffled sequence from unique assigned plate IDs, changes `CatHappiness`, and reports completion. The manager tracks selected and completed instances separately. The sequence log is currently commented out.
- `RunManager` generates three fortune rolls and calculates puzzle count, enemy count and a boss-speed multiplier. `GameManager` consumes the puzzle count and coordinates Cleaning, Encounter, Boss, Complete and GameOver states.
- `LuckyReel` weights each 1–9 number using Happiness; every number remains possible. It classifies the average as Bad/Neutral/Good. Cleaning effects adjust time or complete an unfinished puzzle; Good Encounter/Boss results only log the Karma-dependent Offensive/Peaceful selection. Combat effects are not yet applied. Saved Game scene automatic interval is 20 seconds.

## Conventions and configuration

- Gameplay uses `MonoBehaviour` components, PascalCase types and methods, camelCase fields, `[SerializeField] private` tuning fields, Inspector headers and Unity lifecycle methods.
- Relationships use serialized references, direct calls, tags, `FindFirstObjectByType`, and `Health.OnDeath`. Preserve component composition, input events, tags, layers and serialized references when changing behavior.
- Build settings list `Assets/_Project/Scenes/Game.unity`; an exported build has not been verified.
- The Game scene's active GameManager object has `RunManager` and `LuckyReel`; the Player prefab has `PlayerDeath`. `LuckyReel` expects `GameManager` on the same GameObject. `GameManager.Start()` dereferences its `RunManager` lookup without a null check.
- The former development-only environmental stun in `EnemyAI.OnEnable()` is commented out. An empty encounter list or null entries can prevent progression; overlap attacks may hit a target more than once if it has multiple matching colliders.
- The saved Game scene currently has starting Happiness 100 (intended design default 50), starting Karma 50, and four configured Cleaning puzzle references. Verify Inspector settings and ask before changing possible test values. Selected-puzzle and generated-sequence debug logs are commented out.
- Owner prefers useful debug logs/Gizmos for testing and explanations including changed code snippets in final responses. Pressure plates use yellow accepted-press and green completed-puzzle Gizmos; groups deactivate at the end of Cleaning.

## Validation

Check compilation and relevant Unity play-mode behavior when tooling permits. Verify actual GameObjects, components, Inspector values, input bindings and physics settings. The owner reported successful player-death, reel and separate Cleaning puzzle tests; specific Cleaning reel effects, their transitions, combat effects and an exported build remain unconfirmed.