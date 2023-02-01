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
        #region Look in cursor's direction
        /*
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
        */
        #endregion
        if (movementInput.y > 0) {
            anim.CrossFade("Player_Walk_Up", 0);
            player.LastMovementDirection = 0;
        }
        else if (movementInput.y < 0) {
            anim.CrossFade("Player_Walk_Down", 0);
            player.LastMovementDirection = 2;
        }
        else if (movementInput.x < 0) {
            anim.CrossFade("Player_Walk_Left", 0);
            player.LastMovementDirection = 3;
        }
        else if (movementInput.x > 0) {
            anim.CrossFade("Player_Walk_Right", 0);
            player.LastMovementDirection = 1;
        }

        // Movement logic
        //player.transform.Translate(movementSpeed * Time.deltaTime * movementInput);
        player.SetVelocity(movementSpeed * movementInput);

        // ChangeState logic
        if (movementInput.magnitude == 0) {
            player.ChangeState(player.IdleState);
        }
    }
}
