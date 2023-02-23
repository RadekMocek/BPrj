using UnityEngine;
using TMPro;
using Unity.VectorGraphics;

public class HUDManager : MonoBehaviour
{
    // Observe & Interact
    [Header("HUD – Observe & Interact")]
    [SerializeField] private TMP_Text observeNameText;
    [SerializeField] private TMP_Text interactActionText;

    public void SetObserveNameText(string value) => observeNameText.text = value;
    public void SetInteractActionText(string value) => interactActionText.text = value;
    public void SetIsInteractActionPossible(bool value) => interactActionText.color = (value) ? Color.white : Color.gray;

    // Inspect
    [Header("WIN – Inspect")]
    [SerializeField] private GameObject inspectMainGO;
    [SerializeField] private SVGImage inspectSVGImage;

    public bool IsInspecting { get; private set; }
    public IInteractable InspectedObjectScript { get; private set; }

    public void ShowInspectImage(Sprite sprite, IInteractable script)
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

    // MonoBehaviour
    private void Awake()
    {
        inspectMainGO.SetActive(false);
    }

    private void Start()
    {
        IsInspecting = false;
    }
}
