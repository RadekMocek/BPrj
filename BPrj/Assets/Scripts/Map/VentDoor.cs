using System.Collections;
using UnityEngine;

public class VentDoor : MonoBehaviour, IObservable, IPlayerInteractable
{
    [SerializeField] private BoxCollider2D BC;

    // == Observe ===============================
    public virtual string GetName()
    {
        return "Ventilace";
    }

    // == Interact =============================
    public string GetInteractActionDescription()
    {
        return "Otevøít";
    }

    public bool CanInteract(Player playerScript)
    {
        return (!isOpening && Vector2.Distance(playerScript.transform.position, this.transform.position) <= 2.0f);
    }

    public void OnInteract(Player playerScript)
    {
        isOpening = true;
        StartCoroutine(Open());
    }

    // == Opening ===============================
    private bool isOpening;
    //public bool IsOpened { get; private set; }

    private void Start()
    {
        isOpening = false;
        //IsOpened = false;
    }

    private IEnumerator Open()
    {
        BC.enabled = false;
        Vector2 localPosition = this.transform.localPosition;
        float localX = localPosition.x;
        float localY = localPosition.y;
        while (localY <= 1.3f) {
            localY += 0.1f;
            this.transform.localPosition = new Vector2(localX, localY);
            yield return new WaitForFixedUpdate();
        }
        //IsOpened = true;
    }
}
