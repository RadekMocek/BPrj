using UnityEngine;

public class PlayerDashState : PlayerState
{
    public PlayerDashState(Player player) : base(player)
    {
    }

    private readonly int dashSpeed = 660;
    private readonly float dashDuration = .14f;
    private readonly float breatheOutDuration = .1f;

    public Vector2 dashDirection;

    private bool isBreathingOut;
    private float breatheOutStartTime;
    private Sprite dashAfterImageSprite;

    public override void Enter()
    {
        base.Enter();

        dashAfterImageSprite = player.SR.sprite;

        isBreathingOut = false;

        player.RB.AddForce(dashSpeed * dashDirection);
    }

    public override void Update()
    {
        base.Update();
        // Halt after dashDuration elapses
        if (!isBreathingOut && Time.time > enterTime + dashDuration) {
            player.RB.velocity = Vector2.zero;
            breatheOutStartTime = Time.time;
            isBreathingOut = true;
        }
        // ChangeState after breatheOutDuration elapses
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

        if (!isBreathingOut) player.SpawnAfterImage(dashAfterImageSprite);
    }
}
