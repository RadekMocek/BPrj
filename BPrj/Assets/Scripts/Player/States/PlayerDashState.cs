using UnityEngine;

public class PlayerDashState : PlayerState
{
    public PlayerDashState(Player player) : base(player)
    {
    }

    private readonly int dashSpeed = 500;
    private readonly float dashDuration = .1f;

    public Vector2 dashDirection;

    public override void Enter()
    {
        base.Enter();

        player.RB.AddForce(dashSpeed * dashDirection);
    }

    public override void Update()
    {
        base.Update();

        if (Time.time > enterTime + dashDuration) {
            
            player.ChangeState(player.MoveState);
        }
    }
}
