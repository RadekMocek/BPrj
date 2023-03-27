using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : Trigger
{
    [Header("Dialogue trigger")]
    [TextArea(minLines: 1, maxLines: 2)] [SerializeField] private string[] dialogue;

    private HUDManager HUD;
    
    private Stack<string> dialogueStack;

    private void Awake()
    {
        HUD = ManagerAccessor.instance.HUD;
    }

    private void Start()
    {
        dialogueStack = new Stack<string>();
    }

    protected override void TriggerLogic()
    {
        if (dialogue.Length == 0) return;

        for (int i = dialogue.Length - 1; i >= 0; i--) {
            dialogueStack.Push(dialogue[i]);
        }
            
        HUD.StartDialogue(dialogueStack);
    }
}
