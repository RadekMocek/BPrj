using UnityEngine;

public class PlayerDialogueState : PlayerState
{
    public PlayerDialogueState(Player player) : base(player)
    {
    }

    public override void Enter()
    {
        base.Enter();

        // Choose correct animation
        if (player.LastMovementDirection == Direction.N) anim.CrossFade("Player_Idle_Up", 0);
        else if (player.LastMovementDirection == Direction.E) anim.CrossFade("Player_Idle_Right", 0);
        else if (player.LastMovementDirection == Direction.S) anim.CrossFade("Player_Idle_Down", 0);
        else if (player.LastMovementDirection == Direction.W) anim.CrossFade("Player_Idle_Left", 0);

        // Choose correct weapon position
        UpdateWeaponPosition();

        // Halt
        player.RB.velocity = Vector2.zero;
    }
}
