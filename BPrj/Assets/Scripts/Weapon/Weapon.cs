using UnityEngine;

public class Weapon : MonoBehaviour, IInteractable
{
    private BoxCollider2D BC;

    public string GetInteractActionDescription() => "Sebrat zbraò";

    public void OnInteract(Player playerScript)
    {
        var distanceFromPlayer = Vector2.Distance(playerScript.transform.position, this.transform.position);

        if (distanceFromPlayer <= 1.5f) {
            playerScript.EquipWeapon(this.gameObject);
            BC.enabled = false;
        }
    }

    private void Awake()
    {
        BC = GetComponent<BoxCollider2D>();
    }

}
