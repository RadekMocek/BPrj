using UnityEngine;

public class PlayerAttackState : PlayerState
{
    public PlayerAttackState(Player player) : base(player)
    {
    }

    // Attack consist of motion on the circular sector until it reaches the angle corresponding to the cursor's location ("swing"), damage is then dealt in the near area.
    // Player is also being pushed a little in the attack's direction ("slip").

    private readonly int swingCircularSectorAngle = 80;
    private readonly int swingSpeed = 600;
    private readonly float swingDistanceFromCore = 0.8f;    // Weapon distance from Player.Core while swinging
    private readonly float damageDistanceFromCore = 1.0f;   // Distance between Player.Core and center of damage dealing area
    private readonly float damageRadius = 0.6f;
    private readonly float slipSpeed = 1.0f;

    private float angle;
    private float endingAngle;
    private int angleAdditionMultiplier;
    private Vector2 weaponRawPosition;
    private Vector2 slipDirection;

    public override void Enter()
    {
        base.Enter();

        // Calculate angle from player to cursor (this is also a z-rotation angle of the weapon)
        endingAngle = (int)(Mathf.Rad2Deg * Mathf.Atan2(playerToCursorDirection.y, playerToCursorDirection.x));
        angle = endingAngle - swingCircularSectorAngle;
        angleAdditionMultiplier = 1; // Counter clockwise

        // Change sprite so it faces the cursor (and attack direction), set slip direction
        string animationType = (player.Sneaking) ? "Sneak_Idle" : "Idle";
        if (Mathf.Abs(playerToCursorDirection.x) > Mathf.Abs(playerToCursorDirection.y)) {
            // Right
            if (playerToCursorDirection.x > 0) {
                player.WeaponSR.sortingOrder = 1; // Sprite y-sorting
                anim.CrossFade($"Player_{animationType}_Right", 0);
                player.LastMovementDirection = Direction.Right;
                slipDirection.Set(1, 0);
                // When facing right, it looks better if swing is done clockwise; angles have to be shifted and swopped
                (angle, endingAngle) = (endingAngle + swingCircularSectorAngle, angle + swingCircularSectorAngle);
                angleAdditionMultiplier = -1;
                player.WeaponSR.flipX = true; // Mirroring sprite instead of editing rotation on other axis to avoid gimbal lock and raw quaternions;
                                              // Issue: this can be visible before the swing
            }
            // Left
            else {
                player.WeaponSR.sortingOrder = -1;
                anim.CrossFade($"Player_{animationType}_Left", 0);
                player.LastMovementDirection = Direction.Left;
                slipDirection.Set(-1, 0);
            }
        }
        else {
            // Up
            if (playerToCursorDirection.y > 0) {
                player.WeaponSR.sortingOrder = -1;
                anim.CrossFade($"Player_{animationType}_Up", 0);
                player.LastMovementDirection = Direction.Up;
                slipDirection.Set(0, 1);
                // Same deal as when attacking right
                (angle, endingAngle) = (endingAngle + swingCircularSectorAngle, angle + swingCircularSectorAngle);
                angleAdditionMultiplier = -1;
                player.WeaponSR.flipX = true;
            }
            // Down
            else {
                player.WeaponSR.sortingOrder = 1;
                anim.CrossFade($"Player_{animationType}_Down", 0);
                player.LastMovementDirection = Direction.Down;
                slipDirection.Set(0, -1);
            }
        }

        // Slip
        player.RB.velocity = slipDirection * slipSpeed;
    }
    
    //FixedUpdate() => transform.RotateAround(target.position, new Vector3(0, 0, 1), speed); // (?)

    public override void Update()
    {
        base.Update();

        // Increase the angle, recalculate position, set position and rotation:
        angle += swingSpeed * Time.deltaTime * angleAdditionMultiplier;
        
        weaponRawPosition.Set(/*x*/Mathf.Cos(Mathf.Deg2Rad * angle), /*y*/Mathf.Sin(Mathf.Deg2Rad * angle));

        player.WeaponTransform.SetLocalPositionAndRotation(
            (weaponRawPosition * swingDistanceFromCore + (Vector2)player.Core.localPosition),
            (Quaternion.Euler(0, 0, angle - 90))
        );
        
        // When we reach the endingAngle:
        if (angleAdditionMultiplier == 1 && angle >= endingAngle || angleAdditionMultiplier == -1 && angle <= endingAngle) {
            // Disable slip
            player.RB.velocity = Vector2.zero;

            // TEMP
            //player.gizmoCircleCenter = (Vector2)player.transform.position + (weaponRawPosition * damageDistanceFromCore + (Vector2)player.Core.localPosition);
            //player.gizmoCircleRadius = damageRadius;
            // ----

            // Deal damage to IDamageable
            var hits = Physics2D.OverlapCircleAll((Vector2)player.transform.position + (weaponRawPosition * damageDistanceFromCore + (Vector2)player.Core.localPosition), damageRadius); //TODO: p�idat layermask?
            foreach (var hit in hits) {
                if (hit.TryGetComponent(out IDamageable hitScript)) {
                    hitScript.ReceiveDamage();
                }
            }

            // Unmirror the sprite in case attack direction was right or up
            player.WeaponSR.flipX = false;

            // ChangeState logic
            if (!player.Sneaking)
                player.ChangeState(player.IdleState);
            else
                player.ChangeState(player.SneakIdleState);
        }
    }
}
