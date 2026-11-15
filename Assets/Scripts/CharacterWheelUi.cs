using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class CharacterWheelUI : MonoBehaviour
{
    [System.Serializable]
    public class CharacterData
    {
        public string name;
        public Sprite icon;        // Иконка в колесе (если нужно обновлять автоматически)
        public Sprite spriteFront; // Спрайт "анфас" (стоит)
        public Sprite spriteLeft;  // Спрайт "влево"
        public Sprite spriteRight; // Спрайт "вправо"
        public bool isUnlocked = true; // Можно закрыть 2 и 3 персонажа до клика по предметам на фоне
    }

    [Header("Список из 3 персонажей")]
    public CharacterData[] characters = new CharacterData[3];

    [Header("Ссылки на UI колеса")]
    public GameObject wheelPanel;     // Панель колеса (Canvas -> WheelPanel)
    public Button[] wheelButtons = new Button[3]; // 3 кнопки по кругу
    public Image[] buttonIcons = new Image[3];    // Иконки внутри кнопок (опционально)

    [Header("Настройки управления")]
    [Tooltip("Если включено — нужно удерживать E. Если выключено — нажатие E открывает/закрывает меню.")]
    public bool holdToOpen = false;
    public bool slowMotionWhenOpen = true;

    private PlayerMovementPhysics playerMovement;
    private SpriteRenderer playerSpriteRenderer;
    private bool isWheelOpen = false;
    private int currentCharacterIndex = 0;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovementPhysics>();
        playerSpriteRenderer = GetComponent<SpriteRenderer>();

        if (wheelPanel != null)
            wheelPanel.SetActive(false);

        // Привязываем клики по 3 кнопкам колеса
        for (int i = 0; i < wheelButtons.Length; i++)
        {
            int index = i; // Важно сохранить локальную копию индекса для лямбды
            if (wheelButtons[i] != null)
            {
                wheelButtons[i].onClick.AddListener(() => SelectCharacter(index));
            }

            // Если назначены Image для иконок — сразу ставим туда спрайты персонажей
            if (i < buttonIcons.Length && buttonIcons[i] != null && i < characters.Length)
            {
                Sprite preview = characters[i].icon != null ? characters[i].icon : characters[i].spriteFront;
                if (preview != null) buttonIcons[i].sprite = preview;
            }
        }

        // Применяем первого персонажа на старте
        if (characters.Length > 0)
            ApplyCharacterSprites(0);
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        if (holdToOpen)
        {
            // Режим удержания клавиши E
            if (Keyboard.current.eKey.wasPressedThisFrame)
                SetWheelState(true);
            else if (Keyboard.current.eKey.wasReleasedThisFrame)
                SetWheelState(false);
        }
        else
        {
            // Режим переключения по нажатию E
            if (Keyboard.current.eKey.wasPressedThisFrame)
                SetWheelState(!isWheelOpen);
        }

        // Быстрый выбор цифрами 1, 2, 3 пока открыто колесо
        if (isWheelOpen)
        {
            if (Keyboard.current.digit1Key.wasPressedThisFrame) SelectCharacter(0);
            if (Keyboard.current.digit2Key.wasPressedThisFrame) SelectCharacter(1);
            if (Keyboard.current.digit3Key.wasPressedThisFrame) SelectCharacter(2);
        }
    }

    public void SetWheelState(bool open)
    {
        isWheelOpen = open;

        if (wheelPanel != null)
            wheelPanel.SetActive(open);

        // Обновляем доступность кнопок (если персонаж ещё не разблокирован — кнопка неактивна)
        if (open)
        {
            for (int i = 0; i < wheelButtons.Length && i < characters.Length; i++)
            {
                if (wheelButtons[i] != null)
                    wheelButtons[i].interactable = characters[i].isUnlocked;
            }
        }

        // Блокируем ходьбу и замедляем время, пока выбираем персонажа
        if (playerMovement != null)
            playerMovement.CanMove = !open;

        if (slowMotionWhenOpen)
            Time.timeScale = open ? 0.2f : 1f;
    }

    public void SelectCharacter(int index)
    {
        if (index < 0 || index >= characters.Length) return;
        if (!characters[index].isUnlocked) return;

        currentCharacterIndex = index;
        ApplyCharacterSprites(index);

        // Закрываем колесо после выбора
        SetWheelState(false);
    }

    private void ApplyCharacterSprites(int index)
    {
        CharacterData data = characters[index];

        // Меняем спрайты в твоём скрипте движения
        if (playerMovement != null)
        {
            playerMovement.spriteFront = data.spriteFront;
            playerMovement.spriteLeft = data.spriteLeft;
            playerMovement.spriteRight = data.spriteRight;
        }

        // Сразу обновляем текущий видимый спрайт на сцене
        if (playerSpriteRenderer != null && data.spriteFront != null)
        {
            playerSpriteRenderer.sprite = data.spriteFront;
        }
    }

    // Метод для разблокировки персонажа при клике на предмет на фоне
    public void UnlockCharacter(int index)
    {
        if (index >= 0 && index < characters.Length)
        {
            characters[index].isUnlocked = true;
            Debug.Log($"Персонаж {characters[index].name} разблокирован!");
        }
    }
}