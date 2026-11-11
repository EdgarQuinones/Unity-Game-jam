# Help Lucky Cat — MVP design

This document describes the current approved design direction, not a claim that every mechanic is implemented. See `TODO.md` for implementation status.

## Vision and scope

A short, replayable, top-down Unity 2D action game for the Lucky Number game jam. In an East Asian-inspired supernatural temple, the player helps Lucky Cat through puzzles, spirits and a Fire Dragon. Luck influences the entire run, not just its final reward.

The MVP uses one handcrafted room, one reusable pressure-plate puzzle, one regular enemy type, one boss, Opening Fortune, a Hybrid Lucky Reel, Happiness, Karma Debt, killing and peaceful sealing, victory, game over and restarting the same scene. Additional rooms, permanent upgrades, win streaks, dash and ranged attacks are outside this scope.

## Run loop

Opening Fortune → Cleaning → Encounter → Fire Dragon → Victory or Game Over → Restart.

At the start of each run, three independent 1–9 rolls determine:

| Roll | 1–3 | 4–6 | 7–9 |
| --- | --- | --- | --- |
| Cleaning | 3 puzzles | 2 puzzles | 1 puzzle |
| Encounter | 4 enemies | 3 enemies | 2 enemies |
| Boss | Faster attacks | Normal attacks | Slower attacks |

Select one, two or three distinct puzzles from the available Cleaning puzzle pool. The reusable pressure-plate prefab can provide multiple instances; each generates its own random sequence, which stays the same when a wrong attempt resets. Cleaning times are 30, 50 and 70 seconds for one, two and three puzzles respectively. Puzzle successes improve Happiness; mistakes can reduce it. Future puzzle types can replace individual pressure-plate instances.

Regular enemies can be killed, increasing Karma Debt, or environmentally stunned and sealed, reducing it. Resolving them advances the encounter. The Fire Dragon has a charge, cone fire breath, warnings and recovery. A charge into a pillar environmentally stuns it; sealing requires environmental stun and health at or below its configured threshold. The saved scene boss has 200 HP; confirm the Inspector threshold. Either kill or seal is intended to win.

## Hybrid Lucky Reel

Three separately stored 1–9 numbers are generated per spin. Q manually spins with a 12-second cooldown; automatic spins are configured for every 20 seconds; any two spins must be at least five seconds apart. Manual and automatic spins share outcomes. Spins are disabled during Game Over and Victory; transition-state restrictions remain planned.

For the temporary MVP classification, average the three numbers: below 4 is Bad, 4 to below 7 is Neutral, and 7 or above is Good. Special combinations are intended to override ordinary tiers later; currently triple matches are logged and still use the average.

Happiness weights each displayed number: higher Happiness favors higher values, while every number remains possible. For Good Encounter outcomes, Karma Debt 0–44 is Peaceful, 45–55 Neutral, and 56–100 Offensive. Boss Good effects and their neutral outcome remain to be designed and implemented. Karma does not affect Cleaning, Neutral or Bad reel outcomes for the MVP. Special-combination effects and probabilities remain to be implemented and balanced.

Planned phase effects:

- **Cleaning:** bad loses 10 seconds; neutral gains 10 seconds with no cap; good completes one selected unfinished puzzle and grants its normal Happiness reward. Reaching zero time transitions to Encounter.
- **Encounter:** bad temporarily speeds enemies; neutral heals; good damages all active enemies at high Karma, regularly stuns them at neutral Karma, or environmentally stuns one active enemy at low Karma.
- **Boss:** bad temporarily speeds boss attacks; neutral heals; good damages the boss at high Karma or helps weaken/stun it for sealing at low Karma. Sealing requirements must still be respected.

Temporary effects must expire, avoid infinite stacking or permanent stat corruption, and reset on a new run. Special combinations such as 8-8-8 and 7-7-7 are design ideas; their effects are not established here.

## Player and values

- Controls: WASD movement, left click melee, E stun or seal, Q reel; use the existing Input System and Unity Events.
- Normal attacks, stuns and sealing have no passive random failure. Range, hitbox and eligibility still matter.
- Happiness: 0–100, starting at 50; determines quality of luck.
- Karma Debt: 0–100, starting at 50; kills increase it, seals decrease it, and it directs good-roll effects. Any enemy-damage scaling should remain modest.
- Karma-at-100 instant loss and vision-darkening are not approved MVP mechanics.