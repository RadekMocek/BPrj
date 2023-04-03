using UnityEngine;

public class PlayerKnockbackState : PlayerState
{
    public PlayerKnockbackState(Player player) : base(player)
    {
    }

    private readonly float knockbackDuration = 0.1f;
    private readonly float knockbackSpeed = 10;

    public override void Enter()
    {
        base.Enter();

        player.RB.velocity = knockbackSpeed * player.KnockbackDirection;
    }

    public override void Update()
    {
        base.Update();

        if (Time.time > enterTime + knockbackDuration) {
            player.RB.velocity = Vector2.zero;

            // ChangeState logic
            if (!player.IsSneaking) {
                if (movementInput.magnitude != 0) {
                    player.ChangeState(player.MoveState);
                }
                else {
                    player.ChangeState(player.IdleState);
                }
            }
            else {
                if (movementInput.magnitude != 0) {
                    player.ChangeState(player.SneakMoveState);
                }
                else {
                    player.ChangeState(player.SneakIdleState);
                }
            }
        }
    }
}
