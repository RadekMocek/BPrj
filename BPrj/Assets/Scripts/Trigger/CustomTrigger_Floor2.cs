using UnityEngine;

public class CustomTrigger_Floor2 : DialogueTrigger
{
    [Header("Custom trigger – Floor 2")]
    [SerializeField] private SpriteRenderer tableSR;
    [SerializeField] private Sprite tableWithoutPhoneSprite;

    private readonly Vector2 targetLocation = new(-25.5f, 8.0f);

    private GameObject playerGO;
    private Player playerScript;
    private bool triggerEnd;

    protected override void TriggerLogic()
    {
        // Initialize
        triggerEnd = false;
        // Face left
        facingDirection = Direction.W;
        // DialogueTrigger
        base.TriggerLogic();
        // Push Player to the phone's location
        playerGO = GameObject.Find("Player");
        playerScript = playerGO.GetComponent<Player>();
        playerScript.RB.velocity = (4.7f * (targetLocation - (Vector2)playerGO.transform.position).normalized);
    }

    private void Update()
    {
        if (!triggerEnd && triggered && Vector2.Distance(playerGO.transform.position, targetLocation) < 0.25f) {
            playerScript.RB.velocity = Vector2.zero;
        }

        if (!triggerEnd && triggered && playerScript.CurrentState != playerScript.DialogueState) {
            triggerEnd = true;
            tableSR.sprite = tableWithoutPhoneSprite;
            HUD.NewTask();
        }
    }

    private void OnDisable()
    {
        if (tableSR != null) tableSR.sprite = tableWithoutPhoneSprite;
    }
}
