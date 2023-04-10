using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    // == HUD colors ============================
    private readonly Color barGreenColor = new(0.72f, 0.76f, 0.16f);
    private readonly Color barOrangeColor = new(0.76f, 0.62f, 0.16f);
    private readonly Color barRedColor = new(0.76f, 0.16f, 0.16f);

    // == References ============================
    private Player playerScript;
    public void SetPlayerScript(Player value) => playerScript = value;

    // == Observe & Interact ====================
    [Header("HUD – Observe & Interact")]
    [SerializeField] private TMP_Text observeNameText;
    [SerializeField] private TMP_Text interactActionText;
    [SerializeField] private GameObject observeHealthBarGO;

    private Slider observeHealthBarSlider;

    public void SetObserveNameText(string value) => observeNameText.text = value;
    public void SetInteractActionText(string value) => interactActionText.text = value;
    public void SetIsInteractActionPossible(bool value) => interactActionText.color = (value) ? Color.white : Color.gray;

    public void ShowObserveHealthBar(IObservableHealth enemy)
    {
        var healthInfo = enemy.GetHealthInfo();
        observeHealthBarSlider.maxValue = healthInfo.Item2;
        observeHealthBarSlider.value = healthInfo.Item1;
        observeHealthBarGO.SetActive(true);
    }

    public void HideObserveHealthBar() => observeHealthBarGO.SetActive(false);

    // == HealthBar =============================
    [Header("HUD – Health bar")]
    [SerializeField] private Slider healthBarSlider;

    public void SetMaxHealth(int max) => healthBarSlider.maxValue = max;
    public void SetHealth(int value) => healthBarSlider.value = value;

    // == StaminaBar ============================
    [Header("HUD – Stamina bar")]
    [SerializeField] private Slider staminaBarSlider;
    [SerializeField] private Image staminaBarImage;
    
    public void SetMaxStamina(int max) => staminaBarSlider.maxValue = max;

    public void SetStamina(int value)
    {
        staminaBarSlider.value = value;
        staminaBarImage.color = (value >= PlayerStaticValues.dash_staminaCost) ? barGreenColor : barOrangeColor;
    }

    // == Attack cooldown =======================
    [Header("HUD – Cooldown bar")]
    [SerializeField] private GameObject cooldownBarGO;
    [SerializeField] private RectTransform cooldownBarGreenAreaRT;
    [SerializeField] private Image cooldownBarHandleImage;

    private Slider cooldownBarSlider;
    private RectTransform cooldownBarRT;

    private readonly int cooldownBarWidth = 400;
    private readonly int cooldownBarHeight = 15;
    private readonly float cooldownBarGreenAreaPercentage = 20;

    public void ShowCooldownBar(float start, float duration, bool criticalHitMissed)
    {
        float barDuration = (100 * duration) / (100 - cooldownBarGreenAreaPercentage);
        var maxValue = start + barDuration;

        cooldownBarGO.SetActive(true);
        cooldownBarSlider.minValue = start;
        cooldownBarSlider.value = Time.time;
        cooldownBarSlider.maxValue = maxValue;

        cooldownBarHandleImage.color = (criticalHitMissed) ? barRedColor : (Time.time >= start + duration) ? barGreenColor : barOrangeColor;

        cooldownBarGreenAreaRT.gameObject.SetActive(!criticalHitMissed);

        if (Time.time >= maxValue) HideCooldownBar();
    }

    public void HideCooldownBar() => cooldownBarGO.SetActive(false);

    public bool IsCooldownBarVisible() => cooldownBarGO.activeSelf;

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
    [SerializeField] private GameObject dialogueIconGO;

    private bool isInDialogue;
    private Stack<string> dialogueStack;
    private Coroutine fillDialogueBoxCoroutine;
    private bool isFillDialogueBoxCoroutineRunning;

    public void StartDialogue(Stack<string> dialogueStack, Direction facingDirection)
    {
        this.dialogueStack = dialogueStack;
        fillDialogueBoxCoroutine = StartCoroutine(FillDialogueBox());
        dialogueMainGO.SetActive(true);
        playerScript.DialogueStart(facingDirection);
        isInDialogue = true;

        SetObserveNameText("");
        SetInteractActionText("");
    }

    private void EndDialogue()
    {
        dialogueMainGO.SetActive(false);
        dialogueIconGO.SetActive(false);
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

    private bool DialogueContinuePressed() =>
        playerScript.IH.InteractAction.WasPressedThisFrame() ||
        playerScript.IH.DashAction.WasPressedThisFrame() ||
        Input.GetKeyDown(KeyCode.Return) ||
        Input.GetKeyDown(KeyCode.KeypadEnter) ||
        Input.GetMouseButtonDown(0);

    public void ShowDialogueIcon() => dialogueIconGO.SetActive(true);

    // == MonoBehaviour =========================
    private void Awake()
    {
        // Observe & interact
        observeHealthBarGO.SetActive(false);
        observeHealthBarSlider = observeHealthBarGO.GetComponent<Slider>();

        // Cooldown bar
        cooldownBarGO.SetActive(false);
        cooldownBarSlider = cooldownBarGO.GetComponent<Slider>();
        cooldownBarRT = cooldownBarGO.GetComponent<RectTransform>();

        // Inspecting
        inspectMainGO.SetActive(false);
        
        // Dialogue
        dialogueMainGO.SetActive(false);
    }

    private void Start()
    {
        // Cooldown bar
        cooldownBarRT.sizeDelta = new(cooldownBarWidth, cooldownBarHeight);
        cooldownBarGreenAreaRT.sizeDelta = new((cooldownBarWidth / 100 * cooldownBarGreenAreaPercentage) - (cooldownBarWidth / 11), cooldownBarHeight - 10);

        // Inspecting
        IsInspecting = false;

        // Dialogue
        isInDialogue = false;
        isFillDialogueBoxCoroutineRunning = false;
    }

    private void Update()
    {
        // Dialogue
        if (isInDialogue && DialogueContinuePressed()) {
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
