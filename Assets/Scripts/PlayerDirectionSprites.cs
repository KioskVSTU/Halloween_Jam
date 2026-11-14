using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovementPhysics : MonoBehaviour
{
    [Header("Спрайты")]
    public Sprite spriteFront;
    public Sprite spriteLeft;
    public Sprite spriteRight;

    [Header("Движение")]
    public float speed = 2.5f;

    [Header("Прыжок")]
    public float jumpForce = 7f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private bool isGrounded;

    public bool CanMove { get; set; } = true;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 3f;
        rb.freezeRotation = true;
        spriteRenderer.sprite = spriteFront;
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        // Проверка земли (если объект groundCheck назначен)
        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(
                groundCheck.position, groundCheckRadius, groundLayer);
        }

        // Если движение заблокировано
        if (!CanMove)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        // --- Горизонтальное движение ---
        float moveX = 0f;
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) moveX = -1f;
        else if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) moveX = 1f;

        rb.linearVelocity = new Vector2(moveX * speed, rb.linearVelocity.y);

        // --- Прыжок ---
        if (Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // --- Смена спрайтов ---
        if (moveX < 0f) spriteRenderer.sprite = spriteLeft;
        else if (moveX > 0f) spriteRenderer.sprite = spriteRight;
        else spriteRenderer.sprite = spriteFront;
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}