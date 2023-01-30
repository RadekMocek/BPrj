using UnityEngine;

public abstract class PlayerState
{
    // Player and its common properties
    protected Player player;
    protected Animator anim;

    protected Vector2 playerToCursorDirection;
    protected Vector2 movementInput;

    public PlayerState(Player player)
    {
        this.player = player;

        anim = player.Anim;
    }

    public /*virtual*/ void Enter()
    {
        Debug.Log($"Player changed to {this.GetType().Name}.");
    }

    public virtual void FixedUpdate()
    {

    }

    public virtual void Update()
    {
        playerToCursorDirection = player.GetPlayerToCursorDirection();
        movementInput = player.GetNormalizedMovementInput();
    }
}
