using UnityEngine;

public class CustomTrigger_Floor1 : DialogueTrigger
{
    [Header("Custom trigger – Floor 1")]
    [SerializeField] private GameObject facadeGO;
    [SerializeField] private GameObject facadeOpaqueOnlyGO;
    [SerializeField] private GameObject secondDialogueTriggerPrefab;
    [SerializeField] private GameObject wallParticlePrefab;

    private GameObject playerGO;
    private Player playerScript;
    private bool triggerEnd;
    private bool secondDialogueSpawned;

    protected override void TriggerLogic()
    {
        triggerEnd = false;
        secondDialogueSpawned = false;
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

            Invoke(nameof(SecondDialogue), 1.2f);
        }
        else if (secondDialogueSpawned && playerScript.CurrentState != playerScript.DialogueState) {
            ManagerAccessor.instance.ConsistencyManager.SetRecord(facadeOpaqueOnlyGO.transform.name, false);
            facadeOpaqueOnlyGO.SetActive(false);
        }
    }

    private void SecondDialogue()
    {
        Instantiate(secondDialogueTriggerPrefab, playerGO.transform.position, Quaternion.identity);
        secondDialogueSpawned = true;
        ManagerAccessor.instance.HUD.NewTask();
    }
}
