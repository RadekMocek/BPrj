using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDamageable
{
    [field: Header("Transforms")]
    [field: SerializeField] public Transform Core { get; private set; } // Approximate center of the character's sprite, pivot has to be at the sprite's feet for y-sorting to work    

    // == Component references ==================
    public SpriteRenderer SR { get; private set; }
    public Rigidbody2D RB { get; private set; }
    public PlayerInputHandler IH { get; private set; }
    public Animator Anim { get; private set; }

    // == Managers ==============================
    private HUDManager HUD;

    // == State machine =========================
    public PlayerState CurrentState { get; private set; }
    public PlayerIdleState IdleState { get; private set; }
    public PlayerMoveState MoveState { get; private set; }
    public PlayerSneakIdleState SneakIdleState { get; private set; }
    public PlayerSneakMoveState SneakMoveState { get; private set; }
    public PlayerAttackLightState AttackLightState { get; private set; }
    public PlayerDashState DashState { get; private set; }
    public PlayerDialogueState DialogueState { get; private set; }
    public PlayerKnockbackState KnockbackState { get; private set; }
    public PlayerDeadState DeadState { get; private set; }

    public void ChangeState(PlayerState newState)
    {
        if (CurrentState == IdleState) IdleState.momentumDirection = Vector2.zero;
        else if (CurrentState == SneakIdleState) SneakIdleState.momentumDirection = Vector2.zero;

        CurrentState?.Exit();
        newState.Enter();
        CurrentState = newState;
    }

    // == Health, Receive damage, Knockback =====
    [Header("Receive damage")]
    [SerializeField] private GameObject hitBloodParticlePrefab;
    
    private readonly int maxHealth = 40;
    private readonly int maxHealedHealth = 20;
    private int health;
    public Vector2 KnockbackDirection { get; private set; }

    public virtual void ReceiveDamage(Vector2 direction, int amount)
    {
        if (CurrentState == DashState || CurrentState == DeadState) return;

        health -= amount;

        Instantiate(hitBloodParticlePrefab, this.Core.position, Quaternion.identity);
        CameraShake.Instance.ShakeCamera();

        lastDamagedTime = Time.time;

        if (health <= 0) {
            health = 0;
            HUD.SetHealth(health);
            ChangeState(DeadState);
            return;
        }

        HUD.SetHealth(health);

        KnockbackDirection = direction;
        ChangeState(KnockbackState);
    }

    public void ResetHealth()
    {
        health = maxHealth;
        HUD.SetMaxHealth(maxHealth);
        HUD.SetHealth(health);
    }

    // Health regeneration
    private readonly float healthIncreasePauseDuration = 1.6f;
    private readonly float healthIncreaseDamagedPenaltyDuration = 2.0f;

    private float lastHealthIncreaseTime;
    private float lastDamagedTime;

    private void UpdateHealthRegeneration()
    {
        if (stamina == maxStamina
            && health < maxHealedHealth
            && Time.time > lastHealthIncreaseTime + healthIncreasePauseDuration
            && Time.time > lastDamagedTime + healthIncreaseDamagedPenaltyDuration) {
            
            lastHealthIncreaseTime = Time.time;
            IncreaseHealth(1);
        }
    }

    private void IncreaseHealth(int amount)
    {
        health += amount;
        if (health > maxHealth) health = maxHealth;
        HUD.SetHealth(health);
    }
    
    // == Stamina ===============================
    private readonly int maxStamina = 100;
    private int stamina;

    public bool CanDash() => (stamina >= dashStaminaCost);

    public void DecreaseStamina(int amount)
    {
        stamina -= amount;
        if (stamina < 0) stamina = 0;
        HUD.SetStamina(stamina);

        if (isStaminaRegenerationCoroutineRunning) StopCoroutine(staminaRegenerationCoroutine);
        staminaRegenerationCoroutine = StartCoroutine(StaminaRegeneration());
    }

    // Stamina regeneration
    private Coroutine staminaRegenerationCoroutine;
    private bool isStaminaRegenerationCoroutineRunning;

    private readonly int staminaRegenerationSpeed = 2;
    private readonly int staminaRegenerationSpeedIdle = 3;

    private IEnumerator StaminaRegeneration()
    {
        isStaminaRegenerationCoroutineRunning = true;
        yield return new WaitForSeconds(1.0f);
        while (stamina < maxStamina) {
            IncreaseStamina((CurrentState == IdleState || CurrentState == SneakIdleState || CurrentState == DialogueState) ? staminaRegenerationSpeedIdle : staminaRegenerationSpeed);
            yield return new WaitForSeconds(0.1f);
        }
        isStaminaRegenerationCoroutineRunning = false;
    }

    private void IncreaseStamina(int amount)
    {
        stamina += amount;
        if (stamina > maxStamina) stamina = maxStamina;
        HUD.SetStamina(stamina);
    }

    // == Attack ================================
    public readonly int attackDamageNormal = 10;
    public readonly int attackDamageCritical = 20;
    public readonly int attackStaminaCost = 5;

    // == Attack cooldown =======================
    private readonly float attackCooldownDuration = 0.7f;

    [HideInInspector] public float lastAttackTime;
    [HideInInspector] public bool criticalHitMissed;

    public bool CanAttack() => ((!criticalHitMissed && Time.time > lastAttackTime + attackCooldownDuration) || (criticalHitMissed && !IsCooldownBarVisible()));

    private void UpdateCooldownBar()
    {
        if (WeaponEquipped) {
            if (!criticalHitMissed && IsCooldownBarVisible() && Input.GetMouseButtonDown(0) && !CanAttack()) {
                criticalHitMissed = true;
            }
            HUD.ShowCooldownBar(lastAttackTime, attackCooldownDuration, criticalHitMissed);
        }
    }

    public bool IsCooldownBarVisible() => HUD.IsCooldownBarVisible();

    // == Movement ==============================
    private Vector2 movementInputTempVector; // Saving input to variable so we don't have to call new() every frame
    public Direction LastMovementDirection { get; set; } // Used in IdleState.Enter()/... to set correct sprite

    public Vector2 GetNormalizedMovementInput()
    {
        movementInputTempVector.Set(IH.MovementX, IH.MovementY);
        return (movementInputTempVector.normalized);
    }

    // == Sneaking ==============================
    [Header("Sneaking")]
    [SerializeField] private LayerMask ventLayer;
    
    public bool IsSneaking { get; set; }

    public bool IsVenting() => Physics2D.OverlapCircle(this.transform.position, 0.1f, ventLayer);

    // == Dashing ===============================
    [Header("Dashing")]
    [SerializeField] private GameObject afterImagePrefab;

    public static readonly int dashStaminaCost = 25;

    private GameObject afterImageGO;
    private AfterImageEffect afterImageScript;

    public void SpawnAfterImage(Sprite sprite)
    {
        afterImageGO = Instantiate(afterImagePrefab, this.transform.position, Quaternion.identity);
        if (afterImageGO.TryGetComponent(out afterImageScript)) {
            afterImageScript.position = this.transform.position;
            afterImageScript.sprite = sprite;
        }
        afterImageGO.transform.parent = this.transform; // Make it Player's child so it's affected by the Player's Sorting Group component
    }

    // == Cursor position & coordinates =========
    private Vector3 cursorPosition;
    private Vector2 cursorCoordinates;

    private void UpdateCursorPositionAndCoordinates()
    {
        cursorPosition = Input.mousePosition;
        cursorPosition.z = Camera.main.nearClipPlane;
        cursorCoordinates = Camera.main.ScreenToWorldPoint(cursorPosition);
    }

    public Vector2 GetPlayerCoreToCursorDirection() => ((cursorCoordinates - (Vector2)Core.position).normalized);

    // == Cursor observe & interact =============
    [Header("Observe and interact")]
    [SerializeField] private LayerMask observableLayer;
    [SerializeField] private LayerMask facadeLayer;

    private Collider2D cursorHit;
    private GameObject cursorHitGO;
    private IObservable cursorHitScriptObservable;
    private IPlayerInteractable cursorHitScriptInteractable;

    private void UpdateCursorObserveAndInteract()
    {
        if (CurrentState == DialogueState) return;

        if (HUD.IsInspecting) {
            if (IH.InteractAction.WasPressedThisFrame() || !HUD.InspectedObjectScript.CanInteract(this)) {
                HUD.StopInspecting();
            }
            return;
        }

        if (Physics2D.OverlapCircle(cursorCoordinates, 0.25f, facadeLayer)) return;

        // Ray has no direction and no length but it still detects if cursor hovers over something with a collider
        cursorHit = Physics2D.OverlapCircle(cursorCoordinates, 0.25f, observableLayer);
        if (cursorHit) {
            cursorHitGO = cursorHit.transform.gameObject;
            // Is cursorHit observable ?
            if (cursorHitGO.TryGetComponent(out cursorHitScriptObservable)) {
                HUD.SetObserveNameText(cursorHitScriptObservable.GetName());
                // Is cursorHit interactable ?
                if (cursorHitGO.TryGetComponent(out cursorHitScriptInteractable)) {
                    HUD.HideObserveHealthBar();
                    HUD.SetInteractActionText("(" + IH.InteractBinding + ") " + cursorHitScriptInteractable.GetInteractActionDescription(this));
                    // Can player interact with cursorHit ?
                    if (cursorHitScriptInteractable.CanInteract(this) && CurrentState != DeadState) {
                        HUD.SetIsInteractActionPossible(true);
                        // Interaction:
                        if (IH.InteractAction.WasPressedThisFrame()) {
                            cursorHitScriptInteractable.OnInteract(this);
                        }
                    }
                    // player cannot interact:
                    else {
                        HUD.SetIsInteractActionPossible(false);
                    }
                }
                // has cursorHit observable health ?
                else if (cursorHitScriptObservable is IObservableHealth) {
                    HUD.ShowObserveHealthBar(cursorHitScriptObservable as IObservableHealth);
                    HUD.SetInteractActionText("");
                }
                // not interactable or observable health:
                else {
                    HUD.HideObserveHealthBar();
                    HUD.SetInteractActionText("");
                }
            }
            // not observable:
            else {
                HUD.HideObserveHealthBar();
                HUD.SetObserveNameText("");
                HUD.SetInteractActionText("");
            }
        }
        else {
            HUD.HideObserveHealthBar();
            HUD.SetObserveNameText("");
            HUD.SetInteractActionText("");
        }
    }

    // == Dialogue ==============================
    private bool wasSneakingBeforeDialogue;

    public void DialogueStart(Direction facingDirection)
    {
        wasSneakingBeforeDialogue = IsSneaking;
        IsSneaking = false;
        DialogueState.facingDirection = facingDirection;
        ChangeState(DialogueState);
    }

    public void DialogueEnd()
    {
        IsSneaking = wasSneakingBeforeDialogue;
        ChangeState((IsSneaking) ? SneakIdleState : IdleState);
    }

    // == Weapon handling =======================
    public bool WeaponEquipped { get; private set; }
    private GameObject weaponGO;
    public Transform WeaponTransform { get; private set; }
    public SpriteRenderer WeaponSR { get; private set; }

    public void EquipWeapon(GameObject newWeapon)
    {
        if (WeaponEquipped) return;

        weaponGO = newWeapon;
        WeaponTransform = weaponGO.transform;
        WeaponTransform.SetParent(this.transform);
        WeaponSR = weaponGO.GetComponent<SpriteRenderer>();
        WeaponSR.sortingLayerName = "Player";
        WeaponEquipped = true;
        CurrentState.UpdateWeaponPosition();

        HUD.NewTutorial();
        HUD.NewTask();
    }

    // == Lock, Key, Story items ================
    public HashSet<LockColor> EquippedKeys { get; private set; }
    [HideInInspector] public bool hasFlash;

    // == Changing scenes =======================
    public void OnSceneChanged()
    {
        ChangeState(IdleState);
    }

    // == MonoBehaviour functions ===============
    private void Awake()
    {
        // Set components references
        SR = GetComponent<SpriteRenderer>();
        RB = GetComponent<Rigidbody2D>();
        IH = GetComponent<PlayerInputHandler>();
        Anim = GetComponent<Animator>();

        // Set managers
        HUD = ManagerAccessor.instance.HUD;
        HUD.SetPlayerScript(this);

        // States initialization
        IdleState = new PlayerIdleState(this);
        MoveState = new PlayerMoveState(this);
        SneakIdleState = new PlayerSneakIdleState(this);
        SneakMoveState = new PlayerSneakMoveState(this);
        AttackLightState = new PlayerAttackLightState(this);
        DashState = new PlayerDashState(this);
        DialogueState = new PlayerDialogueState(this);
        KnockbackState = new PlayerKnockbackState(this);
        DeadState = new PlayerDeadState(this);
    }

    private void Start()
    {
        // Init
        LastMovementDirection = Direction.S;
        IsSneaking = false;
        WeaponEquipped = false;

        // Lock, Key, Story items
        EquippedKeys = new HashSet<LockColor>();
        
        //TODO: DEBUG
        /*
        hasFlash = true;
        EquippedKeys.Add(LockColor.Red);
        EquippedKeys.Add(LockColor.Blue);
        EquippedKeys.Add(LockColor.Green);
        /**/

        // Health
        ResetHealth();

        // Stamina
        stamina = maxStamina;
        HUD.SetMaxStamina(maxStamina);
        HUD.SetStamina(stamina);
        isStaminaRegenerationCoroutineRunning = false;

        // Attack cooldown
        lastAttackTime = 0;

        // Start state logic
        ChangeState(IdleState);
    }

    private void FixedUpdate()
    {
        // State logic
        CurrentState.FixedUpdate();
    }

    private void Update()
    {
        if (Time.timeScale == 0) return;

        // State logic
        CurrentState.Update();

        // Update sub-functions
        UpdateHealthRegeneration();
        UpdateCooldownBar();
        UpdateCursorPositionAndCoordinates();
        UpdateCursorObserveAndInteract();
    }
}
