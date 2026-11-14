using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class OutsideTempleFlow : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private OutsideTempleDialogue dialogue;

    private bool leaving;
    private bool fortunePrepared;
    private int cleaningRoll;
    private int encounterRoll;
    private int bossRoll;

    public bool CanInteract => !leaving && (settingsPanel == null || !settingsPanel.activeInHierarchy);

    public string OpeningFortuneDescription
    {
        get
        {
            EnsureFortunePrepared();
            int puzzles = RunManager.PuzzleCountForRoll(cleaningRoll);
            int enemies = RunManager.EnemyCountForRoll(encounterRoll);
            string bossPacing = BossPacing();
            return $"<b>Your opening Fortune</b>\n" +
                $"Cleaning: <b>{cleaningRoll}</b> - {puzzles} puzzle{(puzzles == 1 ? "" : "s")}\n" +
                $"Encounter: <b>{encounterRoll}</b> - {enemies} spirits\n" +
                $"Dragon: <b>{bossRoll}</b> - {bossPacing} attacks\n\n" +
                "This Fortune sets the challenges for this run. Ready to enter?";
        }
    }

    private void Awake()
    {
        if (dialogue == null)
            dialogue = GetComponent<OutsideTempleDialogue>();

        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    private void Start()
    {
        EnsureFortunePrepared();
    }

    private void EnsureFortunePrepared()
    {
        if (fortunePrepared)
            return;

        RunManager.PrepareFortune(out cleaningRoll, out encounterRoll, out bossRoll);
        fortunePrepared = true;
        Debug.Log($"Opening Fortune: Cleaning {cleaningRoll} ({RunManager.PuzzleCountForRoll(cleaningRoll)} puzzles), Encounter {encounterRoll} ({RunManager.EnemyCountForRoll(encounterRoll)} enemies), Boss {bossRoll} ({BossPacing()} attacks).", this);
    }

    private string BossPacing()
    {
        float multiplier = RunManager.BossSpeedMultiplierForRoll(bossRoll);
        return multiplier > 1f ? "faster" : multiplier < 1f ? "slower" : "normal";
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            OpenSettings();
    }

    public void OpenSettings()
    {
        if (leaving)
            return;

        if (settingsPanel == null)
        {
            Debug.LogWarning("Assign the Settings panel to OutsideTempleFlow.", this);
            return;
        }

        settingsPanel.SetActive(!settingsPanel.activeSelf);
    }

    public void CloseSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    public void ContinueToTemple()
    {
        if (!CanInteract)
            return;

        // Preserve the existing Continue button callback without bypassing the introduction.
        if (dialogue != null && dialogue.isActiveAndEnabled && !dialogue.IsReadyToEnter)
        {
            dialogue.Next();
            return;
        }

        EnsureFortunePrepared();
        leaving = true;
        Time.timeScale = 1f;
        SceneManager.LoadScene("InsideTemple");
    }

    public void ReturnToMainMenu()
    {
        if (leaving)
            return;

        leaving = true;
        Time.timeScale = 1f;
        RunManager.ClearPendingFortune();
        SceneManager.LoadScene("MainMenu");
    }
}