using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum GamePhase
    {
        Cleaning,
        Encounter,
        Boss,
        Complete,
        GameOver
    }

    [Header("Cleaning Phase")]
    [SerializeField] private CleaningPuzzle[] puzzleGroups;

    [Header("Encounter Phase")]
    [SerializeField] private EnemyAI[] encounterEnemies;
    [SerializeField] private Health playerHealth;
    private int resolvedEnemies = 0;
    private HashSet<EnemyAI> resolvedEnemySet = new HashSet<EnemyAI>();
    private readonly HashSet<EnemyAI> selectedEncounterEnemies = new HashSet<EnemyAI>();

    [Header("Boss Phase")]
    [SerializeField] private EnemyAI boss;

    [Header("Ending Panels")]
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject defeatPanel;

    [SerializeField] private int totalPuzzles;
    private int completedPuzzles = 0;
    private readonly List<CleaningPuzzle> cleaningPuzzles = new List<CleaningPuzzle>();
    private readonly HashSet<CleaningPuzzle> completedPuzzleSet = new HashSet<CleaningPuzzle>();

    [Header("Misc")]
    [SerializeField] private float timeRemaining;
    private GamePhase currentPhase;
    public GamePhase CurrentPhase => currentPhase;
    public float CleaningTimeRemaining => Mathf.Max(0f, timeRemaining);
    public int CompletedCleaningPuzzles => completedPuzzles;
    public int TotalCleaningPuzzles => totalPuzzles;
    public Health BossHealth => boss != null ? boss.GetComponent<Health>() : null;

    private RunManager runManager;
    private bool leaving;
    private GameActionLog actionLog;
    internal string ReelEffect { get; private set; }

    internal void BeginReelReport()
    {
        ReelEffect = "No effect applied (required target or component unavailable).";
    }

    public void LogAction(string message)
    {
        if (actionLog != null)
            actionLog.Record(message);
    }

    private void Awake()
    {
        actionLog = GetComponent<GameActionLog>();
        if (actionLog == null)
            actionLog = gameObject.AddComponent<GameActionLog>();

        if (victoryPanel != null)
            victoryPanel.SetActive(false);
        if (defeatPanel != null)
            defeatPanel.SetActive(false);
    }

    private void Start()
    {
        runManager = FindFirstObjectByType<RunManager>();

        totalPuzzles = runManager.PuzzleCount;

        foreach (EnemyAI enemy in encounterEnemies)
        {
            if (enemy != null && enemy != boss)
                enemy.gameObject.SetActive(false);
        }

        StartCleaningPhase();
    }

    private void Update()
    {
        if (currentPhase != GamePhase.Cleaning)
            return;

        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            EndCleaningPhase();
        }
    }

    private void StartCleaningPhase()
    {
        currentPhase = GamePhase.Cleaning;
        timeRemaining = 10f + totalPuzzles * 20f;

        if (puzzleGroups == null || puzzleGroups.Length == 0)
            puzzleGroups = GetComponentsInChildren<CleaningPuzzle>(true);

        List<CleaningPuzzle> availablePuzzles = new List<CleaningPuzzle>();
        foreach (CleaningPuzzle puzzle in puzzleGroups)
        {
            if (puzzle == null)
            {
                Debug.LogError("Cleaning puzzle list contains a missing reference.", this);
                continue;
            }

            if (availablePuzzles.Contains(puzzle))
            {
                Debug.LogWarning($"Duplicate cleaning puzzle ignored: {puzzle.name}.", puzzle);
                continue;
            }

            availablePuzzles.Add(puzzle);
            puzzle.gameObject.SetActive(false);
        }

        for (int i = availablePuzzles.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            CleaningPuzzle temporary = availablePuzzles[i];
            availablePuzzles[i] = availablePuzzles[randomIndex];
            availablePuzzles[randomIndex] = temporary;
        }

        int selectedCount = Mathf.Min(totalPuzzles, availablePuzzles.Count);
        for (int i = 0; i < selectedCount; i++)
        {
            CleaningPuzzle puzzle = availablePuzzles[i];
            cleaningPuzzles.Add(puzzle);
            puzzle.gameObject.SetActive(true);
            //Debug.Log($"Randomly selected cleaning puzzle: {puzzle.name}.", puzzle);
        }

        if (availablePuzzles.Count < totalPuzzles)
            Debug.LogError($"Cleaning requires {totalPuzzles} puzzles but only {availablePuzzles.Count} unique puzzles are assigned; completion is not possible before timeout.", this);

        Debug.Log($"Cleaning Phase Started! Required puzzles: {totalPuzzles}; time: {timeRemaining} seconds.");
        LogAction($"CLEANING — {totalPuzzles} puzzle(s), {timeRemaining:0}s.\nStep on plates; E cleans talismans / lights lanterns.");
    }

    public void EndCleaningPhase()
    {
        if (currentPhase != GamePhase.Cleaning)
            return;

        if (timeRemaining <= 0f)
        {
            int unfinishedPuzzles = Mathf.Max(0, totalPuzzles - completedPuzzles);
            if (unfinishedPuzzles > 0)
            {
                CatHappiness catHappiness = FindFirstObjectByType<CatHappiness>();
                if (catHappiness != null)
                {
                    catHappiness.ModifyHappiness(-10 * unfinishedPuzzles);
                    LogAction($"CLEANING TIME UP — {unfinishedPuzzles} puzzle(s) unfinished.\nHappiness penalty: {-10 * unfinishedPuzzles} (minimum 0).");
                    Debug.Log($"Cleaning timed out with {unfinishedPuzzles} unfinished puzzles; Happiness penalty: {-10 * unfinishedPuzzles}.");
                }
                else
                {
                    Debug.LogError("Cannot apply Cleaning timeout penalty: CatHappiness not found.", this);
                }
            }
        }

        currentPhase = GamePhase.Encounter;

        foreach (CleaningPuzzle puzzle in cleaningPuzzles)
        {
            if (puzzle != null)
                puzzle.gameObject.SetActive(false);
        }

        Debug.Log("Cleaning Phase Ended!");

        resolvedEnemies = 0;
        resolvedEnemySet.Clear();
        selectedEncounterEnemies.Clear();

        List<EnemyAI> availableEnemies = new List<EnemyAI>();
        foreach (EnemyAI enemy in encounterEnemies)
        {
            if (enemy == null || enemy == boss || enemy.IsBoss)
            {
                Debug.LogError("Encounter enemy list contains a missing reference or a boss.", this);
                continue;
            }

            if (availableEnemies.Contains(enemy))
            {
                Debug.LogWarning($"Duplicate encounter enemy ignored: {enemy.name}.", enemy);
                continue;
            }

            availableEnemies.Add(enemy);
        }

        for (int i = availableEnemies.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            EnemyAI temporary = availableEnemies[i];
            availableEnemies[i] = availableEnemies[randomIndex];
            availableEnemies[randomIndex] = temporary;
        }

        int selectedCount = Mathf.Min(runManager.EnemyCount, availableEnemies.Count);
        if (selectedCount < runManager.EnemyCount)
            Debug.LogError($"Encounter requires {runManager.EnemyCount} enemies but only {availableEnemies.Count} unique valid enemies are assigned.", this);

        for (int i = 0; i < selectedCount; i++)
            selectedEncounterEnemies.Add(availableEnemies[i]);

        //Debug.Log($"Encounter roll: {runManager.EncounterRoll}; selected {selectedCount} enemies.");

        if (selectedCount == 0)
        {
            Debug.LogError("No valid encounter enemies are assigned; cannot start the encounter.", this);
            return;
        }

        foreach (EnemyAI enemy in selectedEncounterEnemies)
        {
            Debug.Log($"Encounter enemy activated: {enemy.name}.", enemy);
            enemy.gameObject.SetActive(true);
        }
        LogAction($"ENCOUNTER — {selectedEncounterEnemies.Count} spirits.\nAttack, or lure into waterfalls and press E to seal.");
    }

    public void BoostBossAttackSpeed()
    {
        if (currentPhase != GamePhase.Boss)
            return;

        if (boss == null || !boss.gameObject.activeInHierarchy)
        {
            Debug.LogWarning("Cannot boost boss attack speed: boss is missing or inactive.", this);
            return;
        }

        DragonAttackCoordinator coordinator = boss.GetComponent<DragonAttackCoordinator>();
        if (coordinator != null)
        {
            coordinator.ApplyAttackSpeedBoost(1.5f, 12f);
            ReelEffect = "Dragon attacks 1.5x faster for 12s; new warnings shorter.";
        }
        else
            Debug.LogWarning("Cannot boost boss attack speed: DragonAttackCoordinator is missing.", boss);
    }

    public void DamageBossFromReel()
    {
        if (currentPhase != GamePhase.Boss)
            return;

        if (boss == null || !boss.gameObject.activeInHierarchy)
        {
            Debug.LogWarning("Cannot damage boss: boss is missing or inactive.", this);
            return;
        }

        Health bossHealth = boss.GetComponent<Health>();
        if (bossHealth == null)
        {
            Debug.LogWarning("Cannot damage boss: Health is missing.", boss);
            return;
        }

        Debug.Log("Good Boss fortune deals 20 damage to the boss.", boss);
        float healthBefore = bossHealth.CurrentHealth;
        bossHealth.TakeDamage(20f);
        ReelEffect = $"Offensive: Tianlong took {healthBefore - bossHealth.CurrentHealth:0.##} damage.";
    }

    public void StunBossFromReel()
    {
        if (currentPhase != GamePhase.Boss)
            return;

        if (boss == null || !boss.gameObject.activeInHierarchy)
        {
            Debug.LogWarning("Cannot stun boss: boss is missing or inactive.", this);
            return;
        }

        bool preservePillarStun = boss.IsEnvironmentallyStunned;
        boss.Stun(3f, EnemyAI.StunType.Player);
        ReelEffect = preservePillarStun
            ? "Peaceful: existing pillar stun preserved."
            : "Peaceful: Tianlong stunned for at least 3s (not a seal stun).";
        Debug.Log("Good Boss fortune applied a regular stun; an existing environmental stun is preserved.", boss);
    }

    public void HealPlayerFromCombatReel()
    {
        if (currentPhase != GamePhase.Encounter && currentPhase != GamePhase.Boss)
            return;

        if (playerHealth == null)
        {
            Debug.LogWarning("Cannot heal player: Player Health is not assigned.", this);
            return;
        }

        float healthBefore = playerHealth.CurrentHealth;
        playerHealth.Heal(20f);
        float healed = playerHealth.CurrentHealth - healthBefore;
        ReelEffect = healed > 0f
            ? $"Healed {healed:0.##} HP ({playerHealth.CurrentHealth:0.##}/{playerHealth.MaxHealth:0.##})."
            : "Healed 0 HP (already full or unable to heal).";
        Debug.Log($"{currentPhase} reel healed player by {playerHealth.CurrentHealth - healthBefore:0.##}; health: {playerHealth.CurrentHealth:0.##}/{playerHealth.MaxHealth:0.##}.", playerHealth);
    }

    public void BoostEncounterEnemySpeed()
    {
        if (currentPhase != GamePhase.Encounter)
            return;

        int boostedCount = 0;
        foreach (EnemyAI enemy in selectedEncounterEnemies)
        {
            if (enemy == null || !enemy.gameObject.activeInHierarchy || resolvedEnemySet.Contains(enemy))
                continue;

            EnemyMovement movement = enemy.GetComponent<EnemyMovement>();
            if (movement != null)
            {
                movement.ApplySpeedBoost(1.5f, 8f);
                boostedCount++;
            }
            else
                Debug.LogWarning($"{enemy.name} cannot receive the speed boost: EnemyMovement is missing.", enemy);
        }
        ReelEffect = boostedCount > 0
            ? $"{boostedCount} spirit(s) move 1.5x faster for 8s (refreshed)."
            : "No active spirits could receive a speed boost.";
    }

    public void ApplyGoodEncounterFortune(int karmaDebt)
    {
        if (currentPhase != GamePhase.Encounter)
            return;

        bool peaceful = karmaDebt < 45 || (karmaDebt <= 55 && Random.value < 0.5f);
        string fortuneType = peaceful ? "Peaceful" : "Offensive";
        Debug.Log($"Karma Debt: {karmaDebt} -> {(karmaDebt >= 45 && karmaDebt <= 55 ? "Fate chose " : "")}{fortuneType} Encounter fortune.");

        if (peaceful)
        {
            List<EnemyAI> eligibleEnemies = new List<EnemyAI>();
            List<EnemyAI> alreadyStunnedEnemies = new List<EnemyAI>();
            foreach (EnemyAI enemy in selectedEncounterEnemies)
            {
                if (enemy == null || !enemy.gameObject.activeInHierarchy || resolvedEnemySet.Contains(enemy))
                    continue;

                if (enemy.CanBeSealed)
                    alreadyStunnedEnemies.Add(enemy);
                else
                    eligibleEnemies.Add(enemy);
            }

            if (eligibleEnemies.Count == 0)
                eligibleEnemies = alreadyStunnedEnemies;

            if (eligibleEnemies.Count == 0)
            {
                Debug.LogWarning("Good Encounter fortune found no active enemies to stun.", this);
                ReelEffect = "Peaceful: no active spirits to stun.";
                return;
            }

            EnemyAI target = eligibleEnemies[Random.Range(0, eligibleEnemies.Count)];
            target.Stun(8f, EnemyAI.StunType.Environmental);
            ReelEffect = "Peaceful: spirit stunned 8s+; face it, E to seal.";
            Debug.Log($"Good Encounter fortune environmentally stuns {target.name} for at least 8 seconds.", target);
            return;
        }

        int damagedCount = 0;
        float totalDamage = 0f;
        foreach (EnemyAI enemy in selectedEncounterEnemies)
        {
            if (enemy == null || !enemy.gameObject.activeInHierarchy || resolvedEnemySet.Contains(enemy))
                continue;

            Health enemyHealth = enemy.GetComponent<Health>();
            if (enemyHealth == null)
            {
                Debug.LogWarning($"{enemy.name} cannot receive reel damage: Health is missing.", enemy);
                continue;
            }

            Debug.Log($"Good Encounter fortune deals 20 damage to {enemy.name}.", enemy);
            float healthBefore = enemyHealth.CurrentHealth;
            enemyHealth.TakeDamage(20f);
            float damage = healthBefore - enemyHealth.CurrentHealth;
            if (damage > 0f)
            {
                damagedCount++;
                totalDamage += damage;
            }
        }
        ReelEffect = $"Offensive: {totalDamage:0.##} total damage to {damagedCount} spirit(s).";
    }

    public void EnemyResolved(EnemyAI enemy)
    {
        if (currentPhase != GamePhase.Encounter)
            return;

        if (enemy == null || !selectedEncounterEnemies.Contains(enemy) || !resolvedEnemySet.Add(enemy))
            return;

        resolvedEnemies++;

        Debug.Log($"Enemies resolved: {resolvedEnemies}/{selectedEncounterEnemies.Count}");
        LogAction($"SPIRIT RESOLVED — {resolvedEnemies} / {selectedEncounterEnemies.Count}");

        if (resolvedEnemies >= selectedEncounterEnemies.Count)
        {
            EndEncounterPhase();
        }
    }

    private void EndEncounterPhase()
    {
        if (currentPhase != GamePhase.Encounter)
            return;

        currentPhase = GamePhase.Boss;

        //Debug.Log("Encounter Complete!");
        //Debug.Log("Boss Phase Started!");

        if (boss != null)
            boss.gameObject.SetActive(true);
        LogAction("BOSS — Tianlong awakened.\nBait a charge into a pillar; weaken it before sealing.");
    }

    public void AdjustCleaningTime(float seconds)
    {
        if (currentPhase != GamePhase.Cleaning)
            return;

        float timeBefore = timeRemaining;
        timeRemaining = Mathf.Max(0f, timeRemaining + seconds);
        ReelEffect = $"Cleaning time {timeRemaining - timeBefore:+0.##;-0.##;0}s ({timeRemaining:0.##}s left).";
        //Debug.Log($"Cleaning time adjusted by {seconds:+0.##;-0.##;0} seconds; remaining: {timeRemaining:F1} seconds.");

        if (timeRemaining <= 0f)
        {
            //Debug.Log("Cleaning time ran out due to a Lucky Reel outcome.");
            EndCleaningPhase();
        }
    }

    public void CompleteRandomCleaningPuzzle()
    {
        if (currentPhase != GamePhase.Cleaning)
            return;

        List<CleaningPuzzle> unfinished = new List<CleaningPuzzle>();
        foreach (CleaningPuzzle puzzle in cleaningPuzzles)
        {
            if (puzzle != null && !completedPuzzleSet.Contains(puzzle))
                unfinished.Add(puzzle);
        }

        if (unfinished.Count == 0)
        {
            Debug.LogWarning("Lucky Reel could not complete a puzzle: no unfinished selected puzzles remain.", this);
            ReelEffect = "No unfinished puzzle available to complete.";
            return;
        }

        CleaningPuzzle selected = unfinished[Random.Range(0, unfinished.Count)];
        Debug.Log($"Lucky Reel selected {selected.name} for automatic completion.", selected);
        int completedBefore = completedPuzzles;
        selected.CompleteFromLuckyReel();
        ReelEffect = completedPuzzles > completedBefore
            ? "One cleaning puzzle completed automatically."
            : "Selected puzzle did not report a new completion.";
    }

    public void PuzzleCompleted(CleaningPuzzle puzzle)
    {
        if (currentPhase != GamePhase.Cleaning)
            return;

        if (puzzle == null || !cleaningPuzzles.Contains(puzzle) || !completedPuzzleSet.Add(puzzle))
        {
            Debug.LogWarning("Ignored completion from a missing, unselected, or already completed cleaning puzzle.", this);
            return;
        }

        completedPuzzles++;
        LogAction($"PUZZLE COMPLETE — {completedPuzzles} / {totalPuzzles}");

        //Debug.Log($"Completed: {completedPuzzles}/{totalPuzzles}");

        if (completedPuzzles >= totalPuzzles)
        {
            EndCleaningPhase();
        }
    }

    public void BossResolved()
    {
        if (currentPhase != GamePhase.Boss)
            return;

        currentPhase = GamePhase.Complete;
        LogAction("VICTORY — Tianlong resolved. Temple cleansed!");

        //Debug.Log("Boss Defeated!");
        //Debug.Log("Level Complete!");

        ShowEndingPanel(victoryPanel);
    }

    public void GameOver()
    {
        if (currentPhase == GamePhase.GameOver ||
            currentPhase == GamePhase.Complete)
            return;

        currentPhase = GamePhase.GameOver;
        LogAction("DEFEAT — the run has ended. Try a new Fortune.");

        //Debug.Log("GAME OVER!");
        ShowEndingPanel(defeatPanel);
    }

    private void ShowEndingPanel(GameObject panel)
    {
        PauseManager pauseManager = FindFirstObjectByType<PauseManager>();
        if (pauseManager != null)
            pauseManager.EndRun();

        Time.timeScale = 0f;

        if (panel == null)
        {
            Debug.LogError("Assign both Victory and Defeat panels to GameManager.", this);
            return;
        }

        panel.SetActive(true);
        panel.transform.SetAsLastSibling();
    }

    public void PlayAgain()
    {
        LeaveRun("OutsideTemple");
    }

    public void ReturnToMainMenu()
    {
        LeaveRun("MainMenu");
    }

    private void LeaveRun(string sceneName)
    {
        if (leaving)
            return;

        leaving = true;
        Time.timeScale = 1f;
        RunManager.ClearPendingFortune();
        SceneManager.LoadScene(sceneName);
    }

}