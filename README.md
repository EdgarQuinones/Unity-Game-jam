# Help Lucky Cat

**Help Lucky Cat** is a work-in-progress, top-down Unity 2D game created for a game jam with the theme **Lucky Number**. Explore an East Asian-inspired supernatural temple, solve a pressure-plate puzzle, face spirits, and confront a Fire Dragon—all while luck changes the challenges of the run.

The goal is a short, replayable game in which luck matters throughout the experience, not just at the end.

## The game loop

The intended MVP loop is:

**Opening Fortune → Cleaning → Encounter → Fire Dragon → Victory or Game Over → Restart**

- **Opening Fortune:** Three numbers are rolled at the start of a run. The design uses them to set the puzzle count, enemy count, and boss attack speed.
- **Cleaning:** Complete pressure-plate sequences while managing the clock and Cat Happiness.
- **Encounter:** Fight spirits or pursue a peaceful approach by sealing environmentally stunned enemies.
- **Boss:** Confront the Fire Dragon. The intended design allows both victory through combat and victory through sealing.
- **Lucky Reel:** Spin three numbers during gameplay. The planned effects depend on the current phase, Cat Happiness, and Karma Debt.

## Current status

This is an **in-development game jam project**, not a finished release. The project includes player movement and combat, enemy behaviors, health, a pressure-plate puzzle, progression states, and Opening Fortune calculations. Killing and sealing enemies affect Karma.

The Lucky Reel supports manual and automatic spins, classifies the three-number average as Bad, Neutral, or Good, and logs the result for the current gameplay phase. **The phase-specific reel effects are not implemented yet.** Happiness-weighted rolls, special-combination effects, some fortune modifiers, boss-specific mechanics, and a complete victory/game-over/restart flow are also still in progress. Some existing behavior and scene setup may need further testing.

See [GAME_DESIGN.md](GAME_DESIGN.md) for the intended mechanics and [TODO.md](TODO.md) for the development checklist; status details in those documents may need updating as work progresses.

## Controls

| Input | Action |
| --- | --- |
| WASD or arrow keys | Move |
| Left mouse button | Melee attack |
| E | Stun, or seal an eligible enemy |
| Q | Spin the Lucky Reel |

The reel uses a 12-second manual cooldown, a 30-second automatic spin interval, and a minimum five-second gap between spins. Sealing requires an eligible environmental stun; the existing environmental stun is currently a development test behavior.

## Open the project

1. Install **Unity 6000.3.9f1** with Unity Hub.
2. Clone or download this repository and add its root folder to Unity Hub.
3. Open the project and allow Unity to import the assets and resolve its packages.
4. Open `Assets/_Project/Scenes/Game.unity` and press **Play**.

The project uses Unity's Input System, Cinemachine, and the Universal Render Pipeline. Package versions are listed in `Packages/manifest.json`. The Game scene is configured in the build settings; an exported build has not been verified here.

## Project structure

```text
Assets/
├── _Project/
│   ├── Art/       Artwork and UI assets
│   ├── Prefabs/   Player, enemy, boss, and environment prefabs
│   ├── Scenes/    Game and UI scenes
│   └── Scripts/   Gameplay C# scripts
└── InputSystem_Actions.inputactions
Packages/          Unity package manifest and lock file
ProjectSettings/   Unity project configuration
```

The game uses existing Unity components and scripts for movement, combat, health, puzzles, fortune, and phase progression. For the current technical overview, see [.clinerules/project.md](.clinerules/project.md).
