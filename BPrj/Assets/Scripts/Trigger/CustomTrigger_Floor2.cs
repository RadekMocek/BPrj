using UnityEngine;

public class CustomTrigger_Floor2 : DialogueTrigger
{
    [Header("Custom trigger – Floor 2")]
    [SerializeField] SpriteRenderer tableSR;
    [SerializeField] Sprite tableWithoutPhoneSprite;

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
        var playerGO = GameObject.Find("Player");
        playerScript = playerGO.GetComponent<Player>();
        Vector2 targetLocation = new(-25.5f, 8.0f); 
        playerScript.RB.velocity = (4.7f * (targetLocation - (Vector2)playerGO.transform.position).normalized);
    }

    private void Update()
    {
        if (!triggerEnd && triggered && playerScript.CurrentState != playerScript.DialogueState) {
            triggerEnd = true;
            tableSR.sprite = tableWithoutPhoneSprite;
        }
    }

    private void OnDisable()
    {
        if (tableSR != null) tableSR.sprite = tableWithoutPhoneSprite;
    }
}
