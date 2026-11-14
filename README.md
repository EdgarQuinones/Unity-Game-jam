# Perrfect Fortune

A completed top-down Unity 2D game made for a Lucky Number game jam. Cleanse a supernatural temple, face spirits, and confront the Fire Dragon while fortune shapes each run.

## Project status — Complete

Development is complete, as confirmed by the project owner on **November 14, 2026**. This repository contains the finished jam project: Opening Fortune, three Cleaning puzzle types, spirit encounters, the dragon boss, Lucky Reel effects, presentation, menus, settings, pause, endings and replay.

The complete play loop and both boss victory routes were previously reported tested. The former development backlog is closed; `TODO.md` now records completion and optional maintenance checks rather than outstanding release work. This documentation update did not run Unity or retest the exported build.

## Features

- **A different fortune each run:** three opening rolls determine puzzle count, enemy count and dragon attack pacing.
- **Three cleansing challenges:** pressure-plate sequences, randomly selected talismans and a timed lantern puzzle.
- **Two ways to resolve combat:** defeat spirits with melee attacks or use environmental stuns to seal them. Both killing and sealing can resolve the dragon fight.
- **Luck with consequences:** Happiness weights the Lucky Reel; Karma Debt guides its peaceful or offensive Good combat results.
- **Runtime feedback:** ability cooldowns, Health and Karma bars, a separate Lucky Cat Happiness UI, Cleaning time/progress, boss health and a recent-events log with reel results and readiness.
- **A complete session flow:** introduction, settings and audio controls, pause, victory/defeat screens and a new Fortune when replaying.

Lucky Cat is a separate spirit represented by the Happiness UI, not the player character.

## The loop

Main Menu → Temple Introduction / Opening Fortune → Cleaning → Encounter → Fire Dragon → Victory or Defeat → Play Again.

Opening Fortune sets the number of puzzles and enemies and the boss attack frequency. Complete cleansing challenges before the Cleaning timer expires, then fight regular spirits or seal them after an environmental stun; waterfalls provide that opportunity. The dragon can be killed or sealed after a pillar stun at sufficiently low health. Play Again returns to the temple introduction for a fresh Fortune.

The Lucky Reel uses Happiness-weighted 1–9 rolls, and Bad, Neutral and Good effects depend on the current phase. Good combat effects use Karma Debt: low favors peaceful effects, high favors offensive effects, and mid Karma randomly chooses between them. See GAME_DESIGN.md for details.

## Controls

| Input | Action |
| --- | --- |
| WASD / arrows | Move |
| Left mouse | Melee attack |
| E | Spiritual ability: cleanse talismans or light lanterns in Cleaning; stun or seal in combat |
| Q | Spin Lucky Reel, subject to cooldown |
| Escape | Pause / resume |

## Open and play in Unity

1. Open the repository in Unity Hub with **Unity 6000.3.9f1**.
2. Allow packages and assets to import, then open `C:\Users\warri\Documents\GitHub\Unity Game jam\Assets\_Project\Scenes\MainMenu.unity` and enter Play mode.
3. Choose Play, follow the temple introduction and Opening Fortune, then enter the temple.
4. For direct gameplay testing, open `C:\Users\warri\Documents\GitHub\Unity Game jam\Assets\_Project\Scenes\InsideTemple.unity`; it generates fresh Fortune data when there is no pending introduction roll.

The saved build scene order is **MainMenu → OutsideTemple → InsideTemple**. Settings use panels, and victory/defeat use panels inside InsideTemple. Separate Settings and GameOver scene assets remain in the source project but are not enabled in the current build scene list.

## Local WebGL build

A WebGL export is present at `C:\Users\warri\Documents\GitHub\Unity Game jam\Builds\v1\`, with an archive at `C:\Users\warri\Documents\GitHub\Unity Game jam\Builds\v1.zip`. Serve the extracted build through a WebGL-compatible web server or host and open its `index.html` in a browser. The build files are local artifacts ignored by Git; they are not guaranteed to be available in a fresh clone. No public download or hosted-play URL is recorded here.

The project uses Unity Input System, Cinemachine, AI Navigation, and URP. Regular enemies use NavMesh-assisted Rigidbody2D movement. Preserve navigation bake data and verify paths after environmental changes; rebake deliberately when required.

## Repository guide

Paths below use the current checkout root, `C:\Users\warri\Documents\GitHub\Unity Game jam`:

| Absolute path | Contents |
| --- | --- |
| `C:\Users\warri\Documents\GitHub\Unity Game jam\GAME_DESIGN.md` | Final gameplay reference and reel outcomes |
| `C:\Users\warri\Documents\GitHub\Unity Game jam\TODO.md` | Completion record and optional maintenance checks |
| `C:\Users\warri\Documents\GitHub\Unity Game jam\Assets\_Project\Scripts\` | Gameplay, flow and presentation C# |
| `C:\Users\warri\Documents\GitHub\Unity Game jam\Assets\_Project\Scenes\` | Menu, exterior, gameplay and retained scene assets |
| `C:\Users\warri\Documents\GitHub\Unity Game jam\Assets\_Project\Prefabs\` | Reusable GameObjects |
| `C:\Users\warri\Documents\GitHub\Unity Game jam\Assets\_Project\Art\` | Art assets |
| `C:\Users\warri\Documents\GitHub\Unity Game jam\Assets\InputSystem_Actions.inputactions` | Input actions |
| `C:\Users\warri\Documents\GitHub\Unity Game jam\Packages\manifest.json` | Package dependencies |
| `C:\Users\warri\Documents\GitHub\Unity Game jam\ProjectSettings\` | Unity project configuration |

Inspect scene and prefab Inspector values before relying on script defaults. Editor Gizmos and Console logs are not player-facing build UI.
