using UnityEngine;

public class Karma : MonoBehaviour
{
    [Header("Karma Settings")]
    [SerializeField] private int startingKarma = 50;

    public int CurrentKarma { get; private set; }

    private void Awake()
    {
        CurrentKarma = Mathf.Clamp(startingKarma, 0, 100);
    }

    public void ModifyKarma(int amount)
    {
        CurrentKarma = Mathf.Clamp(
            CurrentKarma + amount, 0, 100
        );

        Debug.Log($"Karma Debt: {CurrentKarma}");
    }
}