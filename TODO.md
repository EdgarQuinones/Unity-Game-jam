# Help Lucky Cat — development status

Status as of November 10, 2026. Combines code inspection and the owner's reported Unity tests; implemented does not mean every path has been play-tested. See GAME_DESIGN.md and .clinerules/project.md. Verify current source and scene wiring before changes.

## Latest session: Cleaning and Lucky Reel

- [x] Happiness weights actual 1–9 reel numbers, with all numbers possible; owner reported it seemed to work well.
- [x] Good Encounter rolls apply Karma-dependent effects; Boss good-roll effects remain unimplemented.
- [x] Automatic interval is 20 seconds in the saved Game scene; manual cooldown 12 seconds, minimum shared gap 5 seconds. Recheck complete timing in Play mode.
- [x] Opening Fortune selects 1–3 distinct Cleaning puzzle instances randomly; owner reported testing the separate-puzzle setup.
- [x] Track each selected puzzle once; set 30/50/70-second timers; pressure-plate success awards +10 Happiness, wrong input -1; timeout costs 10 Happiness per unfinished puzzle.
- [x] Each pressure-plate puzzle generates a random permutation of assigned plate IDs, retaining it on retry. The generated-answer log is commented out.
- [x] Implement Cleaning reel effects: Bad -10 seconds, Neutral +10 without cap, Good completes one random unfinished selected puzzle and grants its usual +10 Happiness. Zero time ends Cleaning.
- [ ] Play-test each Cleaning reel effect, automatic completion counting once, Good on the last puzzle and Bad reducing time to zero; verify both transitions to Encounter.
- [ ] Verify saved Game scene starting Happiness is the intended **50**; it was previously observed at 100 and may have changed since.
- [ ] Evaluate pressure-plate trial-and-error readability and balance; sequence is hidden and wrong presses cost 1 Happiness.

CleaningPuzzle is the shared base; future puzzle types implement CompleteFromLuckyReel() and report completion. GameManager selects from its assigned list, or discovers child puzzles when the list is empty. Too few unique instances means the completion target cannot be reached before timeout. The saved scene currently lists four puzzle references; verify their wiring and placement in Unity.

Debug Gizmos: yellow wire squares indicate accepted plate presses; green wire circles indicate completed pressure-plate groups while active. Groups deactivate when Cleaning ends, so the final marker disappears at transition. Enable Gizmos in Unity.

## Encounter and Fire Dragon

The owner identified these as likely priorities, not an irrevocable schedule. Inspect current source, prefabs, scene wiring and physics before implementing.

### Encounter
- [x] Activate 2–4 randomly selected enemies based on Opening Fortune using fixed positions; owner reported testing.
- [x] Kills and seals count once and resolving all selected enemies transitions to Boss; owner reported testing.
- [x] Bad reel: active enemies move at 1.5× speed and turn yellow for 8 seconds; repeated rolls refresh rather than stack, and expiration or disabling restores the modifier and color.
- [x] Neutral reel: heal the player up to 20 HP, capped at maximum; flash green when health is restored.
- [x] Good reel at Karma Debt 56–100: deal 20 damage to each active unresolved enemy.
- [x] Good reel at Karma Debt 45–55: regularly stun all active unresolved enemies for 3 seconds; regular stun does not itself enable sealing.
- [x] Good reel at Karma Debt 0–44: environmentally stun one random active unresolved enemy for 8 seconds, preferring one not already sealable.
- [ ] Verify edge cases if not already covered: a Good damage roll kills the last enemy and starts Boss; a repeated Bad roll refreshes the boost and eventually restores it.
- [ ] Add player-visible sealable-stun feedback for builds; the current green indicator is an Editor Gizmo.

### Fire Dragon and boss resolution
- [ ] Configure approximately 200 HP and activation after Encounter; apply Opening Fortune attack-speed multiplier.
- [ ] Implement Dragon Charge, pillar collision stun, cone Fire Breath, warnings, cooldown and recovery; apply agreed Karma-based boss damage scaling.
- [ ] Allow death at 0 HP; require low HP and waterfall/environmental stun before sealing with E. Verify Karma changes, one-time resolution and Victory.
- [ ] Boss reel effects: Bad temporarily increases boss attack speed; Neutral heals; Good at Karma 50+ damages boss; Good below 50 helps weaken or stun for sealing without bypassing requirements.
- [ ] Restore temporary boss modifiers and prevent indefinite stacking.

## Remaining integration and validation

- [ ] Add a player-facing action log / chat-style text box later to explain reel results and their effects, Karma Debt changes, and how Karma affects enemy damage and good fortune.
- [ ] Apply Opening Fortune boss speed; RunManager calculates it but GameManager does not consume it. Encounter enemy count is applied.
- [ ] Verify Karma starts at 50 and kills increase it and seals decrease it in play. Boss reel effects remain unimplemented.
- [ ] Implement and balance special combinations; triples currently log and use ordinary average tiers.
- [ ] Define transition-state spin restrictions if a transition state is added. Owner reported GameOver/Victory spin blocking and player-death-to-GameOver worked.
- [ ] Complete and test victory, game over, restart, temporary-effect cleanup, full loop and an exported build.

## Friday testing and edge cases — November 13, 2026

Unchecked items are specific checks to confirm, not claims that previously reported tests failed. Test in Play mode with the Console open; record failures and retest fixes.

### Cleaning and reel timing
- [ ] Verify starting Happiness is 50 in the saved Game scene; verify assigned puzzle references are valid and sufficient for all 1–3 puzzle rolls.
- [ ] Check a wrong plate costs 1 Happiness, a completed puzzle grants 10, and timeout costs 10 per unfinished puzzle, clamped to 0–100.
- [ ] Exercise Bad, Neutral and Good Cleaning rolls; verify a Good roll on the final puzzle counts once and transitions to Encounter.
- [ ] Verify both normal timeout and a Bad roll reducing time to zero apply the correct unfinished-puzzle penalty and transition once.
- [ ] Verify manual cooldown, automatic interval and shared minimum spin gap; confirm spins are blocked in Game Over and Victory.

### Encounter and Karma
- [ ] Recheck 2, 3 and 4 selected enemies, fixed positions, and one-time kill/seal counting through the Boss transition.
- [ ] Check a Good Offensive roll killing the final enemy resolves it once and starts Boss without affecting the boss.
- [ ] Check repeated Bad rolls refresh rather than stack speed; confirm speed and color restore on expiry and when an enemy is killed or sealed while boosted.
- [ ] Check Neutral healing near and at maximum HP, green healing flash, and red hit flash during a speed boost.
- [ ] Check Karma boundaries 44/45 and 55/56; at starting 50, one kill leads to Offensive and one seal leads to Peaceful. Verify the selected result remains fixed for the entire roll.
- [ ] Check Peaceful Good targets one active enemy, prefers one not already sealable, and refreshes one if all are already sealable; verify sealing and stun expiry.
- [ ] Verify stun visuals in an exported build; Editor Gizmos alone are not player-facing. Check overlapping attack colliders do not cause unintended repeated hits.

### Boss and full run (after implementation)
- [ ] Test charge, pillar collision, environmental stun, fire breath, warnings, recovery and Opening Fortune boss-speed outcomes.
- [ ] Test boss reel outcomes and temporary-effect cleanup; verify sealing requires the agreed health and environmental conditions.
- [ ] Test death and seal victory paths, player Game Over, restart, repeated runs and a complete exported build. Confirm effects and state reset between runs.

## Planning target

Feature freeze was planned for Thursday, November 12, 2026, Friday for testing and debugging, and jam deadline Saturday, November 14, 2026 at 7 AM (timezone not specified). These are planning targets, not evidence of completion.
