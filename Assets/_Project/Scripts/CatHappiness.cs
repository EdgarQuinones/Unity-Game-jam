using UnityEngine;

public class CatHappiness : MonoBehaviour
{
    [SerializeField] private int startingHappiness = 50;

    public int CurrentHappiness { get; private set; }

    private void Awake()
    {
        CurrentHappiness = startingHappiness;
    }

    public void ModifyHappiness(int amount)
    {
        CurrentHappiness = Mathf.Clamp(
            CurrentHappiness + amount, 0, 100
        );

        Debug.Log($"Cat Happiness: {CurrentHappiness}");
    }
}