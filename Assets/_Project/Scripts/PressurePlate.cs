using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] private int plateID;
    public int PlateID => plateID;

    private PressurePlatePuzzle puzzle;
    private bool isPressed = false;

    private void Awake()
    {
        puzzle = GetComponentInParent<PressurePlatePuzzle>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player") || isPressed)
            return;

        if (puzzle.ActivatePlate(plateID))
        {
            isPressed = true;
        }
    }

    public void ResetPlate()
    {
        isPressed = false;
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying || !isPressed)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(0.8f, 0.8f, 0.1f));
    }
}