# Project technical knowledge

This is a static-review snapshot from November 9, 2026. Verify current code and Unity scene/prefab wiring before changes; an existing script does not establish a working feature. Game design is documented in `GAME_DESIGN.md`; status and next steps are in `TODO.md`.

## Environment and layout

- Unity 6000.3.9f1; confirm in `ProjectSettings/ProjectVersion.txt`.
- `Assets/_Project/Scripts/` contains gameplay C#; `Assets/_Project/Scenes/` contains `Game.unity` and `UI.unity`; `Assets/_Project/Prefabs/` contains Player, Melee, Boss, PressurePlate and Wall; artwork is under `Assets/_Project/Art/`.
- `Assets/InputSystem_Actions.inputactions` defines input actions. The Player prefab uses `PlayerInput` persistent callbacks for movement, attack and stun.
- Relevant packages in `Packages/manifest.json`: Input System 1.18.0, Cinemachine 3.1.7, URP 17.3.0, and Unity Test Framework 1.6.0. Check scene and asset use before removing packages.
- Project scripts reviewed contain no custom gameplay `ScriptableObject` classes; no gameplay test scripts were found in `Assets`.

## Systems and communication

- `PlayerMovement` reads input and sets `Rigidbody2D` velocity and facing; `PlayerCombat` uses facing and 2D overlap queries for attacks, stun and environmental-stun-based sealing.
- `EnemyAI` owns chase, attack and stun states and sealing; `EnemyMovement` and `EnemyCombat` use its state. Enemy damage scales with `Karma`. `EnemyDeath` changes karma and reports enemy or boss resolution.
- `Health` supports damage, healing, hit feedback and an `OnDeath` event, then deactivates the object. `PlayerDeath` and `EnemyDeath` subscribe to death events.
- `PressurePlate` forwards triggers to `PressurePlatePuzzle`; the puzzle checks its sequence, changes `CatHappiness`, and reports success to `GameManager`.
- `RunManager` generates three fortune rolls and calculates puzzle count, enemy count and a boss-speed multiplier. `GameManager` consumes the puzzle count and coordinates Cleaning, Encounter, Boss, Complete and GameOver states.
- `LuckyReel` generates three separate 1–9 numbers and implements manual/automatic timers and a minimum gap. Its current results are logged; gameplay effects are not wired in the reviewed scripts.

## Conventions and configuration

- Gameplay uses `MonoBehaviour` components, PascalCase types and methods, camelCase fields, `[SerializeField] private` tuning fields, Inspector headers and Unity lifecycle methods.
- Relationships use serialized references, direct calls, tags, `FindFirstObjectByType`, and `Health.OnDeath`. Preserve component composition, input events, tags, layers and serialized references when changing behavior.
- The current build settings reference missing `Assets/Scenes/SampleScene.unity`; project scenes were not listed in the inspected settings.
- In the inspected project scenes and prefabs, no component reference was found for `RunManager`, `PlayerDeath` or `LuckyReel`. `GameManager.Start()` currently dereferences the `RunManager` lookup without a null check.
- `EnemyAI.OnEnable()` currently applies a development-only ten-second environmental stun. An empty encounter list or null entries can prevent progression; overlap attacks may hit a target more than once if it has multiple matching colliders.

## Validation

Check compilation and relevant Unity play-mode behavior when tooling permits. Verify actual GameObjects, components, Inspector values, input bindings and physics settings. The documentation review was static; gameplay was not tested in Unity.