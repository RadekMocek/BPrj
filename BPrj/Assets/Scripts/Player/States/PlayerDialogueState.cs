using UnityEngine;

public class PlayerDialogueState : PlayerState
{
    public PlayerDialogueState(Player player) : base(player)
    {
    }

    public Direction facingDirection;

    public override void Enter()
    {
        base.Enter();

        ///*
        // Choose correct animation
        if (facingDirection == Direction.N) anim.CrossFade("Player_Idle_Up", 0);
        else if (facingDirection == Direction.E) anim.CrossFade("Player_Idle_Right", 0);
        else if (facingDirection == Direction.S) anim.CrossFade("Player_Idle_Down", 0);
        else if (facingDirection == Direction.W) anim.CrossFade("Player_Idle_Left", 0);
        /**/

        /*
        // Face down
        anim.CrossFade("Player_Idle_Down", 0);
        /**/

        // Choose correct weapon position
        UpdateWeaponPosition();

        // Halt
        player.RB.velocity = Vector2.zero;
    }
}
