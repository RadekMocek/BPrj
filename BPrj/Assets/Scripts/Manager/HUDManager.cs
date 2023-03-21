using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    // == References ============================
    private Player playerScript;
    public void SetPlayerScript(Player value) => playerScript = value;

    // == Observe & Interact ====================
    [Header("HUD – Observe & Interact")]
    [SerializeField] private TMP_Text observeNameText;
    [SerializeField] private TMP_Text interactActionText;

    public void SetObserveNameText(string value) => observeNameText.text = value;
    public void SetInteractActionText(string value) => interactActionText.text = value;
    public void SetIsInteractActionPossible(bool value) => interactActionText.color = (value) ? Color.white : Color.gray;

    // == HealthBar =============================
    [Header("HUD – HealthBar")]
    [SerializeField] private Slider healthBarSlider;

    // == Inspect ===============================
    [Header("WIN – Inspect")]
    [SerializeField] private GameObject inspectMainGO;
    [SerializeField] private SVGImage inspectSVGImage;

    public bool IsInspecting { get; private set; }
    public IPlayerInteractable InspectedObjectScript { get; private set; }

    public void ShowInspectImage(Sprite sprite, IPlayerInteractable script)
    {
        inspectSVGImage.sprite = sprite;
        inspectMainGO.SetActive(true);
        InspectedObjectScript = script;
        IsInspecting = true;
        SetInteractActionText(interactActionText.text.Substring(0, interactActionText.text.IndexOf(')') + 1) + " Zavøít");
    }

    public void StopInspecting()
    {
        inspectMainGO.SetActive(false);
        InspectedObjectScript = null;
        IsInspecting = false;
    }

    // == Dialogue ==============================
    [Header("WIN – Dialogue")]
    [SerializeField] private GameObject dialogueMainGO;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private GameObject dialogueContinueIndicatorGO;

    private bool isInDialogue;
    private Stack<string> dialogueStack;
    private Coroutine fillDialogueBoxCoroutine;
    private bool isFillDialogueBoxCoroutineRunning;

    public void StartDialogue(Stack<string> dialogueStack)
    {
        this.dialogueStack = dialogueStack;
        fillDialogueBoxCoroutine = StartCoroutine(FillDialogueBox());
        dialogueMainGO.SetActive(true);
        playerScript.DialogueStart();
        isInDialogue = true;

        SetObserveNameText("");
        SetInteractActionText("");
    }

    private void EndDialogue()
    {
        dialogueMainGO.SetActive(false);
        playerScript.DialogueEnd();
        isInDialogue = false;
    }

    private IEnumerator FillDialogueBox()
    {
        isFillDialogueBoxCoroutineRunning = true;
        dialogueContinueIndicatorGO.SetActive(false);

        string line = dialogueStack.Peek();
        string dialogueBoxText = "";

        // Start filling dialogue box with letters
        foreach (char ch in line) {
            dialogueBoxText += ch;
            dialogueText.text = dialogueBoxText;
            yield return new WaitForSeconds(.015f);
        }

        dialogueContinueIndicatorGO.SetActive(true);
        isFillDialogueBoxCoroutineRunning = false;
    }

    // == MonoBehaviour =========================
    private void Awake()
    {
        // Inspecting
        inspectMainGO.SetActive(false);
        
        // Dialogue
        dialogueMainGO.SetActive(false);
    }

    private void Start()
    {
        // Inspecting
        IsInspecting = false;

        // Dialogue
        isInDialogue = false;
        isFillDialogueBoxCoroutineRunning = false;
    }

    private void Update()
    {
        // Dialogue
        if (isInDialogue && playerScript.IH.InteractAction.WasPressedThisFrame()) {
            if (isFillDialogueBoxCoroutineRunning) {
                isFillDialogueBoxCoroutineRunning = false;
                StopCoroutine(fillDialogueBoxCoroutine);
                dialogueText.text = dialogueStack.Peek(); // Skip filling with letters
                dialogueContinueIndicatorGO.SetActive(true);
            }
            else {
                dialogueStack.Pop(); // Handle the next line in stack (if there is any)
                if (dialogueStack.Any()) {
                    fillDialogueBoxCoroutine = StartCoroutine(FillDialogueBox());
                }
                else {
                    EndDialogue();
                }
            }
        }
    }
}
