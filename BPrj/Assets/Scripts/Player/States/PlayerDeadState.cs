using UnityEngine;

public class PlayerDeadState : PlayerState
{
    public PlayerDeadState(Player player) : base(player)
    {
    }

    private readonly float respawnDuration = 3;

    private bool respawnRequested;

    public override void Enter()
    {
        base.Enter();

        // Init
        respawnRequested = false;

        // Change corpse's collision rules
        player.gameObject.layer = LayerMask.NameToLayer("Player_Dead");
        
        // Make enemies ignore player's corpse
        ManagerAccessor.instance.EnemyManager.isPlayerDead = true;
    }

    public override void Update()
    {
        base.Update();

        // Stationary
        player.RB.velocity = Vector2.zero;

        // Respawn after respawnDuration passes
        if (!respawnRequested && Time.time > enterTime + respawnDuration) {
            respawnRequested = true;
            ManagerAccessor.instance.SceneManager.Respawn();
        }
    }

    public override void Exit()
    {
        base.Exit();

        player.ResetHealth();
        player.gameObject.layer = LayerMask.NameToLayer("Player");
        ManagerAccessor.instance.EnemyManager.isPlayerDead = false;
    }
}
