using UnityEngine;
using UnityEngine.UI;

public class ButtonSequenceChecker : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Text infoText;
    
    [Header("Иконки персонажей в UI")]
    [SerializeField] private GameObject ghostIconUI;   // Иконка Призрака (открывается после 3 кнопок)
    [SerializeField] private GameObject scarecrowIconUI; // Иконка Пугала (открывается после 6 кнопок)
    [SerializeField] private GameObject wolfIconUI;     // Иконка Волка (открывается после 8 кнопок)

    [Header("Ссылка на игрока (где висит CharacterFormSwitcher)")]
    [SerializeField] private CharacterFormSwitcher formSwitcher;

    private int totalClickedCount = 0;

    // Флаги, чтобы нельзя было кликать по уже активированным кнопкам повторно (пример для 8 уникальных кнопок)
    private bool btn1, btn2, btn3, btn4, btn5, btn6, btn7, btn8;

    void Start()
    {
        if (ghostIconUI) ghostIconUI.SetActive(false);
        if (scarecrowIconUI) scarecrowIconUI.SetActive(false);
        if (wolfIconUI) wolfIconUI.SetActive(false);

        if (formSwitcher == null)
        {
            formSwitcher = FindObjectOfType<CharacterFormSwitcher>();
        }
    }

    // Методы для привязки к кнопкам (по одному на каждую кнопку уровня)
    public void OnButtonClicked_1() { if (!btn1) { btn1 = true; RegisterClick(); } }
    public void OnButtonClicked_2() { if (!btn2) { btn2 = true; RegisterClick(); } }
    public void OnButtonClicked_3() { if (!btn3) { btn3 = true; RegisterClick(); } }
    public void OnButtonClicked_4() { if (!btn4) { btn4 = true; RegisterClick(); } }
    public void OnButtonClicked_5() { if (!btn5) { btn5 = true; RegisterClick(); } }
    public void OnButtonClicked_6() { if (!btn6) { btn6 = true; RegisterClick(); } }
    public void OnButtonClicked_7() { if (!btn7) { btn7 = true; RegisterClick(); } }
    public void OnButtonClicked_8() { if (!btn8) { btn8 = true; RegisterClick(); } }

    private void RegisterClick()
    {
        totalClickedCount++;

        if (infoText != null)
        {
            infoText.text = $"Найдено предметов: {totalClickedCount} из 8";
        }

        // Этап 1: Первые 3 кнопки -> Открываем Призрака (индекс 1)
        if (totalClickedCount == 3)
        {
            if (ghostIconUI) ghostIconUI.SetActive(true);
            if (formSwitcher != null) formSwitcher.UnlockNextFormTier();
            Debug.Log("Призрак разблокирован!");
        }
        // Этап 2: Следующие 3 кнопки (всего 6) -> Открываем Пугало (индекс 2)
        else if (totalClickedCount == 6)
        {
            if (scarecrowIconUI) scarecrowIconUI.SetActive(true);
            if (formSwitcher != null) formSwitcher.UnlockNextFormTier();
            Debug.Log("Пугало разблокировано!");
        }
        // Этап 3: Последние 2 кнопки (всего 8) -> Открываем Волка (индекс 3)
        else if (totalClickedCount == 8)
        {
            if (wolfIconUI) wolfIconUI.SetActive(true);
            if (formSwitcher != null) formSwitcher.UnlockNextFormTier();
            Debug.Log("Волк разблокирован! Все формы доступны!");
        }
    }
}