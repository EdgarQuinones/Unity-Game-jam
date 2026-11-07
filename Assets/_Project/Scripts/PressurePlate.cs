using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] private int plateID;

    private PressurePlatePuzzle puzzle;

    private void Awake()
    {
        puzzle = GetComponentInParent<PressurePlatePuzzle>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            puzzle.ActivatePlate(plateID);
        }
    }
}