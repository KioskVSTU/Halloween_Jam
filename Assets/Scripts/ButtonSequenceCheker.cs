using UnityEngine;
using UnityEngine.UI;

public class ButtonSequenceChecker : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Text infoText;
    [Header("Иконка нового персонажа в UI (например, портрет второго героя)")]
    [SerializeField] private GameObject characterIconUI; 

    [Header("Ссылка на игрока (где висит CharacterFormSwitcher)")]
    [SerializeField] private CharacterFormSwitcher formSwitcher;

    private int clickedCount = 0;
    private bool isUnlocked = false; 

    // Отдельные флаги для каждой из трех кнопок, чтобы нельзя было нажать одну и ту же
    private bool firstButtonClicked = false;
    private bool secondButtonClicked = false;
    private bool thirdButtonClicked = false;

    void Start()
    {
        if (characterIconUI != null)
        {
            characterIconUI.SetActive(false);
        }

        if (formSwitcher == null)
        {
            formSwitcher = FindObjectOfType<CharacterFormSwitcher>();
        }
    }

    // Метод для первой кнопки
    public void OnFirstButtonClicked()
    {
        if (isUnlocked || firstButtonClicked) return; // Если уже нажата — ничего не делаем
        
        firstButtonClicked = true;
        RegisterUniqueClick();
    }

    // Метод для второй кнопки
    public void OnSecondButtonClicked()
    {
        if (isUnlocked || secondButtonClicked) return; 
        
        secondButtonClicked = true;
        RegisterUniqueClick();
    }

    // Метод для третьей кнопки
    public void OnThirdButtonClicked()
    {
        if (isUnlocked || thirdButtonClicked) return; 
        
        thirdButtonClicked = true;
        RegisterUniqueClick();
    }

    // Общий метод для подсчета уникальных кликов
    private void RegisterUniqueClick()
    {
        clickedCount++;

        if (infoText != null)
        {
            infoText.text = $"Найдено {clickedCount} из трех";
        }

        if (clickedCount >= 3)
        {
            UnlockNewCharacter();
        }
    }

    private void UnlockNewCharacter()
    {
        isUnlocked = true;
        Debug.Log("Победа! Все 3 разные кнопки собраны, персонаж разблокирован!");

        if (characterIconUI != null)
        {
            characterIconUI.SetActive(true);
        }

        if (formSwitcher != null)
        {
            formSwitcher.UnlockFormAccess();
        }
    }
}