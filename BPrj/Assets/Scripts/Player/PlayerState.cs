using UnityEngine;

public abstract class PlayerState
{
    // Player and its components
    protected Player player;
    protected Animator anim;

    // Data from player/input used in particular states
    //protected Vector2 playerToCursorDirection;
    protected Vector2 movementInput;
    protected bool sneakInputPressedThisFrame;

    // Helpful variables
    protected Vector2 tempWeaponPosition;
    protected Quaternion tempWeaponRotation;

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
        //playerToCursorDirection = player.GetPlayerToCursorDirection();
        movementInput = player.GetNormalizedMovementInput();
        sneakInputPressedThisFrame = player.IH.SneakAction.WasPressedThisFrame();
    }

    // Update equipped weapon position, rotation, order in layer
    public virtual void UpdateWeaponPosition()
    {
        if (player.WeaponEquipped) UpdateWeaponPositionInner();
    }

    protected virtual void UpdateWeaponPositionInner()
    {
        if (player.LastMovementDirection == 0) { // Up
            tempWeaponPosition.Set(.55f, .58f);
            tempWeaponRotation = Quaternion.Euler(-180, 75, -90);
            player.WeaponSR.sortingOrder = -1;
        }
        else if (player.LastMovementDirection == 1) { // Right
            tempWeaponPosition.Set(.27f, .50f);
            tempWeaponRotation = Quaternion.Euler(180, 0, -90);
            player.WeaponSR.sortingOrder = 1;
        }
        else if (player.LastMovementDirection == 2) { // Down
            tempWeaponPosition.Set(-.45f, .50f);
            tempWeaponRotation = Quaternion.Euler(-180, -110, -90);
            player.WeaponSR.sortingOrder = 1;
        }
        else if (player.LastMovementDirection == 3) { // Left
            tempWeaponPosition.Set(-.50f, .60f);
            tempWeaponRotation = Quaternion.Euler(0, 0, 90);
            player.WeaponSR.sortingOrder = -1;
        }

        player.WeaponTransform.SetLocalPositionAndRotation(tempWeaponPosition, tempWeaponRotation);
    }
}
