using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Door : MonoBehaviour, IObservable, IPlayerInteractable
{
    // == Component references ==================
    private CapsuleCollider2D CC;
    private ShadowCaster2D shadowCasterScript;

    // == Observe ===============================
    public virtual string GetName()
    {
        return "Dveøe";
    }

    // == Interact =============================
    public string GetInteractActionDescription()
    {
        return (IsOpened) ? "Zavøít" : "Otevøít";
    }

    public bool CanInteract(Player playerScript)
    {
        return (Vector2.Distance(playerScript.transform.position, this.transform.position) <= 2.0f);
    }

    public void OnInteract(Player playerScript)
    {
        IsOpened = !IsOpened;
        ChangeDoorState(IsOpened);
    }

    // == Config ================================
    [Header("Settings")]
    [SerializeField] private bool openedDoorUnwalkable;

    // == Open/close door =======================
    [Header("Door parts")]
    [SerializeField] private SpriteRenderer[] doorPartSRs;
    [Header("Door sprites")]
    [SerializeField] private Sprite sprClosed;
    [SerializeField] private Sprite sprOpened;

    public bool IsOpened { get; private set; }

    private void ChangeDoorState(bool opened)
    {
        this.IsOpened = opened;

        if (!openedDoorUnwalkable) {
            CC.enabled = !opened;
        }
        else {
            CC.enabled = true;
            if (IsOpened) {
                CC.direction = CapsuleDirection2D.Vertical;
                CC.offset = new(0.87f, -0.25f);
                CC.size = new(0.17f, 1.47f);
            }
            else {
                CC.direction = CapsuleDirection2D.Horizontal;
                CC.offset = new(0, 0.06f);
                CC.size = new(2, 0.1f);
            }
        }

        shadowCasterScript.enabled = !opened;

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
        shadowCasterScript = GetComponent<ShadowCaster2D>();
    }

    private void Start()
    {
        IsOpened = false;
        ChangeDoorState(IsOpened);
    }
}
