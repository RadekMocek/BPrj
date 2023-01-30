using UnityEngine;

public class PlayerIdleState : PlayerState
{
    public PlayerIdleState(Player player) : base(player)
    {
    }

    public override void Update()
    {
        base.Update();

        // Animation logic
        if (Mathf.Abs(playerToCursorDirection.x) > Mathf.Abs(playerToCursorDirection.y)) {
            if (playerToCursorDirection.x > 0) {
                anim.CrossFade("Player_Idle_Right", 0);
            }
            else {
                anim.CrossFade("Player_Idle_Left", 0);
            }
        }
        else {
            if (playerToCursorDirection.y > 0) {
                anim.CrossFade("Player_Idle_Up", 0);
            }
            else {
                anim.CrossFade("Player_Idle_Down", 0);
            }
        }

        // ChangeState logic
        if (movementInput.magnitude != 0) {
            player.ChangeState(player.MoveState);
        }
    }
}
