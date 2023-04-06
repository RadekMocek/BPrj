using UnityEngine;

public class Vent : MonoBehaviour
{
    //[SerializeField] private VentDoor ventDoorScript;
    
    private EnemyManager enemyManager;
    private int ventLayer;
    private int ventOpenLayer;

    private void Awake()
    {
        enemyManager = ManagerAccessor.instance.EnemyManager;
        ventLayer = LayerMask.NameToLayer("Vent");
        ventOpenLayer = LayerMask.NameToLayer("Vent_Open");
    }

    private void Update()
    {
        this.gameObject.layer = (/*!ventDoorScript.IsOpened || */!enemyManager.IsPlayerSneaking()) ? ventLayer : ventOpenLayer;
    }
}
