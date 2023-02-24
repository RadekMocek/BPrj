using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private LayerMask playerLayer;

    [TextArea(minLines: 1, maxLines: 2)] [SerializeField] private string[] dialogue;

    private HUDManager HUD;

    private bool triggered;
    private Stack<string> dialogueStack;

    private void Awake()
    {
        HUD = ManagerAccessor.instance.HUD;
    }

    private void Start()
    {
        triggered = false;
        dialogueStack = new Stack<string>();
    }

    private void Update()
    {
        if (!triggered && Physics2D.OverlapCircle(this.transform.position, 1, playerLayer)) {
            triggered = true;

            if (dialogue.Length == 0) return;

            for (int i = dialogue.Length - 1; i >= 0; i--) {
                dialogueStack.Push(dialogue[i]);
            }
            
            HUD.StartDialogue(dialogueStack);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(this.transform.position, 1.0f);
    }
}
