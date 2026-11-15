using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WallMonsterMove : MonoBehaviour
{
[Header("Режим тестирования")]
[Tooltip("Если включено, монстр не убивает игрока и сцена не перезагружается (для тестов уровней)")]
[SerializeField] private bool godMode = false;

[Header("Ссылки")]
[SerializeField] private Transform player;
[SerializeField] private GameObject deathScreen; // Твоя панель DeathScreen
[SerializeField] private CanvasGroup deathCanvasGroup; // Компонент CanvasGroup с этой панели

[Header("Настройки движения и ускорения")]
[SerializeField] private float initialSpeed = 5f;
[SerializeField] private float accelerationRate = 0.5f;

[Header("Настройки смерти")]
[SerializeField] private float fadeDuration = 1f; // Сколько секунд плавно появляется надпись
[SerializeField] private float restartDelay = 2.5f; // Сколько секунд висит надпись до перезагрузки

private float currentSpeed;
private bool isDead = false;

void Start()
{
currentSpeed = initialSpeed;

if (player == null)
Debug.LogError("Не назначен объект игрока на " + gameObject.name);

if (deathScreen == null)
Debug.LogError("Не назначена панель DeathScreen на " + gameObject.name);
else
deathScreen.SetActive(false); // Гарантируем, что при старте выключена
}

void Update()
{
if (isDead) return;

currentSpeed += accelerationRate * Time.deltaTime;
transform.Translate(Vector3.right * currentSpeed * Time.deltaTime);

// Если включен режим тестирования, страховка по оси X тоже не срабатывает
if (!godMode && player != null && transform.position.x > player.position.x)
{
TriggerDeath();
}
}

// Для 2D-коллайдеров
private void OnTriggerEnter2D(Collider2D other)
{
if (other.transform == player)
{
TriggerDeath();
}
}

private void OnCollisionEnter2D(Collision2D collision)
{
if (collision.transform == player)
{
TriggerDeath();
}
}

private void TriggerDeath()
{
// Если активирован режим бога (тест), полностью игнорируем столкновение со смертью
if (godMode) return;

if (isDead) return;
isDead = true;

if (player != null)
{
player.gameObject.SetActive(false);
}

StartCoroutine(DeathSequence());
}

private IEnumerator DeathSequence()
{
if (deathScreen != null)
{
deathScreen.SetActive(true);

if (deathCanvasGroup != null)
{
deathCanvasGroup.alpha = 0f;

Transform textTransform = deathCanvasGroup.transform;
Vector3 startScale = Vector3.one * 0.8f;
Vector3 targetScale = Vector3.one;
textTransform.localScale = startScale;

float elapsed = 0f;

while (elapsed < fadeDuration)
{
elapsed += Time.deltaTime;
float progress = Mathf.Clamp01(elapsed / fadeDuration);

deathCanvasGroup.alpha = progress;
textTransform.localScale = Vector3.Lerp(startScale, targetScale, progress);

yield return null;
}

deathCanvasGroup.alpha = 1f;
textTransform.localScale = targetScale;
}
}

yield return new WaitForSeconds(restartDelay);
SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
}
}