using UnityEngine;

public class Player : MonoBehaviour
{
    // == Component references =============
    private Rigidbody2D RB { get; set; }
    private PlayerInputHandler IH { get; set; }
    public Animator Anim { get; private set; }

    // == State machine ====================
    private PlayerState CurrentState { get; set; }
    public PlayerIdleState IdleState { get; private set; }
    public PlayerMoveState MoveState { get; private set; }

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
        return movementInputTempVector.normalized;
    }

    public void SetVelocity(Vector2 velocity) => RB.velocity = velocity;

    // == Cursor position ==================
    private Vector3 cursorPosition;
    private Vector2 cursorCoordinates;
    public Vector2 GetPlayerToCursorDirection()
    {
        cursorPosition = Input.mousePosition;
        cursorPosition.z = Camera.main.nearClipPlane;
        cursorCoordinates = Camera.main.ScreenToWorldPoint(cursorPosition);

        return ((cursorCoordinates - (Vector2)this.transform.position).normalized);
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
    }

    private void Start()
    {
        LastMovementDirection = 2; // Facing down
        ChangeState(IdleState);
    }

    private void FixedUpdate()
    {
        CurrentState.FixedUpdate();
    }

    private void Update()
    {
        CurrentState.Update();
    }
}
