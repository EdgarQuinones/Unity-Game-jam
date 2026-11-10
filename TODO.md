# Help Lucky Cat — development status

Status as of November 10, 2026. Combines code inspection and the owner's reported Unity tests; implemented does not mean every path has been play-tested. See GAME_DESIGN.md and .clinerules/project.md. Verify current source and scene wiring before changes.

## Latest session: Cleaning and Lucky Reel

- [x] Happiness weights actual 1–9 reel numbers, with all numbers possible; owner reported it seemed to work well.
- [x] Good Encounter/Boss rolls log Offensive at Karma 50+ or Peaceful below 50; actual combat effects remain unimplemented.
- [x] Automatic interval is 20 seconds in the saved Game scene; manual cooldown 12 seconds, minimum shared gap 5 seconds. Recheck complete timing in Play mode.
- [x] Opening Fortune selects 1–3 distinct Cleaning puzzle instances randomly; owner reported testing the separate-puzzle setup.
- [x] Track each selected puzzle once; set 30/50/70-second timers; pressure-plate success awards +10 Happiness, wrong input -5.
- [x] Each pressure-plate puzzle generates a random permutation of assigned plate IDs, retaining it on retry. The generated-answer log is commented out.
- [x] Implement Cleaning reel effects: Bad -10 seconds, Neutral +10 without cap, Good completes one random unfinished selected puzzle and grants its usual +10 Happiness. Zero time ends Cleaning.
- [ ] Play-test each Cleaning reel effect, automatic completion counting once, Good on the last puzzle and Bad reducing time to zero; verify both transitions to Encounter.
- [ ] Verify saved Game scene starting Happiness: currently **100**, whereas intended default is **50**. This may be a testing setting; ask before changing it.
- [ ] Decide whether to show the randomized pressure-plate sequence to the player or intentionally use trial and error; the pressed animation/Gizmo does not reveal the answer.

CleaningPuzzle is the shared base; future puzzle types implement CompleteFromLuckyReel() and report completion. GameManager selects from its assigned list, or discovers child puzzles when the list is empty. Too few unique instances means the completion target cannot be reached before timeout. The saved scene currently lists four puzzle references; verify their wiring and placement in Unity.

Debug Gizmos: yellow wire squares indicate accepted plate presses; green wire circles indicate completed pressure-plate groups while active. Groups deactivate when Cleaning ends, so the final marker disappears at transition. Enable Gizmos in Unity.

## Likely next session: Encounter and Fire Dragon

The owner identified these as likely priorities, not an irrevocable schedule. Inspect current source, prefabs, scene wiring and physics before implementing.

### Encounter
- [ ] Activate 2–4 enemies based on Opening Fortune using fixed spawn positions.
- [ ] Verify kills and seals count exactly once and all resolved enemies transition to Boss.
- [ ] Implement reel effects: Bad temporarily increases enemy movement speed; Neutral heals player; Good at Karma 50+ damages enemies; Good below 50 environmentally stuns enemies.
- [ ] Restore temporary modifiers correctly and prevent indefinite stacking.

### Fire Dragon and boss resolution
- [ ] Configure approximately 200 HP and activation after Encounter; apply Opening Fortune attack-speed multiplier.
- [ ] Implement Dragon Charge, pillar collision stun, cone Fire Breath, warnings, cooldown and recovery; apply agreed Karma-based boss damage scaling.
- [ ] Allow death at 0 HP; require low HP and waterfall/environmental stun before sealing with E. Verify Karma changes, one-time resolution and Victory.
- [ ] Boss reel effects: Bad temporarily increases boss attack speed; Neutral heals; Good at Karma 50+ damages boss; Good below 50 helps weaken or stun for sealing without bypassing requirements.
- [ ] Restore temporary boss modifiers and prevent indefinite stacking.

## Remaining integration and validation

- [ ] Apply Opening Fortune enemy count and boss speed; RunManager calculates them but GameManager does not consume them.
- [ ] Verify Karma starts at 50 and kills increase it and seals decrease it in play. Reel combat Karma selection currently logs only.
- [ ] Implement and balance special combinations; triples currently log and use ordinary average tiers.
- [ ] Define transition-state spin restrictions if a transition state is added. Owner reported GameOver/Victory spin blocking and player-death-to-GameOver worked.
- [ ] Complete and test victory, game over, restart, temporary-effect cleanup, full loop and an exported build.

## Planning target

Feature freeze was planned for Thursday, November 12, 2026, Friday for testing and debugging, and jam deadline Saturday, November 14, 2026 at 7 AM (timezone not specified). These are planning targets, not evidence of completion.
