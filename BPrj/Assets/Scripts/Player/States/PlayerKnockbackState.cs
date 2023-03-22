using UnityEngine;

public class PlayerKnockbackState : PlayerState
{
    public PlayerKnockbackState(Player player) : base(player)
    {
    }

    private readonly float knockbackDuration = 0.1f;
    private readonly float knockbackSpeed = 10;

    private Vector2 direction;

    public override void Enter()
    {
        base.Enter();

        direction = player.KnockbackDirection;

        player.RB.velocity = knockbackSpeed * direction;
    }

    public override void Update()
    {
        base.Update();

        if (Time.time > enterTime + knockbackDuration) {
            player.RB.velocity = Vector2.zero;

            // ChangeState logic
            if (!player.Sneaking) {
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
