using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IObservable, IInteractable
{
    [SerializeField] private SpriteRenderer[] doorPartsSR;

    // == Observe ===============================
    public virtual string GetName()
    {
        return "Dveøe";
    }

    // == Interact =============================
    public string GetInteractActionDescription()
    {
        return "Otevøít";
    }

    public bool CanInteract(Player playerScript)
    {
        return (Vector2.Distance(playerScript.transform.position, this.transform.position) <= 2.0f);
    }

    public void OnInteract(Player playerScript)
    {
        throw new System.NotImplementedException();
    }
}
