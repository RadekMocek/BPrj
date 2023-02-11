using UnityEngine;

public class PlayerSneakSuperState : PlayerState
{
    public PlayerSneakSuperState(Player player) : base(player)
    {
    }

    protected override void UpdateWeaponPositionInner()
    {
        if (player.LastMovementDirection == 0) { // Up
            tempWeaponPosition.Set(.46f, .87f);
            tempWeaponRotation = Quaternion.Euler(0, -110, 0);
            player.WeaponSR.sortingOrder = -1;
        }
        else if (player.LastMovementDirection == 1) { // Right
            tempWeaponPosition.Set(.04f, .85f);
            tempWeaponRotation = Quaternion.Euler(0, 180, 0);
            player.WeaponSR.sortingOrder = 1;
        }
        else if (player.LastMovementDirection == 2) { // Down
            tempWeaponPosition.Set(-.30f, .75f);
            tempWeaponRotation = Quaternion.Euler(0, 115, 0);
            player.WeaponSR.sortingOrder = 1;
        }
        else if (player.LastMovementDirection == 3) { // Left
            tempWeaponPosition.Set(-.35f, .90f);
            tempWeaponRotation = Quaternion.Euler(0, 0, 0);
            player.WeaponSR.sortingOrder = -1;
        }

        player.WeaponTransform.SetLocalPositionAndRotation(tempWeaponPosition, tempWeaponRotation);
    }
}
