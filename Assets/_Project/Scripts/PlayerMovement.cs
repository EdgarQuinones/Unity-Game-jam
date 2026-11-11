using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player Movement")]
    [SerializeField] private float moveSpeed = 5f;
    private Vector2 moveInput;

    // Components
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    // Combat & Actions 
    private Vector2 facingDirection = Vector2.down;
    public Vector2 FacingDirection => facingDirection; // Read only
    private float stunEndTime;
    public bool IsStunned => Time.time < stunEndTime;

    public void ApplyStun(float duration)
    {
        if (duration <= 0f) return;
        stunEndTime = Mathf.Max(stunEndTime, Time.time + duration);
        rb.linearVelocity = Vector2.zero;
        Debug.Log($"{name} stunned for {duration} seconds.", this);
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        //animator = GetComponent<Animator>();
        //spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        if (moveInput.sqrMagnitude > 0.01f)
        {
            facingDirection = moveInput.normalized; 
        } 
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = IsStunned ? Vector2.zero : moveInput.normalized * moveSpeed;
    }

}
