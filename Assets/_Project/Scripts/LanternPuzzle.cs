using System.Collections.Generic;
using UnityEngine;

public class LanternPuzzle : CleaningPuzzle
{
    [SerializeField] private float timeLimit = 10f;
    [SerializeField] private float interactionRadius = 1.5f;
    [SerializeField] private int lanternsToSelect = 4;
    [SerializeField] private Sprite unlitSprite;
    [SerializeField] private Sprite litSprite;

    private readonly List<Collider2D> lanterns = new List<Collider2D>();
    private readonly List<Collider2D> selectedLanterns = new List<Collider2D>();
    private readonly HashSet<Collider2D> litLanterns = new HashSet<Collider2D>();
    private GameManager gameManager;
    private CatHappiness catHappiness;
    private float deadline;
    private bool timerRunning;
    private bool isCompleted;

    private void Awake()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        catHappiness = FindFirstObjectByType<CatHappiness>();
        foreach (Transform child in transform)
        {
            Collider2D lantern = child.GetComponent<Collider2D>();
            if (lantern != null)
                lanterns.Add(lantern);
            else
                Debug.LogError($"{child.name}: lantern needs a Collider2D.", child);
        }

        if (lanterns.Count == 0)
            Debug.LogError($"{name}: no lantern colliders found.", this);
    }

    private void OnEnable()
    {
        isCompleted = false;
        timerRunning = false;
        litLanterns.Clear();
        selectedLanterns.Clear();

        List<Collider2D> available = new List<Collider2D>();
        foreach (Collider2D lantern in lanterns)
        {
            if (lantern == null)
                continue;
            available.Add(lantern);
            lantern.gameObject.SetActive(false);
        }

        for (int i = available.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            Collider2D temporary = available[i];
            available[i] = available[randomIndex];
            available[randomIndex] = temporary;
        }

        int count = Mathf.Clamp(lanternsToSelect, 0, available.Count);
        if (count == 0 || count < lanternsToSelect)
            Debug.LogError($"{name}: requested {lanternsToSelect} lanterns but found {available.Count}; check the puzzle setup.", this);

        for (int i = 0; i < count; i++)
        {
            selectedLanterns.Add(available[i]);
            SetLanternSprite(available[i], unlitSprite);
            available[i].gameObject.SetActive(true);
        }

        Debug.Log($"{name}: selected {count} lanterns.", this);
    }

    private void Update()
    {
        if (timerRunning && Time.time >= deadline)
            ResetLanterns();
    }

    public bool TryLightNearest(Vector2 playerPosition)
    {
        if (isCompleted || gameManager == null || gameManager.CurrentPhase != GameManager.GamePhase.Cleaning)
            return false;

        if (timerRunning && Time.time >= deadline)
            ResetLanterns();

        if (timeLimit <= 0f)
        {
            Debug.LogWarning($"{name}: time limit must be greater than zero.", this);
            return false;
        }

        Collider2D nearest = null;
        float nearestDistance = Mathf.Max(0f, interactionRadius);
        foreach (Collider2D lantern in selectedLanterns)
        {
            if (lantern == null || !lantern.enabled || !lantern.gameObject.activeInHierarchy || litLanterns.Contains(lantern))
                continue;

            float distance = Vector2.Distance(playerPosition, lantern.ClosestPoint(playerPosition));
            if (distance <= nearestDistance)
            {
                nearest = lantern;
                nearestDistance = distance;
            }
        }

        if (nearest == null)
            return false;

        if (!timerRunning)
        {
            timerRunning = true;
            deadline = Time.time + timeLimit;
            Debug.Log($"{name}: lantern timer started: {timeLimit} seconds.", this);
        }

        litLanterns.Add(nearest);
        SetLanternSprite(nearest, litSprite);
        Debug.Log($"{name}: lit {nearest.name}; {litLanterns.Count}/{selectedLanterns.Count}; {Mathf.Max(0f, deadline - Time.time):F1} seconds remaining.", this);
        if (litLanterns.Count == selectedLanterns.Count)
            CompletePuzzle();
        return true;
    }

    private void ResetLanterns()
    {
        timerRunning = false;
        litLanterns.Clear();
        foreach (Collider2D lantern in selectedLanterns)
            SetLanternSprite(lantern, unlitSprite);
        Debug.Log($"{name}: time expired; lanterns reset. Try again.", this);
    }

    public override void CompleteFromLuckyReel()
    {
        if (isCompleted || gameManager == null || gameManager.CurrentPhase != GameManager.GamePhase.Cleaning)
            return;

        foreach (Collider2D lantern in selectedLanterns)
            if (lantern != null)
            {
                litLanterns.Add(lantern);
                SetLanternSprite(lantern, litSprite);
            }
        Debug.Log($"{name}: completed by Lucky Reel.", this);
        CompletePuzzle();
    }

    private void SetLanternSprite(Collider2D lantern, Sprite sprite)
    {
        if (lantern == null || sprite == null)
            return;

        SpriteRenderer spriteRenderer = lantern.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            spriteRenderer.sprite = sprite;
    }

    private void CompletePuzzle()
    {
        if (isCompleted)
            return;

        isCompleted = true;
        timerRunning = false;
        if (catHappiness != null)
            catHappiness.ModifyHappiness(10);
        else
            Debug.LogWarning($"{name}: cannot award Happiness; CatHappiness is missing.", this);
        ReportCompletion();
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;

        Gizmos.color = Color.yellow;
        foreach (Collider2D lantern in litLanterns)
            if (lantern != null)
                Gizmos.DrawWireSphere(lantern.transform.position, 0.65f);
    }
}