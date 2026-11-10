# Help Lucky Cat — development status

Status combines code inspection with the owner's reported Unity tests. Design targets are in `GAME_DESIGN.md`; check current source and scene wiring before making changes.

## Current priority: gambling foundation

- [ ] Verify Opening Fortune startup and modifiers in Unity. `RunManager` is attached; enemy count and boss speed are not yet applied.
- [ ] Verify the complete 12-second manual cooldown, 30-second automatic interval and five-second shared gap in Unity. The scene has a Q spin callback.
- [x] Detect the gameplay phase and log the Bad, Neutral or Good result for that phase. The owner reported the result looked good in testing.
- [ ] Route results to actual phase-specific gameplay effects; current routing only logs.
- [ ] Define and block spins during transitions when a transition state exists. Blocking in GameOver and Complete is implemented; the owner reported the win/loss spin restriction worked.

### Existing code, still requiring integration or testing

- `RunManager` generates three 1–9 fortune rolls and calculates the specified puzzle counts, enemy counts and boss-speed multipliers. `GameManager` reads the puzzle count, but enemy count and boss speed are not applied by reviewed scripts.
- `LuckyReel` generates three separate 1–9 values, logs triple matches and implements the timer logic. It is attached to the scene's GameManager; the Player instance has a spin callback.
- The owner confirmed player death reaches GameOver in Unity. Boss victory and the full victory/restart flow still need verification.
- Normal player attacks and stuns have no passive random miss in the reviewed code; test their natural range and eligibility in Unity.

## Remaining MVP work

- [ ] Implement Happiness-weighted luck and Karma-dependent good outcomes; balance special-combination probabilities. Average-based tiers are the temporary MVP rule.
- [ ] Reset the pressure-plate puzzle for the required number of sequences; apply the planned Cleaning timer and reel effects.
- [ ] Apply the fortune enemy count and implement encounter reel effects.
- [ ] Implement Fire Dragon attacks, fortune attack-speed modifier, boss reel effects and health/environment-based sealing requirements.
- [ ] Complete and test victory, game over and same-scene restart; ensure temporary effects and run data reset.
- [ ] Verify a playable build and test outcomes, softlocks, death, victory and restart. `Game.unity` is now listed in build settings; an exported build has not been verified.

## Planning target

The intended feature freeze is Thursday, November 12, 2026; Friday is for testing and debugging only. The jam deadline is Saturday, November 14, 2026 at 7 AM (timezone not specified). These dates are targets, not evidence of implementation or completion.