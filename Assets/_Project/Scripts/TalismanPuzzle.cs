using System.Collections.Generic;
using UnityEngine;

public class TalismanPuzzle : CleaningPuzzle
{
    [SerializeField] private int talismansToSelect = 4;
    [SerializeField] private float cleanseRadius = 1.5f;

    private readonly List<Collider2D> selectedTalismans = new List<Collider2D>();
    private bool isCompleted;
    private GameManager gameManager;
    private CatHappiness catHappiness;

    private void Awake()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        catHappiness = FindFirstObjectByType<CatHappiness>();
    }

    private void OnEnable()
    {
        isCompleted = false;
        selectedTalismans.Clear();

        List<Collider2D> available = new List<Collider2D>();
        foreach (Transform child in transform)
        {
            Collider2D talisman = child.GetComponent<Collider2D>();
            if (talisman == null)
            {
                Debug.LogWarning($"{child.name}: talisman needs a Collider2D.", child);
                continue;
            }

            available.Add(talisman);
            child.gameObject.SetActive(false);
        }

        for (int i = available.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            Collider2D temporary = available[i];
            available[i] = available[randomIndex];
            available[randomIndex] = temporary;
        }

        int count = Mathf.Clamp(talismansToSelect, 0, available.Count);
        if (count == 0 || count < talismansToSelect)
            Debug.LogError($"{name}: requested {talismansToSelect} talismans but found {available.Count}; check the puzzle setup.", this);

        for (int i = 0; i < count; i++)
        {
            selectedTalismans.Add(available[i]);
            available[i].gameObject.SetActive(true);
        }

        Debug.Log($"{name}: selected {count} talismans.", this);
    }

    public bool TryCleanseNearest(Vector2 playerPosition)
    {
        if (isCompleted || gameManager == null || gameManager.CurrentPhase != GameManager.GamePhase.Cleaning)
            return false;

        Collider2D nearest = null;
        float nearestDistance = Mathf.Max(0f, cleanseRadius);
        foreach (Collider2D talisman in selectedTalismans)
        {
            if (talisman == null || !talisman.gameObject.activeInHierarchy || !talisman.enabled)
                continue;

            float distance = Vector2.Distance(playerPosition, talisman.ClosestPoint(playerPosition));
            if (distance <= nearestDistance)
            {
                nearest = talisman;
                nearestDistance = distance;
            }
        }

        if (nearest == null)
            return false;

        selectedTalismans.Remove(nearest);
        nearest.gameObject.SetActive(false);
        Debug.Log($"{name}: cleansed {nearest.name}; {selectedTalismans.Count} remaining.", this);
        if (selectedTalismans.Count == 0)
            CompletePuzzle();
        return true;
    }

    public override void CompleteFromLuckyReel()
    {
        if (isCompleted || gameManager == null || gameManager.CurrentPhase != GameManager.GamePhase.Cleaning)
            return;

        foreach (Collider2D talisman in selectedTalismans)
            if (talisman != null)
                talisman.gameObject.SetActive(false);
        selectedTalismans.Clear();
        Debug.Log($"{name}: completed by Lucky Reel.", this);
        CompletePuzzle();
    }

    private void CompletePuzzle()
    {
        if (isCompleted)
            return;

        isCompleted = true;
        if (catHappiness != null)
            catHappiness.ModifyHappiness(10);
        else
            Debug.LogWarning($"{name}: cannot award Happiness; CatHappiness is missing.", this);
        ReportCompletion();
    }
}