using UnityEngine;

public class Stairs : MonoBehaviour, IPlayerInteractable
{
    [Header("IInteractable")]
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private string interactActionDescription;
    [SerializeField] Vector2 interactZonePointA;
    [SerializeField] Vector2 interactZonePointB;

    [Header("Stairs")]
    [SerializeField] private string sceneName;
    [SerializeField] private float playerX;
    [SerializeField] private float playerY;

    private Vector2 tempVector;

    public bool CanInteract(Player playerScript)
    {
        tempVector = this.transform.position;
        return !string.IsNullOrEmpty(sceneName) && Physics2D.OverlapArea(interactZonePointA + tempVector, interactZonePointB + tempVector, playerLayer);
    }

    public string GetInteractActionDescription(Player playerScript)
    {
        return (!string.IsNullOrEmpty(sceneName)) ? ("→ " + interactActionDescription) : ("Nedostupné");
    }

    public void OnInteract(Player playerScript)
    {
        ManagerAccessor.instance.SceneManager.ChangeScene(sceneName, playerX, playerY);
    }

    private void OnDrawGizmosSelected()
    {
        GizmosHelper.DrawArea(interactZonePointA, interactZonePointB, this.transform.position);
    }
}
