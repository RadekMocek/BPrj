using UnityEngine;

public class ManagerAccessor : MonoBehaviour
{
    public static ManagerAccessor instance = null;

    public HUDManager HUD { get; private set; }

    private void Awake()
    {
        if (instance == null) {
            instance = this;
        }
        else if (instance != this) {
            Destroy(this.gameObject);
            return;
        }

        DontDestroyOnLoad(this.gameObject);

        HUD = transform.Find("HUDManager").GetComponent<HUDManager>();
    }
}
