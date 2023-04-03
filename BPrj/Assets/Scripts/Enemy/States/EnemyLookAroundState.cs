using UnityEngine;

public class EnemyLookAroundState : EnemyState
{
    public EnemyLookAroundState(Enemy enemy) : base(enemy)
    {
    }

    protected bool End_PlayerLost { get; private set; }
    protected bool End_PlayerSpotted { get; private set; }

    private readonly float rotationPauseDuration = .75f;
    private readonly int[] rotations = new int[] { 60, -60, -60, 60 };

    private int nRotations;
    private int rotationsIndex;
    private float lastRotationTime;
    private float targetRotation;

    public override void Enter()
    {
        base.Enter();

        End_PlayerLost = false;
        End_PlayerSpotted = false;

        nRotations = rotations.Length;
        rotationsIndex = 0;
        lastRotationTime = enterTime;
        targetRotation = enemy.TargetFacingDirection;
    }

    public override void Update()
    {
        base.Update();

        // Update weapon position
        UpdateWeaponPosition();

        // Halt
        enemy.RB.velocity = Vector2.zero;

        // Transition to another state if we spot the player
        if (enemy.IsPlayerVisible) {
            End_PlayerSpotted = true;
        }

        //enemy.UpdateDecreaseViewConeRedRadius();

        // Rotate according to angles stored in `rotations` every time `rotationPauseDuration` passes
        if (Time.time > lastRotationTime + rotationPauseDuration) {
            lastRotationTime = Time.time;

            if (rotationsIndex < nRotations) {
                targetRotation += rotations[rotationsIndex];

                if (targetRotation > 360) targetRotation -= 360;
                if (targetRotation < 0) targetRotation += 360;

                enemy.TargetFacingDirection = targetRotation;
                enemy.DirectionToAnimation(enemy.FacingDirectionToDirectionRound(targetRotation));

                rotationsIndex++;
            }
            else {
                End_PlayerLost = true;
            }
            
        }
    }
}
