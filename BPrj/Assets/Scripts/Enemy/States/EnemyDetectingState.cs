using UnityEngine;

public class EnemyDetectingState : EnemyState
{
    public EnemyDetectingState(Enemy enemy) : base(enemy)
    {
        detectionSpeed = enemy.Data.DetectionSpeed;
    }

    protected bool End_PlayerLost { get; private set; }
    protected bool End_PlayerSpotted { get; private set; }

    protected float DetectionLengthWhenPlayerLost { get; private set; }

    private readonly float detectionSpeed; // SO

    private float fullDetectionDuration;
    private float enterDetectionLength;
    private float currentDetectionLength;

    public override void Enter()
    {
        base.Enter();

        End_PlayerLost = false;
        End_PlayerSpotted = false;

        enemy.ChangeViewConeColor(Color.yellow);
        enemy.FaceThePlayer(true);

        enterDetectionLength = enemy.CurrentDetectionLength;
        currentDetectionLength = enterDetectionLength;
        fullDetectionDuration = enemy.ViewDistance / detectionSpeed;
    }

    public override void Update()
    {
        base.Update();

        // Update weapon position
        UpdateWeaponPosition();

        // Halt
        enemy.RB.velocity = Vector2.zero;

        // Look directly at the player and change animation accordingly
        enemy.FaceThePlayer(true);

        // Transition to a another state if we lose the sight of the player
        if (!enemy.IsPlayerVisible) {
            DetectionLengthWhenPlayerLost = currentDetectionLength;
            enemy.lastKnownPlayerPosition = enemy.EnemyManager.GetPlayerPosition();
            End_PlayerLost = true;
        }
        // Player is spotted if red view cone "touches" them
        else if (currentDetectionLength >= enemy.GetEnemyToPlayerDistance()) {
            End_PlayerSpotted = true;
        }
        // Increase red view cone radius over time
        else if (currentDetectionLength < enemy.ViewDistance) {
            currentDetectionLength = (((Time.time - enterTime) / fullDetectionDuration) * enemy.ViewDistance) + enterDetectionLength;
            enemy.ChangeViewConeRedRadius(currentDetectionLength);
        }
    }
}
