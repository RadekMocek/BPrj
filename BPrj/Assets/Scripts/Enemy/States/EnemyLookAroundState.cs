using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class EnemyLookAroundState : EnemyState
{
    public EnemyLookAroundState(Enemy enemy) : base(enemy)
    {
    }

    protected bool End_PlayerLost { get; private set; }
    protected bool End_PlayerSpotted { get; private set; }

    private readonly float rotationPauseDuration = 1;
    private readonly int[] rotations = new int[] { 90, -90, -90, 90 };

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

        enemy.RB.velocity = Vector2.zero;

        if (enemy.IsPlayerVisible()) {
            End_PlayerSpotted = true;
        }

        //enemy.UpdateDecreaseViewConeRedRadius();

        if (Time.time > lastRotationTime + rotationPauseDuration) {
            lastRotationTime = Time.time;

            rotationsIndex++;

            if (true && rotationsIndex < nRotations) {
                targetRotation += rotations[rotationsIndex];
                //targetRotation += 90;

                if (targetRotation > 360) targetRotation -= 360;
                if (targetRotation < 0) targetRotation += 360;

                enemy.DirectionToFacingDirectionAndAnimation(enemy.FacingDirectionToDirection(targetRotation));
            }
            else {
                //End_PlayerLost = true;
                rotationsIndex = 0;
                Debug.Log("done");
            }
            
        }
    }
}
