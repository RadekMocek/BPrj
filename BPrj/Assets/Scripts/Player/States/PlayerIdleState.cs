using UnityEngine;

public class PlayerIdleState : PlayerState
{
    public PlayerIdleState(Player player) : base(player)
    {
        momentumDirection = Vector2.zero;
    }

    private readonly int momentumSpeedDeceleration = 39;

    public float initialMomentumSpeed;
    public Vector2 momentumDirection;
    private float momentumSpeed;

    public override void Enter()
    {
        base.Enter();

        // Choose correct animation
        if (player.LastMovementDirection == Direction.Up) anim.CrossFade("Player_Idle_Up", 0);
        else if (player.LastMovementDirection == Direction.Right) anim.CrossFade("Player_Idle_Right", 0);
        else if (player.LastMovementDirection == Direction.Down) anim.CrossFade("Player_Idle_Down", 0);
        else if (player.LastMovementDirection == Direction.Left) anim.CrossFade("Player_Idle_Left", 0);

        // Choose correct weapon position
        UpdateWeaponPosition();

        // Momentum
        momentumSpeed = initialMomentumSpeed;
    }

    public override void Update()
    {
        base.Update();

        // Momentum
        player.RB.velocity = (momentumSpeed * momentumDirection);

        if (momentumSpeed > 0) {
            momentumSpeed -= momentumSpeedDeceleration * Time.deltaTime;
        }
        else {
            momentumSpeed = 0;
        }

        // ChangeState logic
        if (movementInput.magnitude != 0) {
            momentumDirection = Vector2.zero;
            player.ChangeState(player.MoveState);
        }
        else if (sneakInputPressedThisFrame) {
            momentumDirection = Vector2.zero;
            player.ChangeState(player.SneakIdleState);
        }
        else if (Input.GetMouseButtonDown(0) && player.WeaponEquipped) {
            momentumDirection = Vector2.zero;
            player.ChangeState(player.AttackState);
        }
    }

}
