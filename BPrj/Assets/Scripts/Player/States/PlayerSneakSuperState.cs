using UnityEngine;

public class PlayerSneakSuperState : PlayerState
{
    public PlayerSneakSuperState(Player player) : base(player)
    {
    }

    public override void Enter()
    {
        base.Enter();

        player.Sneaking = true;
    }

    protected override void UpdateWeaponPositionInner()
    {
        if (player.LastMovementDirection == Direction.Up) { // Up
            tempWeaponPosition.Set(.46f, .87f);
            tempWeaponRotation = Quaternion.Euler(0, -110, 0);
            player.WeaponSR.sortingOrder = -1;
        }
        else if (player.LastMovementDirection == Direction.Right) { // Right
            tempWeaponPosition.Set(.04f, .85f);
            tempWeaponRotation = Quaternion.Euler(0, 180, 0);
            player.WeaponSR.sortingOrder = 1;
        }
        else if (player.LastMovementDirection == Direction.Down) { // Down
            tempWeaponPosition.Set(-.30f, .75f);
            tempWeaponRotation = Quaternion.Euler(0, 115, 0);
            player.WeaponSR.sortingOrder = 1;
        }
        else if (player.LastMovementDirection == Direction.Left) { // Left
            tempWeaponPosition.Set(-.35f, .90f);
            tempWeaponRotation = Quaternion.Euler(0, 0, 0);
            player.WeaponSR.sortingOrder = -1;
        }

        player.WeaponTransform.SetLocalPositionAndRotation(tempWeaponPosition, tempWeaponRotation);
    }
}
