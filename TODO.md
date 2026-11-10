# Help Lucky Cat — development status

Status is based on static inspection on November 9, 2026, not a Unity play-mode test. Design targets are in `GAME_DESIGN.md`. Check current source and scene wiring before marking items complete.

## Current priority: gambling foundation

- [ ] Verify Opening Fortune startup in Unity: attach/configure `RunManager` if needed and confirm `GameManager` initializes without a missing reference.
- [ ] Wire and test manual Q Lucky Reel spins in the actual scene; verify the 12-second cooldown, 30-second automatic interval and five-second shared gap.
- [ ] Disable manual and automatic spins in transitions, Game Over and Victory; define transition handling when that state exists.
- [ ] Detect the gameplay phase and route reel results to the appropriate effect handling.

### Existing code, still requiring integration or testing

- `RunManager` generates three 1–9 fortune rolls and calculates the specified puzzle counts, enemy counts and boss-speed multipliers. `GameManager` reads the puzzle count, but enemy count and boss speed are not applied by reviewed scripts.
- `LuckyReel` generates three separate 1–9 values, logs triple matches and implements the timer logic. No component reference or player spin callback was found in the inspected scene/prefabs.
- Normal player attacks and stuns have no passive random miss in the reviewed code; test their natural range and eligibility in Unity.

## Remaining MVP work

- [ ] Implement Happiness-weighted luck and Karma-dependent good outcomes; agree on tier calculation and special-combination probabilities.
- [ ] Reset the pressure-plate puzzle for the required number of sequences; apply the planned Cleaning timer and reel effects.
- [ ] Apply the fortune enemy count and implement encounter reel effects.
- [ ] Implement Fire Dragon attacks, fortune attack-speed modifier, boss reel effects and health/environment-based sealing requirements.
- [ ] Complete and test victory, game over and same-scene restart; ensure temporary effects and run data reset.
- [ ] Fix build scene configuration and verify a playable build; test outcomes, softlocks, death, victory and restart.

## Planning target

The intended feature freeze is Thursday, November 12, 2026; Friday is for testing and debugging only. The jam deadline is Saturday, November 14, 2026 at 7 AM (timezone not specified). These dates are targets, not evidence of implementation or completion.