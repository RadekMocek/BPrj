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
    private PlayerState currentState;
    public PlayerIdleState IdleState { get; private set; }
    public PlayerMoveState MoveState { get; private set; }
    public PlayerSneakIdleState SneakIdleState { get; private set; }
    public PlayerSneakMoveState SneakMoveState { get; private set; }
    public PlayerAttackLightState AttackLightState { get; private set; }
    public PlayerDashState DashState { get; private set; }
    public PlayerDialogueState DialogueState { get; private set; }
    public PlayerKnockbackState KnockbackState { get; private set; }

    public void ChangeState(PlayerState newState)
    {
        if (currentState == IdleState) IdleState.momentumDirection = Vector2.zero;
        else if (currentState == SneakIdleState) SneakIdleState.momentumDirection = Vector2.zero;

        currentState?.Exit();
        newState.Enter();
        currentState = newState;
    }

    // == Health, Receive damage, Knockback =====
    [Header("Receive damage")]
    [SerializeField] private GameObject hitBloodParticlePrefab;
    
    private readonly int maxHealth = 100;
    private int health;
    public Vector2 KnockbackDirection { get; private set; }

    public virtual void ReceiveDamage(Vector2 direction, int amount)
    {
        if (currentState == DashState) return;

        health -= amount;

        Instantiate(hitBloodParticlePrefab, this.Core.position, Quaternion.identity);
        CameraShake.Instance.ShakeCamera();

        if (health <= 0) {
            health = 0;
        }

        HUD.SetHealth(health);

        KnockbackDirection = direction;
        ChangeState(KnockbackState);
    }

    // == Stamina ===============================
    private readonly int maxStamina = 100;
    private int stamina;

    public bool CanDash() => (stamina >= PlayerStaticValues.dash_staminaCost);

    public void DecreaseStamina(int amount)
    {
        stamina -= amount;
        if (stamina < 0) stamina = 0;
        HUD.SetStamina(stamina);

        if (isStaminaRegenerationCoroutineRunning) StopCoroutine(staminaRegenerationCoroutine);
        staminaRegenerationCoroutine = StartCoroutine(StaminaRegeneration());
    }

    private void IncreaseStamina(int amount)
    {
        stamina += amount;
        if (stamina > maxStamina) stamina = maxStamina;
        HUD.SetStamina(stamina);
    }

    // Stamina regen
    private Coroutine staminaRegenerationCoroutine;
    private bool isStaminaRegenerationCoroutineRunning;

    private readonly int staminaRegenerationSpeed = 2;

    private IEnumerator StaminaRegeneration()
    {
        isStaminaRegenerationCoroutineRunning = true;
        yield return new WaitForSeconds(1.0f);
        while (stamina < maxStamina) {
            IncreaseStamina(staminaRegenerationSpeed);
            yield return new WaitForSeconds(0.1f);
        }
        isStaminaRegenerationCoroutineRunning = false;
    }

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

    private Collider2D cursorHit;
    private GameObject cursorHitGO;
    private IObservable cursorHitScriptObservable;
    private IPlayerInteractable cursorHitScriptInteractable;

    private void UpdateCursorObserveAndInteract()
    {
        if (currentState == DialogueState) return;

        if (HUD.IsInspecting) {
            if (IH.InteractAction.WasPressedThisFrame() || !HUD.InspectedObjectScript.CanInteract(this)) {
                HUD.StopInspecting();
            }
            return;
        }

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
                    if (cursorHitScriptInteractable.CanInteract(this)) {
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
    public void DialogueStart()
    {
        ChangeState(DialogueState);
    }

    public void DialogueEnd()
    {
        ChangeState(IdleState);
    }

    // == Weapon handling =======================
    public bool WeaponEquipped { get; private set; }
    private GameObject weaponGO;
    public Transform WeaponTransform { get; private set; }
    public SpriteRenderer WeaponSR { get; private set; }

    public void EquipWeapon(GameObject newWeapon)
    {
        if (WeaponEquipped) return; //TODO: Temporary?, two equipped weapons prevention

        weaponGO = newWeapon;
        WeaponTransform = weaponGO.transform;
        WeaponTransform.SetParent(this.transform);
        WeaponSR = weaponGO.GetComponent<SpriteRenderer>();
        WeaponSR.sortingLayerName = "Player";
        WeaponEquipped = true;
        currentState.UpdateWeaponPosition();
    }

    // == Lock, Key =============================
    public HashSet<LockColor> EquippedKeys { get; private set; }

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
    }

    private void Start()
    {
        // Init
        LastMovementDirection = Direction.S;
        IsSneaking = false;
        WeaponEquipped = false;
        EquippedKeys = new HashSet<LockColor>();

        // Health
        health = maxHealth;
        HUD.SetMaxHealth(maxHealth);
        HUD.SetHealth(health);

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
        currentState.FixedUpdate();
    }

    private void Update()
    {
        // State logic
        currentState.Update();

        // Update sub-functions
        UpdateCooldownBar();
        UpdateCursorPositionAndCoordinates();
        UpdateCursorObserveAndInteract();
    }

    // == Debug =================================
    [Header("Debug")]
    public Vector2 gizmoCircleCenter;
    public float gizmoCircleRadius;
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(gizmoCircleCenter, gizmoCircleRadius);
    }
}
