# Help Lucky Cat — current design

Updated November 11, 2026. This describes the approved gameplay direction and current implementation; see TODO.md for testing and remaining work. Inspector overrides and play tests determine actual runtime behavior.

## Vision and scope

A short, replayable top-down 2D Lucky Number jam game in a supernatural temple. Help Lucky Cat cleanse the temple, resolve spirits and confront the Fire Dragon. The owner intends to freeze gameplay features tonight, November 11; unfinished extras are cut or simplified. November 12 is for art, music, sound and UI integration. Bug fixes and essential release work remain necessary.

## Run loop

Opening Fortune → Cleaning → Encounter → Boss → Victory or Game Over → Restart. The game-state transitions exist; player-facing start, endings and restart are still to be implemented.

Three independent Opening Fortune rolls are generated at run start:

| Roll | 1–3 | 4–6 | 7–9 |
| --- | --- | --- | --- |
| Cleaning | 3 puzzles | 2 puzzles | 1 puzzle |
| Encounter | 4 enemies | 3 enemies | 2 enemies |
| Boss attack wait multiplier | 1.25× frequency | 1× | 0.75× frequency |

Cleaning selects distinct puzzle instances, not distinct types. Timers are 30, 50 or 70 seconds; each unfinished puzzle costs 10 Happiness at timeout. Completed puzzles grant 10 Happiness. Verify the configured pool and runtime overrides in Unity.

### Cleaning puzzles

- **Pressure plates:** enter a randomly generated sequence; a wrong plate resets progress and costs 1 Happiness. The same sequence is retained during retries.
- **Talismans:** randomly activate a fixed number of placed locations (code default 4); press E near the nearest active talisman to cleanse it. Cleanse all selected to finish.
- **Lanterns:** randomly activate a fixed number of placed lanterns (code default 4). E lights them in any order; the first starts an Inspector-configured timer. Timeout resets the same selected lanterns for another attempt. The owner reported the puzzle works and is easy but acceptable.

All three use CleaningPuzzle and support Good-reel completion. Talisman and lantern interactions are separate from combat; if both are in range, talisman takes priority. Lit lantern indication is currently an Editor Gizmo, not player-facing feedback. A visible lit state, countdown and clear instructions remain needed.

## Combat and dragon

Movement uses WASD/arrows; left click attacks. E is the spiritual ability: cleanse/light during Cleaning, and stun or seal during combat. Regular player stun is not sealable and wakes on damage. Environmental stun enables sealing, which decreases Karma Debt; killing raises it. The waterfall is a pass-through trigger that environmentally stuns regular enemies on entry, not the dragon, with per-enemy per-waterfall cooldown. Entry must happen again after cooldown.

The Fire Dragon uses a charge and fire breath with warnings and recovery. After an initial charge, attacks are selected randomly with at most two of one type consecutively. A charge into a pillar environmentally stuns the dragon; sealing also requires health above zero and at or below its configured threshold. The Boss prefab currently has 100 max HP and a 10% seal threshold; scene overrides and tuning must be checked before making balance claims. Boss melee uses Enemy AI attack range for state selection and Enemy Combat range/radius for its hit area; the owner adjusted the former after testing. Both killing and sealing can resolve the boss; verify both complete victory paths.

## Lucky Reel

Three 1–9 numbers are drawn, biased by Happiness while every number remains possible. Their average is Bad below 4, Neutral from 4 to below 7, and Good at least 7. Q requests a manual spin; code defaults include 12-second manual cooldown, 20-second automatic interval, five-second first automatic spin and five-second shared minimum gap. Inspect saved overrides. Spins are blocked in Complete and GameOver. Triple matches do not have distinct gameplay effects; special combinations are a cut-first extra, not a release requirement.

| Phase | Bad | Neutral | Good |
| --- | --- | --- | --- |
| Cleaning | −10 seconds | +10 seconds, no cap | Complete one selected unfinished puzzle with its usual reward |
| Encounter | 1.5× enemy movement for 8 seconds; refresh, no stacking | Heal up to 20 HP | Low Karma: environmental stun one enemy for 8 seconds, preferring an enemy not already sealable. High: 20 damage to all active unresolved enemies. |
| Boss | 1.5× attack frequency for 12 seconds; warnings that begin while boosted are 25% shorter; refresh, no stacking | Heal up to 20 HP | Low Karma: regular 3-second stun, wakes on hit; high: 20 damage, potentially lethal. |

For Good combat results, low Karma Debt is 0–44, neutral is 45–55, high is 56–100. At neutral Karma, fate randomly selects the corresponding Peaceful or Offensive result with equal probability in either combat phase. Offensive damage may kill. Boss regular stun never grants environmental sealing eligibility and does not replace an existing pillar stun.

Happiness and Karma Debt are clamped to 0–100; prefab defaults are 50 each. Happiness biases reel quality; Karma guides Good combat effects and scales incoming enemy damage. Actual scene overrides must be verified. Normal attacks and stun/seal inputs have no passive random failure.

## Presentation and finish

Make player-facing start, Victory, Game Over and restart flow; explain controls, Opening Fortune, reel outcomes, phase goals and Karma/Happiness. Provide readable pressure-plate clues, lantern state/countdown and environmental-stun/seal eligibility indicators in builds; Gizmos and Console logs are Editor-only. Art, animations, music, sound, and polished UI follow. Confirm full-run behavior and an exported build before release.
