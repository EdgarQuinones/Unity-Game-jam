# Help Lucky Cat — development status and next steps

Updated November 11, 2026. [x] means implemented or reported working in play tests as specifically described, not blanket regression coverage. [ ] identifies work or verification still needed. See GAME_DESIGN.md for rules. Verify Inspector overrides and scene wiring before changing behavior.

## Scope and schedule

- [x] **Gameplay feature freeze is planned for tonight, Wednesday November 11**. Cut or simplify optional unfinished features instead of assuming Thursday is available for feature development.
- [ ] Finish essential start, end and restart flow; agree whether Play Again starts immediately or returns to a start screen. The proposed UI uses simple temporary buttons/panels, then receives presentation polish.
- [ ] Thursday November 12: art, animation, music, sound, UI and integration. Essential fixes remain in scope.
- [ ] Friday November 13: regression, exported build and release testing. Previously noted jam deadline: Saturday November 14 at 7 AM; timezone not confirmed.

## Completed gameplay milestone

- [x] Opening Fortune generates puzzle/enemy counts and boss attack-frequency modifier. Cleaning selects distinct assigned instances, not types.
- [x] Pressure-plate puzzle, shuffled talisman locations and timed shuffled lantern locations. Play tests confirmed talisman and lantern functionality, including randomized lantern selection. Lantern difficulty was accepted as good enough.
- [x] E is the spiritual ability: selected talismans and lanterns in Cleaning, stun/seal in combat. Talisman interaction currently takes priority when both are nearby.
- [x] Waterfall trigger environmentally stuns regular enemies on entry; per-enemy cooldown; excludes boss. Play testing confirmed it works.
- [x] Encounter reel Bad speed, Neutral heal and Good Peaceful/Offensive effects. Mid-Karma Good now randomly chooses Peaceful or Offensive; the consistency change is approved. Do not assume every boundary and edge case was tested.
- [x] Boss reel Bad 1.5× frequency for 12 seconds, 25% shorter newly started warnings; Neutral +20 HP; Good high Karma 20 damage; Good low Karma 3-second regular stun; mid Karma randomly chooses either. Play tests confirmed Bad, Neutral and high-Karma damage work. Test low/mid and edge cases separately if not already covered.
- [x] Boss charge and fire breath choose randomly after initial charge, maximum two of one type in succession. Play testing found it acceptable. Melee range was debugged via Enemy AI attack range; check actual saved values rather than changing test settings.
- [x] Boss kill/seal resolution and player Game Over states exist in code. Player-death flow has been reported working; complete boss victory paths still require explicit verification.

## Next: playable start, endings and restart

- [ ] Agree and implement minimal Play, Victory/Game Over and Play Again flow, one manageable step at a time. Placeholder buttons may be polished tomorrow.
- [ ] Ensure gameplay timers, input, fortune and reels do not run behind a start screen. Handle scene loading/reset deliberately rather than assuming adding a panel pauses the game.
- [ ] Show correct Victory or Game Over once; verify boss kill and seal, player death, and no unintended transition after either end state.
- [ ] Implement and test restart: fresh HP, Happiness, Karma, fortune, puzzle/encounter selection, boss state, reel timers and temporary effects on repeated runs.
- [ ] Decide whether Play Again immediately starts a run or returns to the menu; no decision recorded yet.

## Essential player-facing feedback and presentation

- [ ] Show controls, phase objectives, Cleaning timer, player HP, Happiness, Karma, Opening Fortune and Lucky Reel numbers/outcomes clearly.
- [ ] Provide a visible readable pressure-plate sequence/clue and progress. The sequence debug log is disabled; Gizmos alone are not enough in a build.
- [ ] Add lantern lit/unlit appearance, timer/countdown and timeout/retry feedback. Current yellow lit Gizmos and Console messages are Editor-only; no continuous countdown UI.
- [ ] Show talisman and lantern interaction cues, remaining progress and completion feedback; ensure talisman locations are reachable.
- [ ] Provide visible regular vs environmental/sealable stun and the boss low-health seal window; green/cyan Gizmos are not build UI.
- [ ] Apply art, animation, music, SFX and UI; confirm gameplay colliders, sorting and timing remain correct after visual replacements.

## Targeted tests and release

- [ ] Confirm GameManager puzzle pool has at least three valid distinct assigned instances, and unlisted puzzle objects are inactive where appropriate. Verify current scene Happiness/Karma settings; prefab defaults are 50, scene overrides may differ.
- [ ] Recheck Good Cleaning auto-completion for each puzzle, final-puzzle transition, Bad time-to-zero, timeout penalties, and completion counted once. Talisman tests worked; confirm exact edge coverage if needed.
- [ ] Confirm lantern random combinations remain reachable within the saved time limit; timeout reuses the chosen locations, and reel completion awards once.
- [ ] Check Encounter high/low/mid Karma boundaries 44/45 and 55/56; selected fate result stays fixed even if a damage kill changes Karma. Test last-enemy reel kill, repeated speed refresh and expiration, and player healing at max health.
- [ ] Check waterfall re-entry/cooldown and dragon exclusion; test pillar stun, low-health seal, kill, Karma changes and one Victory transition.
- [ ] Check Boss speed boost refresh/expiry and warnings, stun/wake-on-hit, preserved pillar stun, mid-Karma lethal damage, random attack streak cap and resumed attacks after pillar stun.
- [ ] Check attack hitboxes for duplicate damage with multiple colliders; preserve combat and input behavior while adding UI.
- [ ] Verify regular enemy navigation and stun/resume after map changes. Navigation assets and rebakes need care; do not delete unfamiliar assets or change geometry without review.
- [ ] Test full runs, both endings, repeat runs, all Fortune tiers, scene transitions and an exported build with no gameplay-blocking errors. Game.unity is currently the only enabled build scene; confirm the eventual menu scene/build order if one is added.

## Deferred / cut-first

Special triple combinations currently only log and use ordinary average classification. Additional puzzle systems, rooms, upgrades and other unapproved features are not required for this freeze. Do not add packages or architecture without approval.
