using UnityEngine;

public class Stairs : MonoBehaviour, IInteractable
{
    [Header("IInteractable")]
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private string interactActionDescription;
    [SerializeField] Vector2 interactZonePointA;
    [SerializeField] Vector2 interactZonePointB;

    private Vector2 tempVector;

    public bool CanInteract(Player playerScript)
    {
        tempVector = this.transform.position;
        return Physics2D.OverlapArea(interactZonePointA + tempVector, interactZonePointB + tempVector, playerLayer);
    }

    public string GetInteractActionDescription()
    {
        return ("→ " + interactActionDescription);
    }

    public void OnInteract(Player playerScript)
    {
        
    }

    private void OnDrawGizmosSelected()
    {
        GizmosHelper.DrawArea(interactZonePointA, interactZonePointB, this.transform.position);
    }
}
