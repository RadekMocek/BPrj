using UnityEngine;

public class PlayerSneakMoveState : PlayerSneakSuperState
{
    public PlayerSneakMoveState(Player player) : base(player)
    {
    }

    private readonly float maxMovementSpeed = 3.3f;
    private readonly float initialMovementSpeed = 0.0f;
    private readonly int movementSpeedAcceleration = 18;

    private bool isPrimaryAxisVertical;
    private float movementSpeed;
    private Vector2 momentumDirection;
    private float momentumSpeed;

    public override void Enter()
    {
        base.Enter();

        isPrimaryAxisVertical = (movementInput.y != 0);

        movementSpeed = initialMovementSpeed;
    }

    public override void Update()
    {
        base.Update();

        // Animation logic
        if (isPrimaryAxisVertical) {
            if (movementInput.y > 0) {
                anim.CrossFade("Player_Sneak_Walk_Up", 0);
                player.LastMovementDirection = Direction.N;
            }
            else if (movementInput.y < 0) {
                anim.CrossFade("Player_Sneak_Walk_Down", 0);
                player.LastMovementDirection = Direction.S;
            }
            else {
                isPrimaryAxisVertical = false;
            }
        }
        else {
            if (movementInput.x < 0) {
                anim.CrossFade("Player_Sneak_Walk_Left", 0);
                player.LastMovementDirection = Direction.W;
            }
            else if (movementInput.x > 0) {
                anim.CrossFade("Player_Sneak_Walk_Right", 0);
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
            player.SneakIdleState.momentumDirection = momentumDirection;
            player.SneakIdleState.initialMomentumSpeed = momentumSpeed;
            player.ChangeState(player.SneakIdleState);
        }
        else if (sneakInputPressedThisFrame && !player.IsVenting()) {
            player.IsSneaking = false;
            player.ChangeState(player.MoveState);
        }
        else if (player.WeaponEquipped) {
            if (Input.GetMouseButtonDown(0) && player.CanAttack()) {
                player.ChangeState(player.AttackLightState);
            }
        }
    }
}
