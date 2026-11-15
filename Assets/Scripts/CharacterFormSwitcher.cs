using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CharacterFormSwitcher : MonoBehaviour
{
    [System.Serializable]
    public class CharacterForm
    {
        public string formName; 
        
        [Header("4 картинки персонажа")]
        public Sprite defaultSprite;     
        public Sprite spriteFront;       
        public Sprite spriteLeft;        
        public Sprite spriteRight;       

        [Header("Способности и ограничения")]
        public bool canPhase;            
        public bool cantJump;            // Новый флаг: может ли этот персонаж прыгать?

        [Header("Эффекты одержимости (раздельные)")]
        public bool canBeInvisible;      
        public bool canBeSlowed;         
    }

    [Header("Список всех персонажей")]
    public List<CharacterForm> forms = new List<CharacterForm>();

    [Header("UI для предупреждения об одержимости")]
    [SerializeField] private Text possessionText; 

    [Header("Настройки безопасности")]
    [SerializeField] private float safeWindowDuration = 2f; 

    private int currentFormIndex = 0;
    private bool canSwitchForms = false; 

    private SpriteRenderer spriteRenderer;
    private PlayerMovementPhysics movementScript;
    private PlayerPhase phaseScript;

    private float possessionTimer = 0f;
    private bool isPossessed = false;
    private int activePossessionType = 0; 
    private float originalSpeed = 10f;    

    private float safeWindowTimer = 0f;
    private bool isInSafeWindow = false;
    private CharacterForm currentForm;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        movementScript = GetComponent<PlayerMovementPhysics>();
        phaseScript = GetComponent<PlayerPhase>();

        if (movementScript != null)
        {
            originalSpeed = movementScript.speed; 
        }

        if (possessionText != null)
        {
            possessionText.text = ""; 
        }

        if (forms.Count > 0)
        {
            ApplyForm(0); 
        }
    }

    void Update()
    {
        if (isInSafeWindow)
        {
            safeWindowTimer -= Time.deltaTime;
            if (safeWindowTimer <= 0f)
            {
                isInSafeWindow = false;
                if (currentForm != null)
                {
                    TryTriggerPossession(currentForm);
                }
            }
        }

        if (isPossessed)
        {
            possessionTimer -= Time.deltaTime;
            if (possessionTimer <= 0f)
            {
                EndPossession();
            }
        }

        if (!canSwitchForms || Keyboard.current == null || forms.Count <= 1) return;

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            NextForm();
        }
    }

    public void UnlockFormAccess()
    {
        canSwitchForms = true;
        Debug.Log("Смена форм разблокирована!");
    }

    public void NextForm()
    {
        if (forms.Count == 0) return;
        
        if (isPossessed) EndPossession();

        currentFormIndex = (currentFormIndex + 1) % forms.Count;
        ApplyForm(currentFormIndex);
    }

    private void ApplyForm(int index)
    {
        if (index < 0 || index >= forms.Count) return;
        currentForm = forms[index];

        if (spriteRenderer != null && currentForm.defaultSprite != null)
        {
            spriteRenderer.sprite = currentForm.defaultSprite;
        }

        if (movementScript != null)
        {
            movementScript.spriteFront = currentForm.spriteFront;
            movementScript.spriteLeft = currentForm.spriteLeft;
            movementScript.spriteRight = currentForm.spriteRight;

            // Передаем в скрипт движения запрет или разрешение прыжка
            movementScript.CanJump = !currentForm.cantJump;
        }

        if (phaseScript != null)
        {
            if (!currentForm.canPhase && spriteRenderer != null)
            {
                Color c = spriteRenderer.color;
                c.a = 1f;
                spriteRenderer.color = c;
            }

            phaseScript.enabled = currentForm.canPhase;
        }

        safeWindowTimer = safeWindowDuration;
        isInSafeWindow = true;
    }

    private void TryTriggerPossession(CharacterForm form)
    {
        List<int> availableEffects = new List<int>();

        if (form.canBeInvisible) availableEffects.Add(1); 
        if (form.canBeSlowed) availableEffects.Add(2);    

        if (availableEffects.Count == 0) return;

        possessionTimer = Random.Range(2f, 7.1f);
        isPossessed = true;

        int randomIndex = Random.Range(0, availableEffects.Count);
        activePossessionType = availableEffects[randomIndex];

        if (activePossessionType == 1)
        {
            if (spriteRenderer != null)
            {
                Color c = spriteRenderer.color;
                c.a = 0f; 
                spriteRenderer.color = c;
            }

            if (possessionText != null)
            {
                possessionText.text = "You are possessed by Invisibility!";
            }
            Debug.Log("Одержимость: Невидимость на " + possessionTimer.ToString("F1") + " сек.");
        }
        else if (activePossessionType == 2)
        {
            if (movementScript != null)
            {
                movementScript.speed = originalSpeed / 2f;
            }

            if (possessionText != null)
            {
                possessionText.text = "You are possessed by Fatigue!";
            }
            Debug.Log("Одержимость: Усталость на " + possessionTimer.ToString("F1") + " сек.");
        }
    }

    private void EndPossession()
    {
        isPossessed = false;

        if (spriteRenderer != null)
        {
            Color c = spriteRenderer.color;
            c.a = 1f;
            spriteRenderer.color = c;
        }

        if (movementScript != null)
        {
            movementScript.speed = originalSpeed;
        }

        if (possessionText != null)
        {
            possessionText.text = "";
        }

        Debug.Log("Эффект одержимости спал.");
    }
}