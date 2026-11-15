using UnityEngine;
using UnityEngine.InputSystem;

public class GameLevelManager : MonoBehaviour
{
    [Header("Настройки туториала")]
    [SerializeField] private GameObject tutorialPanel; // Панель туториала
    [SerializeField] private float tutorialDuration = 13f; // 13 секунд

    [Header("Настройки победы")]
    [SerializeField] private GameObject victoryPanel; // Победное окно
    [SerializeField] private Transform playerTransform; // Ссылка на игрока
    [SerializeField] private Transform finishObject;   // Финальный объект на конце уровня
    [SerializeField] private float finishDistance = 1.5f; // Дистанция, на которой засчитывается победа

    private bool isGameStarted = false;
    private bool isVictory = false;

    void Start()
    {
        // 1. В начале игры останавливаем время (Time.timeScale = 0)
        Time.timeScale = 0f;

        // 2. Включаем туториал, выключаем победное окно
        if (tutorialPanel != null) tutorialPanel.SetActive(true);
        if (victoryPanel != null) victoryPanel.SetActive(false);
    }

    void Update()
    {
        // --- Логика таймера туториала (используем Time.unscaledDeltaTime) ---
        if (!isGameStarted)
        {
            tutorialDuration -= Time.unscaledDeltaTime;
            if (tutorialDuration <= 0f)
            {
                StartGame();
            }
            return; // Пока идет туториал, больше ничего не проверяем
        }

        // --- Логика проверки победы ---
        if (!isVictory && playerTransform != null && finishObject != null)
        {
            // Считаем расстояние между игроком и финишем
            float distance = Vector2.Distance(playerTransform.position, finishObject.position);
            if (distance <= finishDistance)
            {
                TriggerVictory();
            }
        }

        // --- Закрытие победного окна по клавише Enter ---
        if (isVictory && Keyboard.current != null)
        {
            if (Keyboard.current.enterKey.wasPressedThisFrame || Keyboard.current.numpadEnterKey.wasPressedThisFrame)
            {
                CloseVictoryPanel();
            }
        }
    }

    private void StartGame()
    {
        isGameStarted = true;
        Time.timeScale = 1f; // Запускаем время в игре

        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false); // Скрываем туториал
        }
    }

    private void TriggerVictory()
    {
        isVictory = true;
        Time.timeScale = 0f; // Останавливаем игру при победе

        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true); // Показываем окно победы
        }
        
        Debug.Log("Победа! Игрок достиг финиша.");
    }

    private void CloseVictoryPanel()
    {
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(false);
        }
        Time.timeScale = 1f; // Возвращаем нормальное время
        
        Debug.Log("Победное окно закрыто игроком.");
    }
}