using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterFormSwitcher : MonoBehaviour
{
    // Специальный контейнер-анкета для каждого персонажа
    [System.Serializable]
    public class CharacterForm
    {
        public string formName;          // Просто название для удобства (например: "Призрак" или "Человек")
        
        [Header("4 картинки персонажа")]
        public Sprite defaultSprite;     // Картинка для самого SpriteRenderer
        public Sprite spriteFront;       // Для PlayerMovementPhysics: прямо
        public Sprite spriteLeft;        // Для PlayerMovementPhysics: влево
        public Sprite spriteRight;       // Для PlayerMovementPhysics: вправо

        [Header("Способности")]
        public bool canPhase;            // Включать ли скрипт PlayerPhase для этой формы?
    }

    [Header("Список всех персонажей")]
    public List<CharacterForm> forms = new List<CharacterForm>();

    private int currentFormIndex = 0;

    // Ссылки на компоненты на этом же объекте
    private SpriteRenderer spriteRenderer;
    private PlayerMovementPhysics movementScript;
    private PlayerPhase phaseScript;

    void Start()
    {
        // Автоматически находим все нужные компоненты на объекте
        spriteRenderer = GetComponent<SpriteRenderer>();
        movementScript = GetComponent<PlayerMovementPhysics>();
        phaseScript = GetComponent<PlayerPhase>();

        // При старте игры сразу применяем самого первого персонажа из списка (индекс 0)
        if (forms.Count > 0)
        {
            ApplyForm(0);
        }
    }

    void Update()
    {
        if (Keyboard.current == null || forms.Count <= 1) return;

        // Переключение на следующего персонажа по кругу на клавишу E (или Tab)
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            NextForm();
        }
    }

    public void NextForm()
    {
        // Переходим к следующему индексу. Если дошли до конца списка — возвращаемся к 0
        currentFormIndex = (currentFormIndex + 1) % forms.Count;
        ApplyForm(currentFormIndex);
    }

    private void ApplyForm(int index)
    {
        CharacterForm form = forms[index];

        // 1. Меняем основную картинку в SpriteRenderer
        if (spriteRenderer != null && form.defaultSprite != null)
        {
            spriteRenderer.sprite = form.defaultSprite;
        }

        // 2. Меняем 3 спрайта направления в скрипте движения
        if (movementScript != null)
        {
            movementScript.spriteFront = form.spriteFront;
            movementScript.spriteLeft = form.spriteLeft;
            movementScript.spriteRight = form.spriteRight;
        }

        // 3. Включаем или выключаем способность фазирования
        if (phaseScript != null)
        {
            // Важно: если мы переключаемся с призрака в момент, когда он полупрозрачный,
            // возвращаем ему нормальную непрозрачность (альфа-канал = 1)
            if (!form.canPhase && spriteRenderer != null)
            {
                Color c = spriteRenderer.color;
                c.a = 1f;
                spriteRenderer.color = c;
            }

            phaseScript.enabled = form.canPhase;
        }
    }
}