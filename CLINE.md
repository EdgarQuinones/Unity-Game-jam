# Project knowledge

This is a Unity game jam project. This document is a snapshot of the code and serialized assets reviewed on November 9, 2026; verify the current implementation and scene wiring before relying on it or changing behavior.

## Working with the owner

- Follow existing C# and Unity conventions. Prefer simple, maintainable, Unity-compatible solutions and avoid unnecessary packages or dependencies.
- Do not assume a feature exists without checking. Ask before making major architectural changes.
- Explain code changes and why they were made so the owner can learn.
- Inspect relevant scripts, scenes, and prefabs before changing behavior; validate changes in Unity when possible and report any testing limitations.

## Project layout and environment

- Unity editor version: **6000.3.9f1** (`ProjectSettings/ProjectVersion.txt`).
- Gameplay scripts: `Assets/_Project/Scripts/` (15 C# files, no script subfolders as of this review).
- Scenes: `Assets/_Project/Scenes/Game.unity` and `UI.unity`.
- Prefabs: `Assets/_Project/Prefabs/` includes Player, Melee, Boss, PressurePlate, and Wall.
- Art: `Assets/_Project/Art/`. Input actions: `Assets/InputSystem_Actions.inputactions`.
- The package manifest includes the Input System, Cinemachine, URP, Unity Test Framework, and other packages. Check actual scene and asset usage before removing any package.

## Gameplay architecture

Most systems are MonoBehaviours communicating through component calls, serialized references, tag lookup, and `FindFirstObjectByType`. `Health.OnDeath` is an event used by death handlers. Scene and prefab configuration matters: code existing does not mean it is wired into play.

- **Player:** `PlayerMovement` reads Input System callbacks, moves a `Rigidbody2D`, and exposes facing direction. `PlayerCombat` uses 2D overlap queries and a layer mask for directional attacks, area stun, and sealing. Sealing takes priority over stun and requires an environmentally stunned target.
- **Enemies:** `EnemyAI` manages chase, attack, and stunned states, target selection, sealing, and boss identification. `EnemyMovement` and `EnemyCombat` read its state. Enemy attacks scale damage with karma. `EnemyDeath` subscribes to health death, adds karma, and reports resolution to `GameManager`.
- **Health:** `Health` initializes health, takes damage, wakes stunned enemies, flashes a sprite, invokes `OnDeath`, and deactivates the object. It also provides healing. `PlayerDeath` is intended to report player death to `GameManager`.
- **Karma:** `Karma` stores a value clamped to 0–100. Killing an enemy increases it; sealing decreases it. `EnemyCombat` interpolates its damage multiplier from the current value.
- **Happiness and puzzles:** `CatHappiness` stores happiness. `PressurePlate` forwards player trigger entries to its parent `PressurePlatePuzzle`, which checks a sequence, adjusts happiness, and reports success to `GameManager`. No other gameplay consumer of happiness was found in the reviewed scripts.
- **Progression:** `GameManager` runs Cleaning → Encounter → Boss → Complete, with a GameOver state. Cleaning ends when its timer expires or the required number of puzzles completes; resolved encounter enemies advance to the boss; resolving the boss completes the level. `RunManager` rolls puzzle count, enemy count, and boss speed multiplier, but only puzzle count is consumed by another script in the reviewed code.
- **Lucky reel:** `LuckyReel` rolls three numbers manually or on a timer, with cooldowns and a shared minimum gap. It logs results; no gameplay effect is connected in the reviewed scripts.

## Current wiring and known risks

These are static-review findings, not verified play-mode results. Recheck after project changes:

1. `ProjectSettings/EditorBuildSettings.asset` points to `Assets/Scenes/SampleScene.unity`, which was absent. The two project scenes were not listed there.
2. No `RunManager` component reference was found in the project scenes or prefabs, but `GameManager.Start()` dereferences the result of `FindFirstObjectByType<RunManager>()` without a null check. This can stop initialization.
3. No `PlayerDeath` component reference was found in the project scenes or prefabs; absent runtime setup, player death will not notify the game manager.
4. `EnemyAI.OnEnable()` applies a 10-second environmental stun marked as development-only. No other environmental stun caller was found in the scripts.
5. Encounter progression does not automatically advance for an empty enemy array, and null array entries count toward the completion threshold. The fortune-generated enemy count and boss-speed multiplier are not applied by the reviewed scripts.
6. Missing required scene/component references can produce null-reference exceptions, including in puzzle activation and enemy movement when no player target is available.
7. Overlap-based attacks process each collider rather than each unique target, so multiple matching colliders on one target may cause repeated damage.

## Validation

Use the Unity editor version recorded in `ProjectSettings/ProjectVersion.txt`. Check the actual scene, prefab, input-event, tag, and layer setup when working on gameplay. Run relevant Unity tests and/or play-mode checks when available; do not claim they passed unless actually run. No play-mode validation was performed for this document.