using UnityEngine;

public class EnemyAttackState : EnemyState
{
    public EnemyAttackState(Enemy enemy) : base(enemy)
    {
    }

    protected bool End_PlayerClose { get; private set; }
    protected bool End_PlayerVisible { get; private set; }
    protected bool End_PlayerLost { get; private set; }

    private readonly float backSwingDuration = 0.3f;
    private readonly int backSwingSpeed = 80;
    private readonly int swingCircularSectorAngle = 80;
    private readonly int swingSpeed = 1000;
    private readonly float swingDistanceFromCore = 0.8f;
    private readonly float damageDistanceFromCore = 1.0f;
    private readonly float damageRadius = 0.6f;
    private readonly float slipSpeed = 1.0f;
    private readonly float recoveryDuration = .15f;

    private Vector2 enemyToPlayerVector;
    private float angle;
    private float endingAngle;
    private int angleAdditionMultiplier;
    private Vector2 weaponRawPosition;
    private bool recovering;
    private float recoveryStartTime;

    public override void Enter()
    {
        base.Enter();

        End_PlayerClose = false;
        End_PlayerVisible = false;
        End_PlayerLost = false;

        // Init
        recovering = false;
        enemy.RB.velocity = Vector2.zero;

        enemy.UpdatePlayerPositionInfo();

        enemyToPlayerVector = enemy.EnemyToPlayerVector.normalized;

        endingAngle = enemy.EnemyToPlayerAngle - 270;
        if (endingAngle < 0) endingAngle += 360;

        angle = endingAngle - swingCircularSectorAngle;

        enemy.FaceThePlayer(true);

        Direction facingDirection = enemy.CurrentFacingDirectionAnimation;

        enemy.WeaponSR.sortingOrder = (facingDirection <= Direction.SW) ? -1 : 1;

        if (facingDirection >= Direction.SE) {
            (angle, endingAngle) = (endingAngle + swingCircularSectorAngle, angle + swingCircularSectorAngle);
            angleAdditionMultiplier = -1;
            enemy.WeaponSR.flipX = true;
        }
        else {
            angleAdditionMultiplier = 1;
        }

        ApplyPositionAndRotationAccordingToAngle();
    }

    public override void Update()
    {
        base.Update();

        enemy.FaceThePlayer(false);
        enemy.RB.velocity = Vector2.zero;

        if (recovering) {
            // 3. RECOVERY
            angle += backSwingSpeed * Time.deltaTime * angleAdditionMultiplier;
            ApplyPositionAndRotationAccordingToAngle();
            if (Time.time > recoveryStartTime + recoveryDuration) {
                AttackEnd();
            }
        }
        else if (Time.time < enterTime + backSwingDuration) {
            // 1. BACKSWING
            angle -= backSwingSpeed * Time.deltaTime * angleAdditionMultiplier;
            ApplyPositionAndRotationAccordingToAngle();
        }
        else {
            // 2. SWING
            // Slip
            enemy.RB.velocity = enemyToPlayerVector * slipSpeed;

            // Increase the angle, recalculate position, set position and rotation:
            angle += swingSpeed * Time.deltaTime * angleAdditionMultiplier;
            ApplyPositionAndRotationAccordingToAngle();

            // When we reach the endingAngle:
            if (angleAdditionMultiplier == 1 && angle >= endingAngle || angleAdditionMultiplier == -1 && angle <= endingAngle) {
                ///* TEMP gizmos
                enemy.gizmoCircleCenter = (Vector2)enemy.transform.position + (weaponRawPosition * damageDistanceFromCore + (Vector2)enemy.Core.localPosition);
                enemy.gizmoCircleRadius = damageRadius;
                /**/

                // Deal damage to IDamageable
                var hits = Physics2D.OverlapCircleAll((Vector2)enemy.transform.position + (weaponRawPosition * damageDistanceFromCore + (Vector2)enemy.Core.localPosition), damageRadius);
                foreach (var hit in hits) {
                    if (hit.gameObject != enemy.gameObject && hit.TryGetComponent(out IDamageable hitScript)) {
                        hitScript.ReceiveDamage(enemyToPlayerVector);
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
        enemy.WeaponSR.flipX = false;

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
