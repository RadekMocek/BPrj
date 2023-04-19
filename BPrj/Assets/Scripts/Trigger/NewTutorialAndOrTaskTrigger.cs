using UnityEngine;

public class NewTutorialAndOrTaskTrigger : Trigger
{
    [Header("NewTutorialAndOrTaskTrigger")]
    [SerializeField] private bool newTutorial;
    [SerializeField] private bool newTask;

    private bool triggerDone;
    private GameObject playerGO;
    private Player playerScript;

    protected override void Start()
    {
        base.Start();
        triggerDone = false;
    }

    protected override void TriggerLogic()
    {
        playerGO = GameObject.Find("Player");
        playerScript = playerGO.GetComponent<Player>();
    }

    private void Update()
    {
        if (!triggerDone && triggered && playerScript.CurrentState != playerScript.DialogueState) {
            triggerDone = true;
            if (newTutorial) ManagerAccessor.instance.HUD.NewTutorial();
            if (newTask) ManagerAccessor.instance.HUD.NewTask();
        }
    }
}
