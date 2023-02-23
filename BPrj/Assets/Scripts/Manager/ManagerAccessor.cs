using UnityEngine;

public class ManagerAccessor : MonoBehaviour
{
    // Singleton + persistent, has all other managers as their child objects
    public static ManagerAccessor instance = null;

    public HUDManager HUD { get; private set; }

    private void Awake()
    {
        // Singleton
        if (instance == null) {
            instance = this;
        }
        else if (instance != this) {
            Destroy(this.gameObject);
            return;
        }

        // Persistent
        DontDestroyOnLoad(this.gameObject);

        // Find managers in children
        HUD = transform.Find("HUDManager").GetComponent<HUDManager>();
    }
}
