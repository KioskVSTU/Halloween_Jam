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

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        // Отключаем гравитацию и вращение — персонаж не должен падать и крутиться
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        spriteRenderer.sprite = spriteFront;
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        float moveX = 0f;
        float moveY = 0f;

        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) moveX = -1f;
        else if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) moveX = 1f;


        // Движение через физику
        Vector2 direction = new Vector2(moveX, moveY).normalized;
        rb.linearVelocity = direction * speed;
        // В старых версиях Unity: rb.velocity = direction * speed;

        // Спрайты
        if (moveX < 0f) spriteRenderer.sprite = spriteLeft;
        else if (moveX > 0f) spriteRenderer.sprite = spriteRight;
        else spriteRenderer.sprite = spriteFront;
    }
}