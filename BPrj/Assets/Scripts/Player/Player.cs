using UnityEngine;

public class Player : MonoBehaviour
{
    // Component references
    private PlayerInputHandler IH { get; set; }
    public Animator Anim { get; private set; }

    // State machine
    private PlayerState CurrentState { get; set; }
    public PlayerIdleState IdleState { get; private set; }
    public PlayerMoveState MoveState { get; private set; }
    public void ChangeState(PlayerState newState)
    {
        newState.Enter();
        CurrentState = newState;
    }

    // Movement
    public Vector2 GetNormalizedMovementInput() => new Vector3(IH.MovementX, IH.MovementY).normalized;

    // Cursor position
    private Vector3 cursorPosition;
    private Vector2 cursorCoordinates;
    public Vector2 GetPlayerToCursorDirection()
    {
        cursorPosition = Input.mousePosition;
        cursorPosition.z = Camera.main.nearClipPlane;
        cursorCoordinates = Camera.main.ScreenToWorldPoint(cursorPosition);

        return ((cursorCoordinates - (Vector2)this.transform.position).normalized);
    }

    // MonoBehaviour functions
    private void Awake()
    {
        // Set components references
        IH = GetComponent<PlayerInputHandler>();
        Anim = GetComponent<Animator>();

        // States initialization
        IdleState = new PlayerIdleState(this);
        MoveState = new PlayerMoveState(this);
    }

    private void Start()
    {
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
