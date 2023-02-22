using UnityEngine;
using TMPro;

public class HUDManager : MonoBehaviour
{
    [Header("HUD – Observe & Interact")]
    [SerializeField] private TMP_Text observeNameText;
    [SerializeField] private TMP_Text interactActionText;

    public void SetObserveNameText(string value) => observeNameText.text = value;
    public void SetInteractActionText(string value) => interactActionText.text = value;
    public void SetIsInteractActionPossible(bool value) => interactActionText.color = (value) ? Color.white : Color.gray;
}
