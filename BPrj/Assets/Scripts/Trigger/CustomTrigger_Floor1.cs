using UnityEngine;

public class CustomTrigger_Floor1 : DialogueTrigger
{
    [Header("Custom trigger – Floor 1")]
    [SerializeField] private GameObject facadeGO;
    [SerializeField] private GameObject dialogueTriggerPrefab;
    [SerializeField] private GameObject wallParticlePrefab;

    private GameObject playerGO;
    private Player playerScript;
    private bool triggerEnd;

    protected override void TriggerLogic()
    {
        triggerEnd = false;
        base.TriggerLogic();
        playerGO = GameObject.Find("Player");
        playerScript = playerGO.GetComponent<Player>();
    }

    private void Update()
    {
        if (!triggerEnd && triggered && playerScript.CurrentState != playerScript.DialogueState) {
            triggerEnd = true;
            ManagerAccessor.instance.ConsistencyManager.SetRecord(facadeGO.transform.name, false);
            facadeGO.SetActive(false);

            CameraShake.Instance.ShakeCamera();
            Instantiate(wallParticlePrefab, (Vector2)facadeGO.transform.position + Vector2.up, Quaternion.identity);
            Instantiate(wallParticlePrefab, (Vector2)facadeGO.transform.position + Vector2.up + Vector2.left, Quaternion.identity);
            Instantiate(wallParticlePrefab, (Vector2)facadeGO.transform.position + Vector2.up + Vector2.right, Quaternion.identity);

            Invoke(nameof(Dialogue), 0.4f);
        }
    }

    private void Dialogue()
    {
        Instantiate(dialogueTriggerPrefab, playerGO.transform.position, Quaternion.identity);
    }
}
