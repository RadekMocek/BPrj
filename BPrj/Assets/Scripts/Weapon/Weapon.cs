using UnityEngine;

public class Weapon : MonoBehaviour, IPlayerInteractable
{
    // == Components ============================
    private BoxCollider2D BC;

    // == Equipped ==============================
    [HideInInspector] public bool equipped;

    // == IPlayerInteractable ===================
    public string GetInteractActionDescription() => "Sebrat zbraò";

    public bool CanInteract(Player playerScript)
    {
        return (Vector2.Distance(playerScript.transform.position, this.transform.position) <= 1.5f);
    }

    public void OnInteract(Player playerScript)
    {
        playerScript.EquipWeapon(this.gameObject);
        BC.enabled = false;
        equipped = true;
    }
 
    // == MonoBehaviour functions ===============
    private void Awake()
    {
        BC = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        equipped = false;
    }

    // == Static ================================
    // Weapon position and rotation for every facing direction
    private static Vector2 tempWeaponPosition;
    private static Quaternion tempWeaponRotation;
    public static (Vector2, Quaternion, int) GetCorrectWeaponPosition(Direction direction)
    {
        int sortingOrder;

        if (direction == Direction.N) {         // Up
            tempWeaponPosition.Set(.55f, .70f);
            tempWeaponRotation = Quaternion.Euler(-180, 75, -90);
            sortingOrder = -1;
        }
        else if (direction == Direction.E) {    // Right
            tempWeaponPosition.Set(.27f, .62f);
            tempWeaponRotation = Quaternion.Euler(180, 0, -90);
            sortingOrder = 1;
        }
        else if (direction == Direction.S) {    // Down
            tempWeaponPosition.Set(-.45f, .62f);
            tempWeaponRotation = Quaternion.Euler(-180, -110, -90);
            sortingOrder = 1;
        }
        else if (direction == Direction.W) {    // Left
            tempWeaponPosition.Set(-.50f, .72f);
            tempWeaponRotation = Quaternion.Euler(0, 0, 90);
            sortingOrder = -1;
        }
        else if (direction == Direction.NW) {   // Up-Left
            tempWeaponPosition.Set(-.35f, .88f);
            tempWeaponRotation = Quaternion.Euler(0, -58, 90);
            sortingOrder = -1;
        }
        else if (direction == Direction.SW) {   // Down-Left
            tempWeaponPosition.Set(-.56f, .78f);
            tempWeaponRotation = Quaternion.Euler(0, -305, 90);
            sortingOrder = -1;
        }
        else if (direction == Direction.SE) {   // Down-Right
            tempWeaponPosition.Set(-.15f, .75f);
            tempWeaponRotation = Quaternion.Euler(0, -245, 90);
            sortingOrder = 1;
        }
        else {                                  // Up-Right
            tempWeaponPosition.Set(.45f, .70f);
            tempWeaponRotation = Quaternion.Euler(0, -123, 90);
            sortingOrder = 1;
        }

        return (tempWeaponPosition, tempWeaponRotation, sortingOrder);
    }

}
