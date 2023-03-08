using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IObservable, IInteractable
{
    [SerializeField] private SpriteRenderer[] doorPartsSR;

    // == Observe ===============================
    public virtual string GetName()
    {
        return "Dve�e";
    }

    // == Interact =============================
    public string GetInteractActionDescription()
    {
        return "Otev��t";
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
