using UnityEngine;

public class PlayerMoveState : PlayerState
{
    public PlayerMoveState(Player player) : base(player)
    {
    }

    private readonly float maxMovementSpeed = 4.7f;
    private readonly float initialMovementSpeed = 0.8f;
    private readonly int movementSpeedAcceleration = 22;

    private bool isPrimaryAxisVertical;
    private float movementSpeed;
    private Vector2 momentumDirection;
    private float momentumSpeed;
    //private string correctAnimationName;

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
                anim.CrossFade("Player_Walk_Up", 0);
                //correctAnimationName = "Player_Walk_Up";
                player.LastMovementDirection = Direction.Up;
            }
            else if (movementInput.y < 0) {
                anim.CrossFade("Player_Walk_Down", 0);
                //correctAnimationName = "Player_Walk_Down";
                player.LastMovementDirection = Direction.Down;
            }
            else {
                isPrimaryAxisVertical = false;
            }
        }
        else {
            if (movementInput.x < 0) {
                anim.CrossFade("Player_Walk_Left", 0);
                //correctAnimationName = "Player_Walk_Left";
                player.LastMovementDirection = Direction.Left;
            }
            else if (movementInput.x > 0) {
                anim.CrossFade("Player_Walk_Right", 0);
                //correctAnimationName = "Player_Walk_Right";
                player.LastMovementDirection = Direction.Right;
            }
            else {
                isPrimaryAxisVertical = true;
            }
        }
        /*
        if (correctAnimationName != anim.GetCurrentAnimatorClipInfo(0)[0].clip.name) {
            anim.CrossFade(correctAnimationName, 0);
        }
        */

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
