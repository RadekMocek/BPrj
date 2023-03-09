using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IObservable, IInteractable
{
    // == Component references ==================
    private CapsuleCollider2D CC;

    // == Observe ===============================
    public virtual string GetName()
    {
        return "Dveøe";
    }

    // == Interact =============================
    public string GetInteractActionDescription()
    {
        return (opened) ? "Zavøít" : "Otevøít";
    }

    public bool CanInteract(Player playerScript)
    {
        return (Vector2.Distance(playerScript.transform.position, this.transform.position) <= 2.0f);
    }

    public void OnInteract(Player playerScript)
    {
        opened = !opened;
        ChangeDoorState(opened);
    }

    // == Open/close door =======================
    [Header("Door parts")]
    [SerializeField] private SpriteRenderer[] doorPartSRs;
    [Header("Door sprites")]
    [SerializeField] private Sprite sprClosed;
    [SerializeField] private Sprite sprOpened;

    private bool opened;

    private void ChangeDoorState(bool opened)
    {
        this.opened = opened;

        CC.enabled = !opened;

        foreach (var doorPartSR in doorPartSRs) {
            doorPartSR.sprite = (opened) ? sprOpened : sprClosed;
        }
    }

    public void OpenDoor() => ChangeDoorState(true);

    // == MonoBehaviour functions ===============
    private void Awake()
    {
        // Set components references
        CC = GetComponent<CapsuleCollider2D>();
    }

    private void Start()
    {
        opened = false;
        ChangeDoorState(opened);
    }
}
