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

        [Header("Параметры формы")]
        public float formSpeed = 2.5f;   // Индивидуальная скорость для этой формы

        [Header("Способности и ограничения")]
        public bool canPhase;            
        public bool cantJump;            

        [Header("Эффекты одержимости (раздельные)")]
        public bool canBeInvisible;      
        public bool canBeSlowed;         
        public bool canBeConstantMoving; 
        public bool canBeInverted;       
    }

    [Header("Список всех персонажей")]
    public List<CharacterForm> forms = new List<CharacterForm>();

    [Header("UI для предупреждения об одержимости")]
    [SerializeField] private Text possessionText; 

    [Header("Настройки безопасности")]
    [SerializeField] private float safeWindowDuration = 2f; 

    private int currentFormIndex = 0;
    private int unlockedFormLimit = 1; 

    private SpriteRenderer spriteRenderer;
    private PlayerMovementPhysics movementScript;
    private PlayerPhase phaseScript;

    private float possessionTimer = 0f;
    private bool isPossessed = false;
    private int activePossessionType = 0; 
    private float currentFormBaseSpeed = 2.5f; // Базовая скорость текущей формы (до дебаффов)   

    private float safeWindowTimer = 0f;
    private bool isInSafeWindow = false;
    private CharacterForm currentForm;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        movementScript = GetComponent<PlayerMovementPhysics>();
        phaseScript = GetComponent<PlayerPhase>();

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

        if (unlockedFormLimit <= 1 || Keyboard.current == null || forms.Count <= 1) return;

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            NextForm();
        }
    }

    public void UnlockNextFormTier()
    {
        if (unlockedFormLimit < forms.Count)
        {
            unlockedFormLimit++;
            Debug.Log("Доступна новая форма! Всего разблокировано: " + unlockedFormLimit);
        }
    }

    public void NextForm()
    {
        if (forms.Count == 0) return;
        
        if (isPossessed) EndPossession();

        currentFormIndex = (currentFormIndex + 1) % unlockedFormLimit;
        ApplyForm(currentFormIndex);
    }

    private void ApplyForm(int index)
    {
        if (index < 0 || index >= forms.Count) return;
        currentForm = forms[index];

        // Запоминаем базовую скорость этой формы и передаем в физику движения
        currentFormBaseSpeed = currentForm.formSpeed;
        if (movementScript != null)
        {
            movementScript.speed = currentFormBaseSpeed;
        }

        if (spriteRenderer != null && currentForm.defaultSprite != null)
        {
            spriteRenderer.sprite = currentForm.defaultSprite;
        }

        if (movementScript != null)
        {
            movementScript.spriteFront = currentForm.spriteFront;
            movementScript.spriteLeft = currentForm.spriteLeft;
            movementScript.spriteRight = currentForm.spriteRight;
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
        if (form.canBeConstantMoving) availableEffects.Add(3); 
        if (form.canBeInverted) availableEffects.Add(4);       

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
        }
        else if (activePossessionType == 2)
        {
            if (movementScript != null)
            {
                // Усталость делит текущую скорость формы пополам
                movementScript.speed = currentFormBaseSpeed / 2f;
            }

            if (possessionText != null)
            {
                possessionText.text = "You are possessed by Fatigue!";
            }
        }
        else if (activePossessionType == 3)
        {
            if (movementScript != null)
            {
                movementScript.IsConstantMoving = true;
            }

            if (possessionText != null)
            {
                possessionText.text = "You are possessed by Constant Movement!";
            }
        }
        else if (activePossessionType == 4)
        {
            if (movementScript != null)
            {
                movementScript.IsInverted = true;
            }

            if (possessionText != null)
            {
                possessionText.text = "You are possessed by Inversion!";
            }
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
            // Возвращаем скорость к базовой скорости текущей формы
            movementScript.speed = currentFormBaseSpeed;
            movementScript.IsConstantMoving = false;
            movementScript.IsInverted = false;
        }

        if (possessionText != null)
        {
            possessionText.text = "";
        }
    }
}