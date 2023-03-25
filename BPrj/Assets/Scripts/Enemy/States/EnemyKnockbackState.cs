using UnityEngine;

public class EnemyKnockbackState : EnemyState
{
    public EnemyKnockbackState(Enemy enemy) : base(enemy)
    {
    }

    protected bool End_PlayerClose { get; private set; }
    protected bool End_PlayerVisible { get; private set; }
    protected bool End_PlayerLost { get; private set; }

    private readonly float knockbackDuration = 0.1f;
    private readonly float knockbackSpeed = 10;

    public override void Enter()
    {
        base.Enter();

        End_PlayerClose = false;
        End_PlayerVisible = false;
        End_PlayerLost = false;

        if (!enemy.IsDead) enemy.ChangeViewConeRedRadius(enemy.ViewDistance);

        enemy.RB.velocity = knockbackSpeed * enemy.KnockbackDirection;
    }

    public override void Update()
    {
        base.Update();

        if (Time.time > enterTime + knockbackDuration) {
            enemy.RB.velocity = Vector2.zero;
            enemy.lastKnownPlayerPosition = enemy.EnemyManager.GetPlayerPosition();

            // ChangeState logic
            if (enemy.IsPlayerVisibleClose()) {
                End_PlayerClose = true;
            }
            else if (enemy.IsPlayerVisible()) {
                End_PlayerVisible = true;
            }
            else {
                End_PlayerLost = true;
            }
        }
    }
}
