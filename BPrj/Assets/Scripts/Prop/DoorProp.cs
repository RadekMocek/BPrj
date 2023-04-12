using UnityEngine;

public class DoorProp : MonoBehaviour, IObservable, IPlayerInteractable
{
    public string GetName() => "Dve�e";
    public bool CanInteract(Player playerScript) => false;
    public string GetInteractActionDescription(Player playerScript) => "Nedostupn�";
    public void OnInteract(Player playerScript) { }
}
