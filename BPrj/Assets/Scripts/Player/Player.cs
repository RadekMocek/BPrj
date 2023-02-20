using UnityEngine;

public class Player : MonoBehaviour
{
    // == Values set in the editor =========
    [field: Header("Transforms")]
    [field: SerializeField] public Transform Core { get; private set; } // Approx. center of the character's sprite, pivot has to be at the sprite's feet for y-sorting to work    

    // == Component references =============
    public Rigidbody2D RB { get; private set; }
    public PlayerInputHandler IH { get; private set; }
    public Animator Anim { get; private set; }

    // == State machine ====================
    private PlayerState currentState;
    public PlayerIdleState IdleState { get; private set; }
    public PlayerMoveState MoveState { get; private set; }
    public PlayerSneakIdleState SneakIdleState { get; private set; }
    public PlayerSneakMoveState SneakMoveState { get; private set; }
    public PlayerAttackLightState AttackLightState { get; private set; }
    public PlayerAttackHeavyState AttackHeavyState { get; private set; }
    public PlayerDashState DashState { get; private set; }

    public void ChangeState(PlayerState newState)
    {
        if (currentState == IdleState) IdleState.momentumDirection = Vector2.zero;
        else if (currentState == SneakIdleState) SneakIdleState.momentumDirection = Vector2.zero;

        newState.Enter();
        currentState = newState;
    }

    // == Movement =========================
    private Vector2 movementInputTempVector; // Saving input to variable so we don't have to call new() every frame
    public Direction LastMovementDirection { get; set; } // Used in IdleState.Enter()/... to set correct sprite

    public Vector2 GetNormalizedMovementInput()
    {
        movementInputTempVector.Set(IH.MovementX, IH.MovementY);
        return (movementInputTempVector.normalized);
    }

    // == Sneaking =========================
    public bool Sneaking { get; set; }

    // == Cursor position ==================
    private Vector3 cursorPosition;
    private Vector2 cursorCoordinates;

    public void UpdateCursorPositionAndCoordinates()
    {
        cursorPosition = Input.mousePosition;
        cursorPosition.z = Camera.main.nearClipPlane;
        cursorCoordinates = Camera.main.ScreenToWorldPoint(cursorPosition);
    }

    public Vector2 GetPlayerCoreToCursorDirection() => ((cursorCoordinates - (Vector2)Core.position).normalized);

    // == Weapon handling ==================
    public bool WeaponEquipped { get; private set; }
    private GameObject weaponGO;
    public Transform WeaponTransform { get; private set; }
    public SpriteRenderer WeaponSR { get; private set; }

    public void EquipWeapon(GameObject newWeapon)
    {
        weaponGO = newWeapon;
        WeaponTransform = weaponGO.transform;
        WeaponTransform.SetParent(this.transform);
        WeaponSR = weaponGO.GetComponent<SpriteRenderer>();
        WeaponSR.sortingLayerName = "Player";
        WeaponEquipped = true;
        currentState.UpdateWeaponPosition();
    }

    // == MonoBehaviour functions ==========
    private void Awake()
    {
        // Set components references
        RB = GetComponent<Rigidbody2D>();
        IH = GetComponent<PlayerInputHandler>();
        Anim = GetComponent<Animator>();

        // States initialization
        IdleState = new PlayerIdleState(this);
        MoveState = new PlayerMoveState(this);
        SneakIdleState = new PlayerSneakIdleState(this);
        SneakMoveState = new PlayerSneakMoveState(this);
        AttackLightState = new PlayerAttackLightState(this);
        AttackHeavyState = new PlayerAttackHeavyState(this);
        DashState = new PlayerDashState(this);
    }

    private void Start()
    {
        LastMovementDirection = Direction.Down;

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

        // Cursor
        // - Interact
        if (IH.InteractAction.WasPressedThisFrame()) {
            RaycastHit2D hit = Physics2D.Raycast(cursorCoordinates, Vector2.zero, 0);
            if (hit) {
                var hitGO = hit.transform.gameObject;
                if (hitGO.TryGetComponent(out IRightClickable hitScript)) {
                    hitScript.OnRightClick(this);
                }
            }
        }

        // Debug
        //Debug.Log(Sneaking);
    }

    // Debug
    [Header("Debug")]
    public Vector2 gizmoCircleCenter;
    public float gizmoCircleRadius;
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(gizmoCircleCenter, gizmoCircleRadius);
    }
}
