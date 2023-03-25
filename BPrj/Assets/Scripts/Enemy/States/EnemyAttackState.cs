using UnityEngine;

public class EnemyAttackState : EnemyState
{
    public EnemyAttackState(Enemy enemy) : base(enemy)
    {
    }

    protected bool End_PlayerClose { get; private set; }
    protected bool End_PlayerVisible { get; private set; }
    protected bool End_PlayerLost { get; private set; }

    private readonly float backSwingDuration = 0.5f;
    private readonly int backSwingSpeed = 80;
    private readonly int swingCircularSectorAngle = 80;
    private readonly int swingSpeed = 1200;
    private readonly float swingDistanceFromCore = 1.0f;
    private readonly float damageDistanceFromCore = 0.9f;
    private readonly float damageRadius = 0.5f;
    private readonly float slipSpeed = 1.0f;
    private readonly float recoveryDuration = .25f;

    private readonly float movementSpeed = 3.0f;

    private bool backswinging;
    private Vector2 enemyToPlayerVector;
    private Vector2 enemyToPlayerVectorNormalized;
    private float angle;
    private float angleDiff;
    private float startingAngle;
    private float endingAngle;
    private Vector2 weaponRawPosition;
    private bool recovering;
    private float recoveryStartTime;

    private void InitializeSwing()
    {
        enemy.UpdatePlayerPositionInfo();

        enemyToPlayerVector = enemy.EnemyToPlayerVector;
        enemyToPlayerVectorNormalized = enemyToPlayerVector.normalized;

        endingAngle = enemy.EnemyToPlayerAngle - 270;
        if (endingAngle < 0) endingAngle += 360;

        startingAngle = endingAngle - swingCircularSectorAngle;

        Direction facingDirection = enemy.CurrentFacingDirectionAnimation;

        // Show weapon behind enemy for N/NW/W/SW directions
        enemy.WeaponSR.sortingOrder = (facingDirection <= Direction.SW) ? -1 : 1;
    }

    public override void Enter()
    {
        base.Enter();

        End_PlayerClose = false;
        End_PlayerVisible = false;
        End_PlayerLost = false;

        // Init
        backswinging = true;
        recovering = false;
        angleDiff = 0;

        // Halt
        enemy.RB.velocity = Vector2.zero;

        // Rotate to player
        enemy.FaceThePlayer(true);

        // Full red view cone
        enemy.ChangeViewConeColor(Color.yellow);
        enemy.ChangeViewConeRedRadius(enemy.ViewDistance);

        // Initialize swing – get player position, set angles, animation
        InitializeSwing();
        angle = startingAngle;
        ApplyPositionAndRotationAccordingToAngle();
    }

    public override void Update()
    {
        base.Update();

        enemy.FaceThePlayer(backswinging);

        if (recovering) {
            // 3. RECOVERY
            enemy.RB.velocity = Vector2.zero;
            angleDiff += backSwingSpeed * Time.deltaTime;
            angle = startingAngle + angleDiff;
            ApplyPositionAndRotationAccordingToAngle();
            if (Time.time > recoveryStartTime + recoveryDuration) {
                AttackEnd();
            }
        }
        else if (Time.time < enterTime + backSwingDuration) {
            // 1. BACKSWING
            angleDiff -= backSwingSpeed * Time.deltaTime;
            InitializeSwing();
            angle = startingAngle + angleDiff;
            ApplyPositionAndRotationAccordingToAngle();

            if (enemyToPlayerVector.magnitude > 1.5f) {
                enemy.RB.velocity = movementSpeed * enemyToPlayerVectorNormalized;
            }
            else {
                enemy.RB.velocity = Vector2.zero;
            }
        }
        else {
            if (backswinging) {
                backswinging = false;
                //TODO: Telegraph enemy attack here (?)
            }
            // 2. SWING
            // Slip
            enemy.RB.velocity = enemyToPlayerVectorNormalized * slipSpeed;

            // Increase the angle, recalculate position, set position and rotation:
            angleDiff += swingSpeed * Time.deltaTime;
            angle = startingAngle + angleDiff;
            ApplyPositionAndRotationAccordingToAngle();

            // When we reach the endingAngle:
            if (angle >= endingAngle) {
                ///* TEMP gizmos
                enemy.gizmoCircleCenter = (Vector2)enemy.transform.position + (weaponRawPosition * damageDistanceFromCore + (Vector2)enemy.Core.localPosition);
                enemy.gizmoCircleRadius = damageRadius;
                /**/

                // Deal damage to IDamageable
                var hits = Physics2D.OverlapCircleAll((Vector2)enemy.transform.position + (weaponRawPosition * damageDistanceFromCore + (Vector2)enemy.Core.localPosition), damageRadius);
                foreach (var hit in hits) {
                    if (hit.gameObject != enemy.gameObject && hit.TryGetComponent(out IDamageable hitScript)) {
                        hitScript.ReceiveDamage(enemyToPlayerVectorNormalized, 10);
                    }
                }

                // Transition to Recovery
                recovering = true;
                recoveryStartTime = Time.time;
            }
        }
    }

    private void ApplyPositionAndRotationAccordingToAngle()
    {
        weaponRawPosition.Set(/*x: */Mathf.Cos(Mathf.Deg2Rad * angle), /*y: */Mathf.Sin(Mathf.Deg2Rad * angle));

        enemy.WeaponTransform.SetLocalPositionAndRotation(
            (weaponRawPosition * swingDistanceFromCore + (Vector2)enemy.Core.localPosition),
            (Quaternion.Euler(0, 0, angle - 90))
        );
    }

    private void AttackEnd()
    {
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
