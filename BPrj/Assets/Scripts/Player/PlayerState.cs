using UnityEngine;

public abstract class PlayerState
{
    // Player and its components
    protected Player player;
    protected Animator anim;

    // Data from player used in particular states
    protected Vector2 playerToCursorDirection;
    protected Vector2 movementInput;    

    // Initialization
    public PlayerState(Player player)
    {
        this.player = player;
        anim = player.Anim;
    }

    // Called once every state change
    public virtual void Enter()
    {
        //Debug.Log($"Player changed to {this.GetType().Name}.");
    }

    // Called every physics update
    public virtual void FixedUpdate()
    {

    }

    // Called every frame
    public virtual void Update()
    {
        // Get player's data
        playerToCursorDirection = player.GetPlayerToCursorDirection();
        movementInput = player.GetNormalizedMovementInput();
    }
}
