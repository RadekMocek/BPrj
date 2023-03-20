using UnityEngine;

public class Player : MonoBehaviour
{
    [field: Header("Transforms")]
    [field: SerializeField] public Transform Core { get; private set; } // Approx. center of the character's sprite, pivot has to be at the sprite's feet for y-sorting to work    

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
    public PlayerAttackHeavyState AttackHeavyState { get; private set; }
    public PlayerDashState DashState { get; private set; }
    public PlayerDialogueState DialogueState { get; private set; }

    public void ChangeState(PlayerState newState)
    {
        if (currentState == IdleState) IdleState.momentumDirection = Vector2.zero;
        else if (currentState == SneakIdleState) SneakIdleState.momentumDirection = Vector2.zero;

        newState.Enter();
        currentState = newState;
    }

    // == Movement ==============================
    private Vector2 movementInputTempVector; // Saving input to variable so we don't have to call new() every frame
    public Direction LastMovementDirection { get; set; } // Used in IdleState.Enter()/... to set correct sprite

    public Vector2 GetNormalizedMovementInput()
    {
        movementInputTempVector.Set(IH.MovementX, IH.MovementY);
        return (movementInputTempVector.normalized);
    }

    // == Sneaking ==============================
    public bool Sneaking { get; set; }

    // == Dashing ===============================
    [Header("Dashing")]
    [SerializeField] private GameObject afterImagePrefab;

    private GameObject afterImageGO;
    private PlayerAfterImageEffect afterImageScript;

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
    private RaycastHit2D cursorHit;
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
        cursorHit = Physics2D.Raycast(cursorCoordinates, Vector2.zero, 0);
        if (cursorHit) {
            cursorHitGO = cursorHit.transform.gameObject;
            if (cursorHitGO.TryGetComponent(out cursorHitScriptObservable)) {
                
                HUD.SetObserveNameText(cursorHitScriptObservable.GetName());

                if (cursorHitGO.TryGetComponent(out cursorHitScriptInteractable)) {

                    HUD.SetInteractActionText("(" + IH.InteractBinding + ") " + cursorHitScriptInteractable.GetInteractActionDescription());

                    if (cursorHitScriptInteractable.CanInteract(this)) {

                        HUD.SetIsInteractActionPossible(true);

                        if (IH.InteractAction.WasPressedThisFrame()) {
                            cursorHitScriptInteractable.OnInteract(this);
                        }
                    }
                    else {
                        HUD.SetIsInteractActionPossible(false);
                    }
                }
                else {
                    HUD.SetInteractActionText("");
                }
            }
        }
        else {
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
        AttackHeavyState = new PlayerAttackHeavyState(this);
        DashState = new PlayerDashState(this);
        DialogueState = new PlayerDialogueState(this);
    }

    private void Start()
    {
        LastMovementDirection = Direction.S;

        Sneaking = false;

        WeaponEquipped = false;
        
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
