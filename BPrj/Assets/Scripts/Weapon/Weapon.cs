using UnityEngine;

public class Weapon : MonoBehaviour, IInteractable
{
    private BoxCollider2D BC;

    public string GetInteractActionDescription() => "Sebrat zbraò";

    public bool CanInteract(Player playerScript)
    {
        return (Vector2.Distance(playerScript.transform.position, this.transform.position) <= 1.5f);
    }

    public void OnInteract(Player playerScript)
    {
        //if (!CanInteract(playerScript)) return;
        
        playerScript.EquipWeapon(this.gameObject);
        BC.enabled = false;
    }

    private void Awake()
    {
        BC = GetComponent<BoxCollider2D>();
    }

}
