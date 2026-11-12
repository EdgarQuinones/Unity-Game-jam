# Help Lucky Cat

A work-in-progress top-down Unity 2D game for a Lucky Number game jam. Cleanse a supernatural temple, face spirits, and confront a Fire Dragon while fortune shapes each run.

## Current status — November 11, 2026

The core gameplay is substantially implemented: three Cleaning puzzle types, Encounter and Boss combat, killing and sealing, and phase-specific Lucky Reel effects. Play tests have confirmed the talisman and lantern puzzles, waterfall stun, and several Boss reel effects. That does not mean every edge case or full run is verified.

The game is **not yet release-ready**: player-facing gameplay feedback, art/audio integration, full-loop regression and an exported build remain; menu, pause, ending and replay flows are implemented but still need end-to-end verification. Gameplay features are intended to freeze tonight; tomorrow is reserved primarily for presentation and integration. See TODO.md for priorities and verification status.

## The loop

Opening Fortune → Cleaning → Encounter → Fire Dragon → Victory or Game Over → Restart. Opening Fortune sets the number of puzzles and enemies and the boss attack frequency. Cleaning features pressure-plate sequences, cleansing randomly placed talismans and a timed lantern challenge. Fight regular spirits or seal them after an environmental stun; the waterfall can provide that stun. The dragon can be killed or sealed after a pillar stun at sufficiently low health.

The Lucky Reel uses Happiness-weighted 1–9 rolls, and Bad, Neutral and Good effects depend on the current phase. Good combat effects use Karma Debt: low favors peaceful effects, high favors offensive effects, and mid Karma randomly chooses between them. See GAME_DESIGN.md for details.

## Controls

| Input | Action |
| --- | --- |
| WASD / arrows | Move |
| Left mouse | Melee attack |
| E | Spiritual ability: cleanse talismans or light lanterns in Cleaning; stun or seal in combat |
| Q | Spin Lucky Reel, subject to cooldown |
| Escape | Pause / resume |

## Open the project

1. Open the repository in Unity Hub with **Unity 6000.3.9f1**.
2. Allow packages and assets to import, then open `Assets/_Project/Scenes/MainMenu.unity` and enter Play mode (or `Assets/_Project/Scenes/InsideTemple.unity` for direct gameplay testing).
3. MainMenu, Settings, OutsideTemple, InsideTemple and GameOver are enabled in Build Settings. An exported build and complete release flow have not yet been verified.

The project uses Unity Input System, Cinemachine, AI Navigation, and URP. Regular enemies use NavMesh-assisted Rigidbody2D movement. Preserve navigation bake data and verify paths after environmental changes; rebake deliberately when required.

## Files

- `GAME_DESIGN.md` — mechanics, reel outcomes and scope.
- `TODO.md` — next steps, tested versus unverified behavior and release checklist.
- `Assets/_Project/Scripts/` — gameplay C#.
- `Assets/_Project/Scenes/` — menu, settings, exterior, gameplay and ending scenes.
- `Assets/_Project/Prefabs/` and `Assets/_Project/Art/` — reusable assets and art.
- `Assets/InputSystem_Actions.inputactions` — input actions.
- `Packages/manifest.json` and `ProjectSettings/` — dependencies and configuration.

Inspect scene and prefab Inspector values before relying on script defaults. Editor Gizmos and Console logs are not player-facing build UI.
