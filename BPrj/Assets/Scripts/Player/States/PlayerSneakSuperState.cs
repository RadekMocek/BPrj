using UnityEngine;

public class PlayerSneakSuperState : PlayerState
{
    public PlayerSneakSuperState(Player player) : base(player)
    {
    }

    public override void Enter()
    {
        base.Enter();

        player.IsSneaking = true;
    }

    protected override void UpdateWeaponPositionInner()
    {
        if (player.LastMovementDirection == Direction.N) { // Up
            tempWeaponPosition.Set(.46f, .99f);
            tempWeaponRotation = Quaternion.Euler(0, -110, 0);
            player.WeaponSR.sortingOrder = -1;
        }
        else if (player.LastMovementDirection == Direction.E) { // Right
            tempWeaponPosition.Set(.04f, .97f);
            tempWeaponRotation = Quaternion.Euler(0, 180, 0);
            player.WeaponSR.sortingOrder = 1;
        }
        else if (player.LastMovementDirection == Direction.S) { // Down
            tempWeaponPosition.Set(-.30f, .87f);
            tempWeaponRotation = Quaternion.Euler(0, 115, 0);
            player.WeaponSR.sortingOrder = 1;
        }
        else if (player.LastMovementDirection == Direction.W) { // Left
            tempWeaponPosition.Set(-.35f, 1.02f);
            tempWeaponRotation = Quaternion.Euler(0, 0, 0);
            player.WeaponSR.sortingOrder = -1;
        }

        player.WeaponTransform.SetLocalPositionAndRotation(tempWeaponPosition, tempWeaponRotation);
    }
}
