using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Scene-local presentation only. GameManager creates this without changing saved scene wiring.
public class GameActionLog : MonoBehaviour
{
    private const int EntryLimit = 5;
    private readonly List<string> entries = new List<string>();
    private readonly TMP_Text[] entryLabels = new TMP_Text[EntryLimit];
    private GameObject panel;
    private TMP_Text reelStatus;
    private LuckyReel reel;
    private GameManager manager;
    private string lastStatus;

    private void Start()
    {
        manager = GetComponent<GameManager>();
        reel = GetComponent<LuckyReel>();

        RunStatusUI hud = FindFirstObjectByType<RunStatusUI>();
        Canvas canvas = hud != null ? hud.GetComponentInParent<Canvas>() : FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            Debug.LogWarning("GameActionLog: no Canvas found; action log cannot be displayed.", this);
            return;
        }

        TMP_Text existingText = canvas.GetComponentInChildren<TMP_Text>(true);
        TMP_FontAsset font = existingText != null ? existingText.font : TMP_Settings.defaultFontAsset;
        panel = new GameObject("ActionLog", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        panel.layer = canvas.gameObject.layer;
        RectTransform rect = panel.GetComponent<RectTransform>();
        rect.SetParent(canvas.transform, false);
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(1f, 0f);
        rect.anchoredPosition = new Vector2(-24f, 24f);
        rect.sizeDelta = new Vector2(640f, 420f);
        // All existing HUD and modal siblings retain their relative ordering and draw above the log.
        rect.SetAsFirstSibling();
        Image background = panel.GetComponent<Image>();
        background.color = new Color(0.035f, 0.04f, 0.075f, 0.84f);
        background.raycastTarget = false;

        CreateLabel("Title", font, 26f, 12f, 34f).text = "RECENT EVENTS";
        reelStatus = CreateLabel("ReelStatus", font, 20f, 48f, 30f);
        reelStatus.color = new Color(1f, 0.85f, 0.5f);
        for (int i = 0; i < EntryLimit; i++)
        {
            entryLabels[i] = CreateLabel("Entry" + (i + 1), font, 20f, 86f + i * 64f, 62f);
            entryLabels[i].color = i == 0 ? Color.white : new Color(0.8f, 0.8f, 0.85f);
        }
        RefreshEntries();
    }

    private TMP_Text CreateLabel(string labelName, TMP_FontAsset font, float size, float top, float height)
    {
        GameObject label = new GameObject(labelName, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        label.layer = panel.layer;
        RectTransform rect = label.GetComponent<RectTransform>();
        rect.SetParent(panel.transform, false);
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.anchoredPosition = new Vector2(0f, -top);
        rect.sizeDelta = new Vector2(-32f, height);
        TMP_Text text = label.GetComponent<TMP_Text>();
        text.font = font;
        text.fontSize = size;
        text.color = new Color(1f, 0.95f, 0.8f);
        text.alignment = TextAlignmentOptions.TopLeft;
        text.textWrappingMode = TextWrappingModes.Normal;
        text.overflowMode = TextOverflowModes.Ellipsis;
        text.richText = false;
        text.raycastTarget = false;
        return text;
    }

    private void Update()
    {
        if (reelStatus == null || reel == null || manager == null)
            return;

        string status;
        if (manager.CurrentPhase == GameManager.GamePhase.Complete || manager.CurrentPhase == GameManager.GamePhase.GameOver)
            status = "Run finished";
        else if (PauseManager.IsPaused)
            status = "Reel paused";
        else
        {
            int seconds = Mathf.CeilToInt(reel.ManualSpinSecondsRemaining);
            string manual = seconds > 0 ? $"Q: ready in {seconds}s" : "Q: REEL READY";
            status = $"{manual}    |    Auto: {Mathf.CeilToInt(reel.AutomaticSpinSecondsRemaining)}s";
        }

        if (status != lastStatus)
        {
            reelStatus.text = status;
            lastStatus = status;
        }
    }

    public void Record(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            return;

        entries.Insert(0, message);
        if (entries.Count > EntryLimit)
            entries.RemoveAt(EntryLimit);
        RefreshEntries();
    }

    private void RefreshEntries()
    {
        for (int i = 0; i < entryLabels.Length; i++)
        {
            if (entryLabels[i] != null)
                entryLabels[i].text = i < entries.Count ? entries[i] : string.Empty;
        }
    }

    private void OnDestroy()
    {
        if (panel != null)
            Destroy(panel);
    }
}