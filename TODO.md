# Help Lucky Cat — development status and next steps

Updated November 13, 2026. [x] means implemented or reported working in play tests as specifically described, not blanket regression coverage. [ ] identifies work or verification still needed. See GAME_DESIGN.md for rules. Verify Inspector overrides and scene wiring before changing behavior.

## Scope and schedule

- [x] Gameplay feature freeze was planned for November 11. Cut or simplify optional unfinished features; focus on presentation and essential fixes.
- [x] Implement menu, ending and replay navigation; Play Again returns to OutsideTemple for a new Fortune. Pause and the audio sliders were reported working. The complete release loop is reported tested.
- [ ] Continue art, animation, music, sound, UI and integration; regression and exported-build testing remain. Previously noted jam deadline: Saturday November 14 at 7 AM; timezone not confirmed.

## November 13 handoff

- [x] Ability bar v1: independent ATK, STUN and ROLL cooldown fills and Health/Karma fills; user reported it working. Reel fill accounts for the shared spin gap. STUN shows combat stun cooldown, not every E action.
- [x] Boss-phase E keeps sealing priority but does not trigger player stun or start its cooldown. Verify in play alongside boss sealing and regular Encounter stun.
- [x] Pressure plate pressed/unpressed sprite references added. Intended unpressed sprite is `floor-wall-candle-plate_6`; pressed sprite is `floor-wall-candle-plate_31`.
- [ ] Retest pressure-plate sprite changes, wrong-order reset and completion after fixing duplicate IDs. Editor log reported duplicate ID 1; each entry in the puzzle's Plates list must reference the intended plate with a unique ID. Fix was discussed, not independently verified.
- [x] Lantern puzzle has shared lit/unlit sprite references; user reported it works. Timeout resets selected lantern visuals and Lucky Reel completion lights them in code; verify these specific paths if not yet tested.
- [ ] Continue HUD/feedback: Cleaning and lantern countdowns, objectives, Fortune/reel results, puzzle clues and seal eligibility. Happiness UI is separate; Lucky Cat hovering on the player is optional, low-priority polish.
- [ ] Preserve current uncommitted scene, prefab, art and script work; inspect Inspector values and scene overrides before making changes.

## Completed gameplay milestone

- [x] Opening Fortune generates puzzle/enemy counts and boss attack-frequency modifier. Cleaning selects distinct assigned instances, not types.
- [x] Pressure-plate puzzle, shuffled talisman locations and timed shuffled lantern locations. Play tests confirmed talisman and lantern functionality, including randomized lantern selection. Lantern difficulty was accepted as good enough.
- [x] E is the spiritual ability: selected talismans and lanterns in Cleaning, stun/seal in combat. Talisman interaction currently takes priority when both are nearby.
- [x] Waterfall trigger environmentally stuns regular enemies on entry; per-enemy cooldown; excludes boss. Play testing confirmed it works.
- [x] Encounter reel Bad speed, Neutral heal and Good Peaceful/Offensive effects. Mid-Karma Good now randomly chooses Peaceful or Offensive; the consistency change is approved. Do not assume every boundary and edge case was tested.
- [x] Boss reel Bad 1.5× frequency for 12 seconds, 25% shorter newly started warnings; Neutral +20 HP; Good high Karma 20 damage; Good low Karma 3-second regular stun; mid Karma randomly chooses either. Play tests confirmed Bad, Neutral and high-Karma damage work. Test low/mid and edge cases separately if not already covered.
- [x] Boss charge and fire breath choose randomly after initial charge, maximum two of one type in succession. Play testing found it acceptable. Melee range was debugged via Enemy AI attack range; check actual saved values rather than changing test settings.
- [x] Boss kill/seal resolution and player Game Over states exist in code. Player-death flow and both boss victory routes have been reported working; retest after integration changes.

## Next: playable start, endings and restart

- [x] Implement Play, Victory/Game Over and Play Again flows using placeholder UI; polish presentation and verify integration.
- [ ] Ensure gameplay timers, input, fortune and reels do not run behind a start screen. Handle scene loading/reset deliberately rather than assuming adding a panel pauses the game.
- [ ] Show correct Victory or Game Over once; verify boss kill and seal, player death, and no unintended transition after either end state.
- [x] The full release loop, including restart, is reported tested.
- [x] Play Again returns to OutsideTemple, then rolls fresh Fortune.

## Essential player-facing feedback and presentation

- [ ] Show controls, phase objectives, Cleaning timer, Happiness, Opening Fortune and Lucky Reel numbers/outcomes clearly; Health and Karma fills are implemented and reported working.
- [ ] Provide a visible readable pressure-plate sequence/clue and progress. The sequence debug log is disabled; Gizmos alone are not enough in a build.
- [ ] Add lantern timer/countdown and timeout/retry feedback. Lit/unlit sprites are implemented and reported working; Gizmos and Console messages alone are not build UI.
- [ ] Show talisman and lantern interaction cues, remaining progress and completion feedback; ensure talisman locations are reachable.
- [ ] Provide visible regular vs environmental/sealable stun and the boss low-health seal window; green/cyan Gizmos are not build UI.
- [ ] Apply art, animation, music, SFX and UI; confirm gameplay colliders, sorting and timing remain correct after visual replacements.

## Targeted tests and release

- [ ] Verify intended scene Happiness/Karma tuning if needed; prefab defaults are 50 and scene overrides may differ. Puzzle and enemy pools are assumed sufficient unless configuration changes.
- [ ] Recheck Good Cleaning auto-completion for each puzzle, final-puzzle transition, Bad time-to-zero, timeout penalties, and completion counted once. Talisman tests worked; confirm exact edge coverage if needed.
- [ ] Confirm lantern random combinations remain reachable within the saved time limit; timeout reuses the chosen locations, and reel completion awards once.
- [ ] Check Encounter high/low/mid Karma boundaries 44/45 and 55/56; selected fate result stays fixed even if a damage kill changes Karma. Test last-enemy reel kill, repeated speed refresh and expiration, and player healing at max health.
- [ ] Check waterfall re-entry/cooldown and dragon exclusion; test pillar stun, low-health seal, kill, Karma changes and one Victory transition.
- [ ] Check Boss speed boost refresh/expiry and warnings, stun/wake-on-hit, preserved pillar stun, mid-Karma lethal damage, random attack streak cap and resumed attacks after pillar stun.
- [ ] Check attack hitboxes for duplicate damage with multiple colliders; preserve combat and input behavior while adding UI.
- [ ] Verify regular enemy navigation and stun/resume after map changes. Navigation assets and rebakes need care; do not delete unfamiliar assets or change geometry without review.
- [x] Complete release loop and both boss victory routes are reported tested.

## Deferred / cut-first

Special triple combinations currently only log and use ordinary average classification. Additional puzzle systems, rooms, upgrades and other unapproved features are not required for this freeze. Do not add packages or architecture without approval.

## Final code review: concrete checks

- [ ] Confirm intended Player attack damage in the InsideTemple Inspector: the saved Player prefab has attackDamage 1000, potentially a test value; Boss prefab max HP is 100. Do not overwrite tuning without agreement.
- [ ] Verify saved Master/Music/SFX settings are applied after a fresh launch before opening Settings. Settings scene has AudioSettings wired; no AudioSettings component was found in the saved MainMenu scene. Verify mixer routing and WebGL persistence in a build.
- [ ] Check duplicate collider damage: player and enemy overlap attacks process colliders individually. Enemy melee uses Health on the hit object, so child-collider configurations also require testing.
- [x] Treat puzzle and enemy pools as sufficient unless the scene configuration changes.
- [ ] Ensure all key gameplay information has runtime UI: OutsideTemple Fortune is currently logged; pressure-plate clues, lantern countdown and sealing feedback must be readable without the Editor.
- [ ] Export and test the intended build. The complete release loop is reported tested; no build or Unity play test was run during this documentation update.
