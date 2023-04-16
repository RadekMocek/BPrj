using UnityEngine;

public class Key : MonoBehaviour, IPlayerInteractable
{
    // == IPlayerInteractable ===================
    public string GetInteractActionDescription(Player playerScript) => "Sebrat";

    public bool CanInteract(Player playerScript)
    {
        return (Vector2.Distance(playerScript.transform.position, this.transform.position) <= 1.5f);
    }

    public void OnInteract(Player playerScript)
    {
        playerScript.EquippedKeys.Add(color);
        ManagerAccessor.instance.ConsistencyManager.SetRecord(this.transform.name, false);
        ManagerAccessor.instance.HUD.ShowItem((int)color, true);
        Destroy(this.gameObject);
    }

    // == Lock, Key =============================
    [SerializeField] private LockColor color;
}
