using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : Trigger
{
    [Header("Dialogue trigger")]
    [TextArea(minLines: 2, maxLines: 3)] [SerializeField] private string[] dialogue;
    [SerializeField] private bool showDialogueIcon;

    protected HUDManager HUD;
    protected Direction facingDirection = Direction.S;

    private Stack<string> dialogueStack;

    private void Awake()
    {
        HUD = ManagerAccessor.instance.HUD;
    }

    protected override void Start()
    {
        base.Start();

        dialogueStack = new Stack<string>();
    }

    protected override void TriggerLogic()
    {
        if (dialogue.Length == 0) return;

        for (int i = dialogue.Length - 1; i >= 0; i--) {
            dialogueStack.Push(dialogue[i]);
        }

        if (showDialogueIcon) HUD.ShowDialogueIcon();

        HUD.StartDialogue(dialogueStack, facingDirection);
    }
}
