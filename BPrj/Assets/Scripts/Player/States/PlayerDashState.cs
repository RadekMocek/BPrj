using UnityEngine;

public class PlayerDashState : PlayerState
{
    public PlayerDashState(Player player) : base(player)
    {
    }

    private readonly int dashSpeed = 630;
    private readonly float dashDuration = .13f;
    private readonly float breatheOutDuration = .1f;

    public Vector2 dashDirection;

    private bool isBreathingOut;
    private float breatheOutStartTime;

    public override void Enter()
    {
        base.Enter();

        isBreathingOut = false;

        player.RB.AddForce(dashSpeed * dashDirection);
    }

    public override void Update()
    {
        base.Update();
        
        if (!isBreathingOut && Time.time > enterTime + dashDuration) {
            player.RB.velocity = Vector2.zero;
            breatheOutStartTime = Time.time;
            isBreathingOut = true;
        }
        else if (isBreathingOut && Time.time > breatheOutStartTime + breatheOutDuration) {
            if (movementInput.magnitude != 0) {
                player.ChangeState(player.MoveState);
            }
            else {
                player.ChangeState(player.IdleState);
            }
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if (!isBreathingOut) player.SpawnAfterImage();
    }
}
