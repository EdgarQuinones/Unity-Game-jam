# Help Lucky Cat — final game design

Completed game reference, updated November 14, 2026. The project owner has confirmed development is complete. This document describes the finished jam scope; the completion record is in `C:\Users\warri\Documents\GitHub\Unity Game jam\TODO.md`. Inspector overrides determine saved runtime tuning. Completion status is not a claim that every edge case was independently tested during this documentation update.

## Vision and scope

A short, replayable top-down 2D Lucky Number jam game in a supernatural temple. The player cleanses the temple, faces umbrella spirits and confronts the Fire Dragon. Lucky Cat is a separate spirit represented by the Happiness UI, not the player character. The jam scope is complete; additional companions, rooms, puzzle systems and upgrades are outside the finished scope rather than unfinished requirements.

## Run loop

MainMenu → OutsideTemple introduction / Opening Fortune → InsideTemple Cleaning → Encounter → Boss → Victory or Defeat → Play Again. Endings are panels inside InsideTemple, not a transition to the retained GameOver scene. Play Again returns to OutsideTemple for a fresh roll; returning to MainMenu is also supported. Settings and pause use panels. The saved build enables MainMenu, OutsideTemple and InsideTemple in that order. The complete release loop and both boss victory routes were previously reported tested.

Three independent Opening Fortune rolls are generated at run start:

| Roll | 1–3 | 4–6 | 7–9 |
| --- | --- | --- | --- |
| Cleaning | 3 puzzles | 2 puzzles | 1 puzzle |
| Encounter | 6 enemies | 4 enemies | 2 enemies |
| Boss attack frequency multiplier | 1.25× | 1× | 0.75× |

The boss frequency multiplier divides the base wait between attacks: values above 1 mean faster attacks, not longer waits.

Cleaning selects distinct puzzle instances, not distinct types. Timers are 30, 50 or 70 seconds; each unfinished puzzle costs 10 Happiness at timeout. Completed puzzles grant 10 Happiness. The puzzle pool is assumed sufficient unless configuration changes.

### Cleaning puzzles

- **Pressure plates:** enter a randomly generated sequence; a wrong plate resets progress and costs 1 Happiness. The same sequence is retained during retries. Correct plates use the assigned pressed sprite; reset restores the unpressed sprite. Each assigned plate needs a unique ID.
- **Talismans:** randomly activate a fixed number of placed locations (code default 4); press E near the nearest active talisman to cleanse it. Cleanse all selected to finish.
- **Lanterns:** randomly activate a fixed number of placed lanterns (code default 4). E lights them in any order; the first starts an Inspector-configured timer. Timeout resets the same selected lanterns for another attempt.

All three use CleaningPuzzle and support Good-reel completion. Talisman and lantern interactions are separate from combat; if both are in range, talisman takes priority. Lanterns use assigned lit/unlit sprites, including on timeout and reel completion. The gameplay HUD displays the overall Cleaning countdown and completed/required puzzle count; the event log provides phase instructions and completion feedback.

## Combat and dragon

Movement uses WASD/arrows; left click attacks. E is the spiritual ability: cleanse/light during Cleaning, and stun or seal during combat. Seal takes priority; in the Boss phase E does not trigger player stun or start its cooldown, but can still seal an eligible boss. Regular player stun is not sealable and wakes on damage. Environmental stun enables sealing, which decreases Karma Debt; killing raises it. The waterfall is a pass-through trigger that environmentally stuns regular enemies on entry, not the dragon, with per-enemy per-waterfall cooldown. Entry must happen again after cooldown.

The Fire Dragon uses a charge and fire breath with warnings and recovery. After an initial charge, attacks are selected randomly with at most two of one type consecutively. A charge into a pillar environmentally stuns the dragon; sealing also requires health above zero and at or below its configured threshold. Boss melee uses Enemy AI attack range for state selection and Enemy Combat range/radius for its hit area. Use saved prefab and scene values for health, damage and sealing thresholds rather than assuming script defaults. Both killing and sealing resolve the boss and display Victory; player death displays Defeat.

## Lucky Reel

Three 1–9 numbers are drawn, biased by Happiness while every number remains possible. Their average is Bad below 4, Neutral from 4 to below 7, and Good at least 7. Q requests a manual spin; code defaults include 12-second manual cooldown, 20-second automatic interval, five-second first automatic spin and five-second shared minimum gap. Inspect saved overrides. Spins are blocked in Complete and GameOver. Triple matches use the ordinary average classification; special combination bonuses are outside the finished scope.

| Phase | Bad | Neutral | Good |
| --- | --- | --- | --- |
| Cleaning | −10 seconds | +10 seconds, no cap | Complete one selected unfinished puzzle with its usual reward |
| Encounter | 1.5× enemy movement for 8 seconds; refresh, no stacking | Heal up to 20 HP | Low Karma: environmental stun one enemy for 8 seconds, preferring an enemy not already sealable. High: 20 damage to all active unresolved enemies. |
| Boss | 1.5× attack frequency for 12 seconds; warnings that begin while boosted are 25% shorter; refresh, no stacking | Heal up to 20 HP | Low Karma: regular 3-second stun, wakes on hit; high: 20 damage, potentially lethal. |

For Good combat results, low Karma Debt is 0–44, neutral is 45–55, high is 56–100. At neutral Karma, fate randomly selects the corresponding Peaceful or Offensive result with equal probability in either combat phase. Offensive damage may kill. Boss regular stun never grants environmental sealing eligibility and does not replace an existing pillar stun.

Happiness and Karma Debt are clamped to 0–100. Happiness biases reel quality; Karma guides Good combat effects and scales incoming enemy damage. Saved scene and prefab values determine starting amounts. Normal attacks and stun/seal inputs have no passive random failure.

## Presentation and session flow

The completed scope includes art, animation, audio, UI and the full start-to-ending/replay flow, as confirmed by the project owner. Current presentation systems include:

- An ability bar for attack, stun and manual reel cooldown progress, plus Health and Karma fills. Reel progress includes the shared post-spin gap; stun fill represents combat stun, not all E interactions.
- A separate Lucky Cat Happiness UI.
- A Cleaning countdown and puzzle completion counter, plus a boss-health panel during the dragon fight.
- A runtime recent-events panel for phase instructions, puzzle/enemy progress, reel numbers/outcomes/effects, manual readiness and automatic-spin timing.
- An OutsideTemple introduction that presents Opening Fortune before entering gameplay.
- Settings panels with saved Master/Music/SFX levels; menu music applies saved mixer values at startup.
- In-game pause and victory/defeat panels, with replay returning to a fresh Opening Fortune.

The separate Settings and GameOver scene assets are retained source assets, not required scene transitions in the current build. A local WebGL export exists at `C:\Users\warri\Documents\GitHub\Unity Game jam\Builds\v1\`; its presence does not establish fresh browser-test coverage. No Unity play test or build execution was performed for this documentation update.
