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
    public string GetInteractActionDescription(Player playerScript)
    {
        return (isLocked && !playerScript.EquippedKeys.Contains(lockColor)) ? "Chybí klíè" : "Otevøít";
    }

    public bool CanInteract(Player playerScript)
    {
        bool distanceOk = (Vector2.Distance(playerScript.transform.position, this.transform.position) <= 2.0f);
        bool lockOk = (!isLocked || (isLocked && playerScript.EquippedKeys.Contains(lockColor)));
        return distanceOk && lockOk;
    }

    public void OnInteract(Player playerScript)
    {
        StartCoroutine(Open());
        if (lockGO != null) {
            ManagerAccessor.instance.ConsistencyManager.SetRecord(lockGO.transform.name, false);
            Destroy(lockGO);
        }
    }

    // == Lock, Key =============================
    [Header("Lock, Key")]
    [SerializeField] private GameObject lockGO;
    [SerializeField] private LockColor lockColor;

    private bool isLocked;

    // == Opening ===============================
    private void Start()
    {
        isLocked = (lockGO != null);
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
    }
}
