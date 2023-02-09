using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSneakMoveState : PlayerState
{
    public PlayerSneakMoveState(Player player) : base(player)
    {
    }

    private float movementSpeed = 3.0f;

    public override void Update()
    {
        base.Update();

        // Animation logic
        if (movementInput.y > 0) {
            anim.CrossFade("Player_Sneak_Walk_Up", 0);
            player.LastMovementDirection = 0;
        }
        else if (movementInput.y < 0) {
            anim.CrossFade("Player_Sneak_Walk_Down", 0);
            player.LastMovementDirection = 2;
        }
        else if (movementInput.x < 0) {
            anim.CrossFade("Player_Sneak_Walk_Left", 0);
            player.LastMovementDirection = 3;
        }
        else if (movementInput.x > 0) {
            anim.CrossFade("Player_Sneak_Walk_Right", 0);
            player.LastMovementDirection = 1;
        }

        // Movement logic
        player.SetVelocity(movementSpeed * movementInput);

        // ChangeState logic
        if (movementInput.magnitude == 0) {
            player.ChangeState(player.SneakIdleState);
        }
        else if (sneakInputPressedThisFrame) {
            player.ChangeState(player.MoveState);
        }
    }
}
