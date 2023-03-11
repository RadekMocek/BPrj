using UnityEngine;

public class PlayerDashState : PlayerState
{
    public PlayerDashState(Player player) : base(player)
    {
    }

    // == Dash ==
    private readonly int dashSpeed = 660;
    private readonly float dashDuration = .14f;
    private readonly float breatheOutDuration = .1f;

    public Vector2 dashDirection;

    private bool isBreathingOut;
    private float breatheOutStartTime;

    // == After image ==
    private readonly float firstAfterImageSpawnDelay = .06f;
    private readonly float nextAfterImageSpawnDelay = .015f;

    private float lastAfterImageSpawnTime;
    private Sprite dashAfterImageSprite;
    
    //
    public override void Enter()
    {
        base.Enter();

        lastAfterImageSpawnTime = Time.time + firstAfterImageSpawnDelay;
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
        // Spawn after image
        else if (!isBreathingOut && Time.time > lastAfterImageSpawnTime + nextAfterImageSpawnDelay) {
            lastAfterImageSpawnTime = Time.time;
            player.SpawnAfterImage(dashAfterImageSprite);
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
}
