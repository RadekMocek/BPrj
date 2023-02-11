using UnityEngine;

public class PlayerIdleState : PlayerState
{
    public PlayerIdleState(Player player) : base(player)
    {
    }

    public override void Enter()
    {
        base.Enter();

        // Choose correct animation
        if (player.LastMovementDirection == 0) anim.CrossFade("Player_Idle_Up", 0);
        else if (player.LastMovementDirection == 1) anim.CrossFade("Player_Idle_Right", 0);
        else if (player.LastMovementDirection == 2) anim.CrossFade("Player_Idle_Down", 0);
        else if (player.LastMovementDirection == 3) anim.CrossFade("Player_Idle_Left", 0);

        // Choose correct weapon position
        UpdateWeaponPosition();
    }

    public override void Update()
    {
        base.Update();

        // ChangeState logic
        if (movementInput.magnitude != 0) {
            player.ChangeState(player.MoveState);
        }
        else if (sneakInputPressedThisFrame) {
            player.ChangeState(player.SneakIdleState);
        }
    }

}
