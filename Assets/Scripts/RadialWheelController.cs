using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class RadialWheelController : MonoBehaviour
{
    [Header("Ссылка на твой основной скрипт")]
    public CharacterWheelUI characterWheel;

    [Header("UI колеса (круг на 4 деления и 4 иконки)")]
    public RectTransform wheelRect;            // Объект со спрайтом круга (или сам WheelPanel)
    public Image[] sectorIcons = new Image[4]; // 4 иконки внутри делений

    [Header("Настройки наведения")]
    [Tooltip("Включи, если линии делений на спрайте идут крестом (+). Выключи, если буквой (X) под 45°.")]
    public bool crossDividers = true;
    [Tooltip("Мертвая зона в центре круга (в пикселях), где выбор не срабатывает")]
    public float deadZoneRadius = 40f;
    public float hoverScale = 1.2f;

    private int hoveredIndex = -1;
    private bool isHoldingE = false;
    private Canvas parentCanvas;

    void Start()
    {
        if (characterWheel == null)
            characterWheel = GetComponent<CharacterWheelUI>();

        if (wheelRect != null)
            parentCanvas = wheelRect.GetComponentInParent<Canvas>();

        if (characterWheel != null)
        {
            // Отключаем Update() у первого скрипта, чтобы он сам не реагировал на клавишу E
            // Его методы SetWheelState и SelectCharacter при этом продолжат работать!
            characterWheel.enabled = false;
            characterWheel.SetWheelState(false);
        }
    }

    void Update()
    {
        if (Keyboard.current == null || Mouse.current == null || characterWheel == null) return;

        // 1. Зажали E — открываем колесо через твой скрипт
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            isHoldingE = true;
            hoveredIndex = -1;
            characterWheel.SetWheelState(true);
            ResetIconsVisual();
        }

        // 2. Пока держим E — определяем сектор по положению мыши
        if (isHoldingE && Keyboard.current.eKey.isPressed)
        {
            UpdateHoveredSector();
        }

        // 3. Отпустили E — выбираем персонажа (если мышь не в центре) и закрываем колесо
        if (isHoldingE && Keyboard.current.eKey.wasReleasedThisFrame)
        {
            isHoldingE = false;

            if (hoveredIndex >= 0 && hoveredIndex < characterWheel.characters.Length)
            {
                // SelectCharacter сам применит спрайты и вызовет SetWheelState(false)
                characterWheel.SelectCharacter(hoveredIndex);
            }
            else
            {
                // Если отпустили E, оставив мышь в центре (без выбора) — просто закрываем меню
                characterWheel.SetWheelState(false);
            }

            ResetIconsVisual();
        }
    }

    private void UpdateHoveredSector()
    {
        if (wheelRect == null) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();

        // Корректно находим центр колеса для любого режима Canvas (Overlay или Camera)
        Camera uiCam = (parentCanvas != null && parentCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
            ? parentCanvas.worldCamera
            : null;
        Vector2 wheelCenter = RectTransformUtility.WorldToScreenPoint(uiCam, wheelRect.position);

        Vector2 dir = mousePos - wheelCenter;

        // Если мышка близко к центру круга — ничего не выбираем
        if (dir.magnitude < deadZoneRadius)
        {
            hoveredIndex = -1;
        }
        else
        {
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            if (angle < 0) angle += 360f;

            if (crossDividers)
            {
                // Деления идут как "+" (0°, 90°, 180°, 270°)
                hoveredIndex = Mathf.FloorToInt(angle / 90f) % 4;
            }
            else
            {
                // Деления идут как "X" (45°, 135°, 225°, 315°)
                float shiftedAngle = (angle + 45f) % 360f;
                hoveredIndex = Mathf.FloorToInt(shiftedAngle / 90f) % 4;
            }
        }

        // Подсвечиваем только выбранную иконку
        for (int i = 0; i < sectorIcons.Length; i++)
        {
            if (sectorIcons[i] == null) continue;

            bool isHovered = (i == hoveredIndex);
            sectorIcons[i].transform.localScale = isHovered ? Vector3.one * hoverScale : Vector3.one;
        }
    }

    private void ResetIconsVisual()
    {
        for (int i = 0; i < sectorIcons.Length; i++)
        {
            if (sectorIcons[i] != null)
                sectorIcons[i].transform.localScale = Vector3.one;
        }
    }
}