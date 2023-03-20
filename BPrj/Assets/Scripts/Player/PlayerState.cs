using UnityEngine;

public abstract class PlayerState
{
    // Player and its components
    protected Player player;
    protected Animator anim;

    // Data from player/input used in particular states
    protected Vector2 playerToCursorDirection;
    protected Vector2 movementInput;
    protected bool sneakInputPressedThisFrame;
    protected bool dashInputPressedThisFrame;

    // Helpful variables
    protected float enterTime;
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
        enterTime = Time.time;
        UpdatePlayerData();
    }

    // Called every physics update
    public virtual void FixedUpdate()
    {

    }

    // Called every frame
    public virtual void Update()
    {
        UpdatePlayerData();        
    }

    // Update data from player
    private void UpdatePlayerData()
    {
        playerToCursorDirection = player.GetPlayerCoreToCursorDirection();
        movementInput = player.GetNormalizedMovementInput();
        sneakInputPressedThisFrame = player.IH.SneakAction.WasPressedThisFrame();
        dashInputPressedThisFrame = player.IH.DashAction.WasPressedThisFrame();
    }

    // Update equipped weapon position, rotation, order in layer
    public virtual void UpdateWeaponPosition()
    {
        if (player.WeaponEquipped) UpdateWeaponPositionInner();
    }

    protected virtual void UpdateWeaponPositionInner()
    {
        var pos = Weapon.GetCorrectWeaponPosition(player.LastMovementDirection);
        player.WeaponSR.sortingOrder = pos.Item3;
        player.WeaponTransform.SetLocalPositionAndRotation(pos.Item1, pos.Item2);
    }
}
