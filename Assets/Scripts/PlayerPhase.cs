using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerPhase : MonoBehaviour
{
    [Header("Спрайты анимации (3 кадра)")]
    public Sprite[] phaseSprites = new Sprite[3];

    [Header("Настройки")]
    public float fadeDuration = 1f;
    public float invisibleDuration = 5f;
    public float animFrameRate = 0.1f;

    [Header("Физика в фазе")]
    [Tooltip("Если включено, выключает коллайдеры и гравитацию (чтобы не провалиться сквозь пол)")]
    public bool disableAllColliders = true;

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Collider2D[] colliders;
    private PlayerMovementPhysics movement;
    private bool isPhasing = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        colliders = GetComponents<Collider2D>();
        movement = GetComponent<PlayerMovementPhysics>();
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.xKey.wasPressedThisFrame && !isPhasing)
        {
            StartCoroutine(PhaseCoroutine());
        }
    }

    private IEnumerator PhaseCoroutine()
    {
        isPhasing = true;
        float originalGravity = rb.gravityScale;

        // 1. Исчезновение (1 сек) — горизонтальное движение заблокировано
        if (movement != null) movement.CanMove = false;
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        yield return FadeWithAnimation(1f, 0f);

        // 2. Невидимость (5 сек) — движение РАЗБЛОКИРОВАНО
        if (disableAllColliders)
        {
            rb.gravityScale = 0f; // Выключаем гравитацию, чтобы не улететь сквозь пол
            rb.linearVelocity = Vector2.zero;
            foreach (var col in colliders) col.enabled = false;
        }

        if (movement != null) movement.CanMove = true;

        yield return new WaitForSeconds(invisibleDuration);

        // 3. Перед появлением — снова блокируем движение и возвращаем коллайдеры
        if (movement != null) movement.CanMove = false;
        rb.linearVelocity = Vector2.zero;

        if (disableAllColliders)
        {
            foreach (var col in colliders) col.enabled = true;
            rb.gravityScale = originalGravity;
        }

        // 4. Появление (1 сек) — движение заблокировано
        yield return FadeWithAnimation(0f, 1f);

        // 5. Возвращаем управление и базовый спрайт
        if (movement != null)
        {
            if (movement.spriteFront != null)
                spriteRenderer.sprite = movement.spriteFront;
            movement.CanMove = true;
        }

        isPhasing = false;
    }

    private IEnumerator FadeWithAnimation(float fromAlpha, float toAlpha)
    {
        float elapsed = 0f;
        int frameIndex = 0;
        float frameTimer = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            frameTimer += Time.deltaTime;

            if (frameTimer >= animFrameRate && phaseSprites != null && phaseSprites.Length > 0)
            {
                frameTimer = 0f;
                if (phaseSprites[frameIndex] != null)
                    spriteRenderer.sprite = phaseSprites[frameIndex];
                frameIndex = (frameIndex + 1) % phaseSprites.Length;
            }

            float alpha = Mathf.Lerp(fromAlpha, toAlpha, elapsed / fadeDuration);
            Color c = spriteRenderer.color;
            c.a = alpha;
            spriteRenderer.color = c;

            yield return null;
        }

        Color final = spriteRenderer.color;
        final.a = toAlpha;
        spriteRenderer.color = final;
    }
}