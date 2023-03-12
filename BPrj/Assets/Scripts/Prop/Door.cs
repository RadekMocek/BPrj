using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Door : MonoBehaviour, IObservable, IInteractable
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
        return (Opened) ? "Zavøít" : "Otevøít";
    }

    public bool CanInteract(Player playerScript)
    {
        return (Vector2.Distance(playerScript.transform.position, this.transform.position) <= 2.0f);
    }

    public void OnInteract(Player playerScript)
    {
        Opened = !Opened;
        ChangeDoorState(Opened);
    }

    // == Open/close door =======================
    [Header("Door parts")]
    [SerializeField] private SpriteRenderer[] doorPartSRs;
    [Header("Door sprites")]
    [SerializeField] private Sprite sprClosed;
    [SerializeField] private Sprite sprOpened;

    public bool Opened { get; private set; }

    private void ChangeDoorState(bool opened)
    {
        this.Opened = opened;

        CC.enabled = !opened;

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
        Opened = false;
        ChangeDoorState(Opened);
    }
}
