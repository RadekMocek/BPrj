using UnityEngine;

public class Flash : MonoBehaviour, IPlayerInteractable
{
    [SerializeField] private GameObject triggerPrefab;

    private Transform playerTransform;

    // == IPlayerInteractable ===================
    public string GetInteractActionDescription(Player playerScript) => "Sebrat";

    public bool CanInteract(Player playerScript)
    {
        return (Vector2.Distance(playerScript.transform.position, this.transform.position) <= 1.5f);
    }

    public void OnInteract(Player playerScript)
    {
        playerTransform = playerScript.transform;
        playerScript.hasFlash = true;
        ManagerAccessor.instance.ConsistencyManager.SetRecord(this.transform.name, false);

        this.GetComponent<SpriteRenderer>().enabled = false;
        this.transform.Find("Light").gameObject.SetActive(false);

        Invoke(nameof(Dialogue), 0.5f);
    }

    private void Dialogue()
    {
        Instantiate(triggerPrefab, playerTransform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }
}
