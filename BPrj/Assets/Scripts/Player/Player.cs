using UnityEngine;

public class Player : MonoBehaviour
{
    // == Values set in the editor =========
    [field: SerializeField] public Transform Center { get; private set; } // Center of the character's sprite, pivot has to be at the sprite's feet for y-sorting to work    

    // == Component references =============
    private Rigidbody2D RB;
    public PlayerInputHandler IH { get; private set; }
    public Animator Anim { get; private set; }

    // == State machine ====================
    private PlayerState CurrentState { get; set; }
    public PlayerIdleState IdleState { get; private set; }
    public PlayerMoveState MoveState { get; private set; }
    public PlayerSneakIdleState SneakIdleState { get; private set; }
    public PlayerSneakMoveState SneakMoveState { get; private set; }

    public void ChangeState(PlayerState newState)
    {
        newState.Enter();
        CurrentState = newState;
    }

    // == Movement =========================
    private Vector2 movementInputTempVector; // Saving input to variable so we don't have to call new() every frame
    public int LastMovementDirection { get; set; } // 0 = up; 1 = right; 2 = down; 3 = left; used in IdleState.Enter() to set correct sprite

    public Vector2 GetNormalizedMovementInput()
    {
        movementInputTempVector.Set(IH.MovementX, IH.MovementY);
        return (movementInputTempVector.normalized);
    }

    public void SetVelocity(Vector2 velocity) => RB.velocity = velocity;

    // == Cursor position ==================
    private Vector3 cursorPosition;
    private Vector2 cursorCoordinates;

    public void UpdateCursorPositionAndCoordinates()
    {
        cursorPosition = Input.mousePosition;
        cursorPosition.z = Camera.main.nearClipPlane;
        cursorCoordinates = Camera.main.ScreenToWorldPoint(cursorPosition);
    }

    //public Vector2 GetPlayerToCursorDirection() => ((cursorCoordinates - (Vector2)this.transform.position).normalized);

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
        WeaponEquipped = true;
        CurrentState.UpdateWeaponPosition();
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
    }

    private void Start()
    {
        WeaponEquipped = false;

        LastMovementDirection = 2; // Facing down
        
        ChangeState(IdleState);
    }

    private void FixedUpdate()
    {
        // State logic
        CurrentState.FixedUpdate();
    }

    private void Update()
    {
        // State logic
        CurrentState.Update();

        // Update sub-functions
        UpdateCursorPositionAndCoordinates();

        // Cursor
        // - Right click
        if (Input.GetMouseButtonDown(1)) {
            RaycastHit2D hit = Physics2D.Raycast(cursorCoordinates, Vector2.zero, 0);
            if (hit) {
                var hitGO = hit.transform.gameObject;
                if (hitGO.TryGetComponent(out IRightClickable hitScript)) {
                    hitScript.OnRightClick(this);
                }
            }
        }
    }
}
