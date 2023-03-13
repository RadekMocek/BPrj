using UnityEngine;

public class EnemyDetectingState : EnemyState
{
    public EnemyDetectingState(Enemy enemy) : base(enemy)
    {
    }

    protected bool End_PlayerLost { get; private set; }
    protected bool End_PlayerSpotted { get; private set; }

    private readonly float detectionSpeed = 6;

    private float fullDetectionDuration;
    private float currentDetectionLength;

    public override void Enter()
    {
        base.Enter();

        End_PlayerLost = false;
        End_PlayerSpotted = false;

        enemy.ChangeViewConeColor(Color.yellow);
        enemy.FaceThePlayer();

        currentDetectionLength = 0;
        fullDetectionDuration = enemy.viewDistance / detectionSpeed;

        enemy.ChangeViewConeRedRadius(0);
        enemy.ShowViewConeRed(true);
    }

    public override void Update()
    {
        base.Update();

        enemy.RB.velocity = Vector2.zero;

        enemy.FaceThePlayer();

        if (!enemy.IsPlayerVisible()) {
            enemy.ShowViewConeRed(false);
            End_PlayerLost = true;
        }
        else if (currentDetectionLength >= enemy.GetEnemyToPlayerDistance()) {
            enemy.ChangeViewConeRedRadius(enemy.viewDistance);
            End_PlayerSpotted = true;
        }
        else if (currentDetectionLength < enemy.viewDistance) {
            currentDetectionLength = ((Time.time - enterTime) / fullDetectionDuration) * enemy.viewDistance;
            enemy.ChangeViewConeRedRadius(currentDetectionLength);
        }
    }
}
