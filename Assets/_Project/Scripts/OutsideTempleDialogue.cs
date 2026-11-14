using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(OutsideTempleFlow))]
public class OutsideTempleDialogue : MonoBehaviour
{
    [Serializable]
    private class DialoguePage
    {
        [TextArea(3, 8)] public string text;
        [Tooltip("Optional. Leave empty to keep the portrait assigned in the scene.")]
        public Sprite portrait;

        public DialoguePage(string text)
        {
            this.text = text;
        }
    }

    [Header("Scene References")]
    [SerializeField] private OutsideTempleFlow flow;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private Image catPortrait;

    [Header("Optional Labels and Buttons")]
    [SerializeField] private TMP_Text speakerText;
    [SerializeField] private TMP_Text pageCounterText;
    [SerializeField] private TMP_Text nextButtonText;
    [SerializeField] private Button backButton;
    [SerializeField] private Button skipButton;

    [Header("Presentation")]
    [SerializeField] private string speakerName = "Lucky Cat";
    [Tooltip("Set to zero to show each page immediately.")]
    [SerializeField, Min(0f)] private float charactersPerSecond = 45f;

    [Header("Introduction Pages (Fortune is added automatically at the end)")]
    [SerializeField] private DialoguePage[] pages =
    {
        new DialoguePage("There you are! This temple needs a little attention. <b>Clean its puzzles, deal with the spirits, and face the dragon</b> waiting within."),
        new DialoguePage("Use <b>WASD or the arrow keys</b> to move. Step on floor plates in order, and press <b>E</b> near talismans and lanterns. Clean before time runs out - and light those lanterns quickly!"),
        new DialoguePage("When the spirits appear, <b>left-click</b> to attack. <b>E</b> stuns ordinary enemies, but your own stun won't let you seal them. Lure them into a <b>waterfall</b>, then get close, face them, and press <b>E</b> to seal instead."),
        new DialoguePage("The dragon won't fall for your ordinary stun. Bait its charge into a <b>pillar</b> to stun it. Defeat it with attacks, or weaken it to <b>very low health</b>, then press <b>E</b> up close while it's stunned to seal it."),
        new DialoguePage("That little cat meter? That's my <b>Happiness</b>. Solve puzzles to improve it; mistakes and unfinished puzzles can lower it. Higher Happiness gives your Lucky Reel <b>better odds</b> - not a guarantee!"),
        new DialoguePage("<b>Sealing lowers Karma; killing raises it.</b> Low Karma favors peaceful good fortune, while high Karma favors offensive good fortune. Near the middle, either can happen. Watch the seven-slot selector!"),
        new DialoguePage("Press <b>Q</b> when your <b>Lucky Reel</b> is ready. It also spins on its own! Fortune can help or hinder you. Watch your ability bars for cooldowns, and press <b>Escape</b> to pause inside the temple.")
    };

    private Sprite defaultPortrait;
    private int pageIndex;
    private int characterCount;
    private float visibleCharacters;
    private bool initialized;

    private int IntroductionPageCount => pages == null ? 0 : pages.Length;
    private bool IsFortunePage => pageIndex == IntroductionPageCount;
    private bool IsRevealing => initialized && dialogueText != null && dialogueText.maxVisibleCharacters < characterCount;
    public bool IsReadyToEnter => initialized && IsFortunePage && !IsRevealing;

    private void Start()
    {
        if (flow == null)
            flow = GetComponent<OutsideTempleFlow>();

        if (flow == null || dialogueText == null)
        {
            Debug.LogError("OutsideTempleDialogue: assign OutsideTempleFlow and a dialogue TextMeshPro text. The introduction cannot start without them.", this);
            return;
        }

        if (catPortrait != null)
        {
            defaultPortrait = catPortrait.sprite;
            catPortrait.raycastTarget = false;
        }

        dialogueText.raycastTarget = false;
        if (speakerText != null)
        {
            speakerText.text = speakerName;
            speakerText.raycastTarget = false;
        }
        if (pageCounterText != null)
            pageCounterText.raycastTarget = false;
        if (nextButtonText != null)
            nextButtonText.raycastTarget = false;

        initialized = true;
        ShowPage(0);
    }

    private void Update()
    {
        if (!CanInteract() || !IsRevealing)
            return;

        if (charactersPerSecond <= 0f)
        {
            RevealPage();
            return;
        }

        visibleCharacters += charactersPerSecond * Time.unscaledDeltaTime;
        dialogueText.maxVisibleCharacters = Mathf.Min(characterCount, Mathf.FloorToInt(visibleCharacters));
        if (!IsRevealing)
            UpdateControls();
    }

    public void Next()
    {
        if (!CanInteract())
            return;

        if (IsRevealing)
        {
            RevealPage();
            return;
        }

        if (IsFortunePage)
            flow.ContinueToTemple();
        else
            ShowPage(pageIndex + 1);
    }

    public void Back()
    {
        if (CanInteract() && pageIndex > 0)
            ShowPage(pageIndex - 1);
    }

    public void SkipToFortune()
    {
        if (!CanInteract())
            return;

        ShowPage(IntroductionPageCount);
        RevealPage();
    }

    private bool CanInteract()
    {
        return initialized && isActiveAndEnabled && flow != null && flow.CanInteract;
    }

    private void ShowPage(int index)
    {
        pageIndex = Mathf.Clamp(index, 0, IntroductionPageCount);
        DialoguePage page = IsFortunePage ? null : pages[pageIndex];
        dialogueText.text = IsFortunePage ? flow.OpeningFortuneDescription : page == null ? string.Empty : page.text;
        dialogueText.maxVisibleCharacters = int.MaxValue;
        dialogueText.ForceMeshUpdate();
        characterCount = dialogueText.textInfo.characterCount;
        visibleCharacters = 0f;
        dialogueText.maxVisibleCharacters = charactersPerSecond > 0f ? 0 : characterCount;

        if (catPortrait != null)
            catPortrait.sprite = page != null && page.portrait != null ? page.portrait : defaultPortrait;

        UpdateControls();
    }

    private void RevealPage()
    {
        visibleCharacters = characterCount;
        dialogueText.maxVisibleCharacters = characterCount;
        UpdateControls();
    }

    private void UpdateControls()
    {
        if (nextButtonText != null)
            nextButtonText.text = IsRevealing ? "Reveal" : IsFortunePage ? "Enter Temple" : "Next";
        if (pageCounterText != null)
            pageCounterText.text = $"{pageIndex + 1} / {IntroductionPageCount + 1}";
        if (backButton != null)
            backButton.interactable = pageIndex > 0;
        if (skipButton != null)
            skipButton.interactable = !IsFortunePage;
    }
}