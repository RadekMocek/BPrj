using UnityEngine;

public class PlayerAttackSuperState : PlayerState
{
    public PlayerAttackSuperState(Player player) : base(player)
    {
    }

    // Attack consist of motion on the circular sector until it reaches the angle corresponding to the cursor's location ("swing"), damage is then dealt in the near area.
    // Player is also being pushed a little in the attack's direction ("slip").
    // There is a slow little anticipation ("backSwing") prior to swinging and delay after the swing ("recovery")

    // Protected values set in SubStates
    protected float movementSpeed;

    protected float backSwingDuration;
    protected int backSwingSpeed;

    protected int swingCircularSectorAngle;
    protected int swingSpeed;
    protected float swingDistanceFromCore;    // Weapon distance from Player.Core while swinging
    protected float damageDistanceFromCore;   // Distance between Player.Core and center of damage dealing area
    protected float damageRadius;
    protected float slipSpeed;
    protected float recoveryDuration;

    private bool backswinging;
    private float angle;
    private float endingAngle;
    private int angleAdditionMultiplier;
    private Vector2 weaponRawPosition;
    private bool recovering;
    private float recoveryStartTime;

    public override void Enter()
    {
        base.Enter();

        // Init
        recovering = false;

        // Halt
        player.RB.velocity = Vector2.zero;

        // Calculate angle from player to cursor (this is also a z-rotation angle of the weapon)
        endingAngle = (int)(Mathf.Rad2Deg * Mathf.Atan2(playerToCursorDirection.y, playerToCursorDirection.x));
        angle = endingAngle - swingCircularSectorAngle;
        angleAdditionMultiplier = 1; // Counter clockwise

        // Change sprite so it faces the cursor (and attack direction), set slip direction
        string animationType = (player.IsSneaking) ? "Sneak_Idle" : "Idle";
        if (Mathf.Abs(playerToCursorDirection.x) > Mathf.Abs(playerToCursorDirection.y)) {
            // Right
            if (playerToCursorDirection.x > 0) {
                player.WeaponSR.sortingOrder = 1; // Sprite y-sorting
                anim.CrossFade($"Player_{animationType}_Right", 0);
                player.LastMovementDirection = Direction.E;
                // When facing right, it looks better if swing is done clockwise; angles have to be shifted and swopped
                (angle, endingAngle) = (endingAngle + swingCircularSectorAngle, angle + swingCircularSectorAngle);
                angleAdditionMultiplier = -1;
                player.WeaponSR.flipX = true; // Mirroring sprite instead of editing rotation on other axis to avoid gimbal lock and raw quaternions
                                              // Issue: this can be visible before the swing
            }
            // Left
            else {
                player.WeaponSR.sortingOrder = -1;
                anim.CrossFade($"Player_{animationType}_Left", 0);
                player.LastMovementDirection = Direction.W;
            }
        }
        else {
            // Up
            if (playerToCursorDirection.y > 0) {
                player.WeaponSR.sortingOrder = -1;
                anim.CrossFade($"Player_{animationType}_Up", 0);
                player.LastMovementDirection = Direction.N;
            }
            // Down
            else {
                player.WeaponSR.sortingOrder = 1;
                anim.CrossFade($"Player_{animationType}_Down", 0);
                player.LastMovementDirection = Direction.S;
            }
        }

        // Initial position
        ApplyPositionAndRotationAccordingToAngle();

        // Initial part of the attack
        backswinging = true;

        // Check critical hit
        if (player.IsCooldownBarVisible()) {
            Debug.Log("Critical hit");
        }
    }

    public override void Update()
    {
        base.Update();

        if (recovering) {
            // 3. RECOVERY
            angle += backSwingSpeed * Time.deltaTime * angleAdditionMultiplier;
            ApplyPositionAndRotationAccordingToAngle();
            if (Time.time > recoveryStartTime + recoveryDuration) {
                // Change state
                AttackEnd();
            }
        }
        else if (Time.time < enterTime + backSwingDuration) {
            // 1. BACKSWING
            player.RB.velocity = movementSpeed * movementInput;

            angle -= backSwingSpeed * Time.deltaTime * angleAdditionMultiplier;
            ApplyPositionAndRotationAccordingToAngle();
        }
        else {
            if (backswinging) {
                // Set lastAttackTime for attack cooldown
                player.criticalHitMissed = false;
                player.lastAttackTime = Time.time;
                backswinging = false;
            }
            // 2. SWING
            // Slip
            player.RB.velocity = playerToCursorDirection * slipSpeed;

            // Increase the angle, recalculate position, set position and rotation:
            angle += swingSpeed * Time.deltaTime * angleAdditionMultiplier;
            ApplyPositionAndRotationAccordingToAngle();

            // When we reach the endingAngle:
            if (angleAdditionMultiplier == 1 && angle >= endingAngle || angleAdditionMultiplier == -1 && angle <= endingAngle) {
                // Disable slip
                player.RB.velocity = Vector2.zero;

                ///* TEMP gizmos
                player.gizmoCircleCenter = (Vector2)player.transform.position + (weaponRawPosition * damageDistanceFromCore + (Vector2)player.Core.localPosition);
                player.gizmoCircleRadius = damageRadius;
                /**/

                // Deal damage to IDamageable
                var hits = Physics2D.OverlapCircleAll((Vector2)player.transform.position + (weaponRawPosition * damageDistanceFromCore + (Vector2)player.Core.localPosition), damageRadius);
                foreach (var hit in hits) {
                    if (hit.gameObject != player.gameObject && hit.TryGetComponent(out IDamageable hitScript)) {
                        hitScript.ReceiveDamage(playerToCursorDirection, 10);
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

        player.WeaponTransform.SetLocalPositionAndRotation(
            (weaponRawPosition * swingDistanceFromCore + (Vector2)player.Core.localPosition),
            (Quaternion.Euler(0, 0, angle - 90))
        );
    }

    private void AttackEnd()
    {
        // ChangeState logic
        if (!player.IsSneaking) {
            if (movementInput.magnitude != 0) {
                player.ChangeState(player.MoveState);
            }
            else {
                player.ChangeState(player.IdleState);
            }
        }
        else {
            if (movementInput.magnitude != 0) {
                player.ChangeState(player.SneakMoveState);
            }
            else {
                player.ChangeState(player.SneakIdleState);
            }
        }
    }

    public override void Exit()
    {
        base.Exit();

        // Unmirror the sprite in case attack direction was Right
        player.WeaponSR.flipX = false;
    }
}
