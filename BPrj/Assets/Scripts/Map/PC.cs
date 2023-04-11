using UnityEngine;

public class PC : MonoBehaviour, IPlayerInteractable
{
    [SerializeField] private GameObject triggerGO;

    private Transform playerTransform;

    // == IPlayerInteractable ===================
    public string GetInteractActionDescription(Player playerScript) => (playerScript.hasFlash) ? "Vložit flash" : "Chybí flash";

    public bool CanInteract(Player playerScript)
    {
        return (playerScript.hasFlash && Vector2.Distance(playerScript.transform.position, this.transform.position) <= 1.5f);
    }

    public void OnInteract(Player playerScript)
    {
        playerTransform = playerScript.transform;

        playerScript.hasFlash = false;
        CameraShake.Instance.ShakeCamera();
        Invoke(nameof(Dialogue), 1.5f);
    }

    private void Dialogue()
    {
        triggerGO.transform.position = playerTransform.position;
        triggerGO.SetActive(true);
    }
}
