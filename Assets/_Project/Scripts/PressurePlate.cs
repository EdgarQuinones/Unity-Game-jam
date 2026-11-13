using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] private int plateID;
    [SerializeField] private Sprite unpressedSprite;
    [SerializeField] private Sprite pressedSprite;
    public int PlateID => plateID;

    private PressurePlatePuzzle puzzle;
    private SpriteRenderer spriteRenderer;
    private bool isPressed = false;

    private void Awake()
    {
        puzzle = GetComponentInParent<PressurePlatePuzzle>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateSprite();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player") || isPressed)
            return;

        if (puzzle.ActivatePlate(plateID))
        {
            isPressed = true;
            UpdateSprite();
        }
    }

    public void ResetPlate()
    {
        isPressed = false;
        UpdateSprite();
    }

    private void UpdateSprite()
    {
        Sprite sprite = isPressed ? pressedSprite : unpressedSprite;
        if (spriteRenderer != null && sprite != null)
            spriteRenderer.sprite = sprite;
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying || !isPressed)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(0.8f, 0.8f, 0.1f));
    }
}