using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerPhase : MonoBehaviour
{
    [Header("Спрайты анимации (3 кадра)")]
    public Sprite[] phaseSprites = new Sprite[3];

    [Header("Звук")]
    public AudioSource audioSource;
    public AudioClip phaseSound;

    [Header("Параметры времени")]
    [Tooltip("Скорость смены кадров анимации растворения")]
    public float animFrameRate = 0.1f;

    [Tooltip("Сколько времени уходит на то, чтобы стать бледным (в 3 раза быстрее обычного)")]
    public float fadeOutDuration = 0.33f;

    [Tooltip("Общее время от нажатия X до возвращения в норму (ровно 3 секунды)")]
    public float totalPhaseDuration = 3f;

    [Tooltip("Скорость возвращения обычного цвета в конце")]
    public float fadeInDuration = 0.33f;

    [Range(0f, 1f)]
    [Tooltip("Минимальная прозрачность в момент фазирования")]
    public float minAlpha = 0.4f;

    [Header("Коллизии и физика")]
    [Tooltip("Если включено, отключаются коллайдеры и гравитация (можно проходить сквозь стены)")]
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

        // Если не перетащил AudioSource вручную — скрипт сам найдет его на персонаже
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.xKey.wasPressedThisFrame && !isPhasing)
        {
            if (audioSource != null && phaseSound != null)
                audioSource.PlayOneShot(phaseSound);

            StartCoroutine(PhaseCoroutine());
        }
    }

    private IEnumerator PhaseCoroutine()
    {
        isPhasing = true;
        float originalGravity = rb.gravityScale;

        // Фиксируем момент нажатия для строгого отсчета 3 секунд
        float timer = 0f;

        // 1. Быстрое побледнение (в 3 раза быстрее)
        if (movement != null) movement.CanMove = false;
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

        yield return FadeWithAnimation(1f, minAlpha, fadeOutDuration);
        timer += fadeOutDuration;

        // 2. Включаем фазирование (проход сквозь стены)
        if (disableAllColliders)
        {
            rb.gravityScale = 0f;
            rb.linearVelocity = Vector2.zero;
            foreach (var col in colliders) col.enabled = false;
        }

        if (movement != null) movement.CanMove = true;

        // 3. Ждем оставшееся время до конца 3-секундного интервала
        float remainingTime = Mathf.Max(0f, totalPhaseDuration - timer);
        yield return new WaitForSeconds(remainingTime);

        // 4. Подготовка к возвращению (запрещаем движение, возвращаем физику/коллайдеры)
        if (movement != null) movement.CanMove = false;
        rb.linearVelocity = Vector2.zero;

        if (disableAllColliders)
        {
            foreach (var col in colliders) col.enabled = true;
            rb.gravityScale = originalGravity;
        }

        // 5. Быстрое возвращение обычной видимости
        yield return FadeWithAnimation(minAlpha, 1f, fadeInDuration);

        // 6. Завершение
        if (movement != null)
        {
            if (movement.spriteFront != null)
                spriteRenderer.sprite = movement.spriteFront;
            movement.CanMove = true;
        }

        isPhasing = false;
    }

    private IEnumerator FadeWithAnimation(float fromAlpha, float toAlpha, float duration)
    {
        // Защита от деления на ноль
        if (duration <= 0f) duration = 0.01f;

        float elapsed = 0f;
        int frameIndex = 0;
        float frameTimer = 0f;

        while (elapsed < duration)
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

            float alpha = Mathf.Lerp(fromAlpha, toAlpha, elapsed / duration);
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