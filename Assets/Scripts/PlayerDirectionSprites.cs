using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovementPhysics : MonoBehaviour
{
    [Header("�������")]
    public Sprite spriteFront;
    public Sprite spriteLeft;
    public Sprite spriteRight;

    [Header("��������")]
    public float speed = 5f;

    [Header("������")]
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
        rb.gravityScale = 1f;
        rb.freezeRotation = true;
        spriteRenderer.sprite = spriteFront;
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        // �������� ����� ������ ����
        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(
                groundCheck.position, groundCheckRadius, groundLayer);
        }

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
            Debug.Log($"Space! isGrounded={isGrounded}, velY={rb.linearVelocity.y}");

        // ���� �������� ������������� (����) � ����� �����������, �� �� ������� Y
        if (!CanMove)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        // --- �������������� �������� ---
        float moveX = 0f;
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) moveX = -1f;
        else if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) moveX = 1f;

        rb.linearVelocity = new Vector2(moveX * speed, rb.linearVelocity.y);

        // --- ������ ---
        if (Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // --- ������� ---
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