using UnityEngine;

public class PlayerMoveState : PlayerState
{
    public PlayerMoveState(Player player) : base(player)
    {
        isPrimaryAxisHorizontalMemory = false;
    }

    private readonly float maxMovementSpeed = 4.7f;
    private readonly float initialMovementSpeed = 0.8f;
    private readonly int movementSpeedAcceleration = 22;

    private bool isPrimaryAxisVertical;
    private float movementSpeed;
    private Vector2 momentumDirection;
    private float momentumSpeed;
    private bool isPrimaryAxisHorizontalMemory; // Do not change animation when dashing diagonally and sprite is facing left/right

    public override void Enter()
    {
        base.Enter();

        isPrimaryAxisVertical = (movementInput.y != 0);
        
        if (isPrimaryAxisHorizontalMemory) {
            isPrimaryAxisVertical = false;
            isPrimaryAxisHorizontalMemory = false; // So it's not true when changing from some other state than DashState
        }

        movementSpeed = initialMovementSpeed;
    }

    public override void Update()
    {
        base.Update();
        
        // Animation logic
        if (isPrimaryAxisVertical) {
            if (movementInput.y > 0) {
                anim.CrossFade("Player_Walk_Up", 0);
                player.LastMovementDirection = Direction.N;
            }
            else if (movementInput.y < 0) {
                anim.CrossFade("Player_Walk_Down", 0);
                player.LastMovementDirection = Direction.S;
            }
            else {
                isPrimaryAxisVertical = false;
            }
        }
        else {
            if (movementInput.x < 0) {
                anim.CrossFade("Player_Walk_Left", 0);
                player.LastMovementDirection = Direction.W;
            }
            else if (movementInput.x > 0) {
                anim.CrossFade("Player_Walk_Right", 0);
                player.LastMovementDirection = Direction.E;
            }
            else {
                isPrimaryAxisVertical = true;
            }
        }

        // Update weapon position according to movement direction
        UpdateWeaponPosition();

        // Movement logic
        player.RB.velocity = (movementSpeed * movementInput);

        // Acceleration
        if (movementSpeed < maxMovementSpeed) {
            movementSpeed += movementSpeedAcceleration * Time.deltaTime;
        }
        else {
            movementSpeed = maxMovementSpeed;
        }

        // Momentum
        if (movementInput.magnitude != 0) {
            momentumDirection = movementInput;
            momentumSpeed = movementSpeed;
        }

        // ChangeState logic
        if (movementInput.magnitude == 0) {
            player.IdleState.momentumDirection = momentumDirection;
            player.IdleState.initialMomentumSpeed = momentumSpeed;
            player.ChangeState(player.IdleState);
        }
        else if (sneakInputPressedThisFrame) {
            player.ChangeState(player.SneakMoveState);
        }
        else if (dashInputPressedThisFrame) {
            isPrimaryAxisHorizontalMemory = !isPrimaryAxisVertical;

            player.DashState.dashDirection = movementInput;
            player.ChangeState(player.DashState);
        }
        else if (player.WeaponEquipped) {
            if (Input.GetMouseButtonDown(0)) {
                player.ChangeState(player.AttackLightState);
            }
            else if (Input.GetMouseButtonDown(1)) {
                player.ChangeState(player.AttackHeavyState);
            }
        }
        
    }
}
