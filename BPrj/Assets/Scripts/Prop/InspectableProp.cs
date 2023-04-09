using UnityEngine;

public class InspectableProp : MonoBehaviour, IPlayerInteractable
{
    // Inspectable prop displays a image on the HUD if inspected by the player

    [Header("Inspect")]
    [SerializeField] private Sprite imageToShow;

    private HUDManager HUD;

    public bool CanInteract(Player playerScript)
    {
        return (Vector2.Distance(playerScript.transform.position, this.transform.position) <= 2.5f);
    }

    public string GetInteractActionDescription(Player playerScript)
    {
        return "Prozkoumat";
    }

    public void OnInteract(Player playerScript)
    {
        HUD.ShowInspectImage(imageToShow, this);
    }

    private void Awake()
    {
        HUD = ManagerAccessor.instance.HUD;
    }
}
