using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : PlayerState
{
    public PlayerMoveState(Player player) : base(player)
    {
    }

    private float movementSpeed = 3.0f;

    public override void Update()
    {
        base.Update();

        // Animation logic
        if (Mathf.Abs(playerToCursorDirection.x) > Mathf.Abs(playerToCursorDirection.y)) {
            if (playerToCursorDirection.x > 0) {
                anim.CrossFade("Player_Walk_Right", 0);
            }
            else {
                anim.CrossFade("Player_Walk_Left", 0);
            }
        }
        else {
            if (playerToCursorDirection.y > 0) {
                anim.CrossFade("Player_Walk_Up", 0);
            }
            else {
                anim.CrossFade("Player_Walk_Down", 0);
            }
        }

        // Movement logic
        player.transform.Translate(movementSpeed * Time.deltaTime * movementInput);

        // ChangeState logic
        if (movementInput.magnitude == 0) {
            player.ChangeState(player.IdleState);
        }
    }
}
