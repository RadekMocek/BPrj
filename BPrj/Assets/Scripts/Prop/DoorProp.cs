using UnityEngine;

public class DoorProp : MonoBehaviour, IObservable, IPlayerInteractable
{
    public string GetName() => "Dveøe";
    public bool CanInteract(Player playerScript) => false;
    public string GetInteractActionDescription(Player playerScript) => "Nedostupné";
    public void OnInteract(Player playerScript) { }
}
